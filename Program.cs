using System.Globalization;
using System.Threading.RateLimiting;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Extensions;
using Portfolio_game_dev.Services.Abstractions;
using Portfolio_game_dev.Services.Implementations;
using Serilog;
using Portfolio_game_dev.Middleware;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

// ── Культура ru-RU ───────────────────────────────────────────────────
var ruCulture = new CultureInfo("ru-RU");
CultureInfo.DefaultThreadCurrentCulture = ruCulture;
CultureInfo.DefaultThreadCurrentUICulture = ruCulture;

var builder = WebApplication.CreateBuilder(args);
// Render даёт порт через переменную PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext());

// ── DbContext ────────────────────────────────────────────────────────
// Путь к БД строим от ContentRootPath, а не от текущей директории,
// иначе dotnet ef и dotnet run смотрят в разные portfolio.db.
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "portfolio.db");
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite($"Data Source={dbPath}"));

// ── Identity ─────────────────────────────────────────────────────────
builder.Services.AddIdentity<IdentityUser, IdentityRole>(o => {
    o.User.RequireUniqueEmail = true;
    o.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(o => {
    o.LoginPath = "/Admin/Account/Login";
    o.AccessDeniedPath = "/Admin/Account/Login";
});

// ── MVC + ViewLocationFormats ────────────────────────────────────────
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options => {
        // Искать partial view'хи в Views/.../Partials/ по короткому имени
        options.ViewLocationFormats.Add("/Views/{1}/Partials/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Shared/Partials/{0}.cshtml");

        // И для Areas
        options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/Partials/{0}.cshtml");
        options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/Partials/{0}.cshtml");
    });

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database", tags: new[] { "db", "sqlite" });

builder.Services.AddOutputCache(options => {
    options.AddPolicy("public", policy => policy
        .Expire(TimeSpan.FromSeconds(60))
        .Tag("public"));
});

// ── HttpClient (нужен для ReCaptchaService) ──────────────────────────
builder.Services.AddHttpClient();

// ── Rate Limiter ─────────────────────────────────────────────────────
// Политика "contact" — 3 запроса в минуту с одного IP.
builder.Services.AddRateLimiter(options => {
    options.AddFixedWindowLimiter("contact", opt => {
        opt.PermitLimit = 3;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// FluentValidation: валидаторы и авто-интеграция с ASP.NET Core
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddResponseCompression(options => {
    options.EnableForHttps = true;
    options.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults.MimeTypes
        .Concat(new[] { "image/svg+xml", "application/xml" });
});

// ── Наши сервисы ─────────────────────────────────────────────────────
builder.Services.AddScoped<IPdfResumeService, PdfResumeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IReCaptchaService, ReCaptchaService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ISlugService, SlugService>();

// ── QuestPDF: license + регистрация шрифтов ──────────────────────────
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Раскомментировать перед деплоем на прод (Linux-контейнер без шрифтов):
// QuestPDF.Settings.UseEnvironmentFonts = false;

var fontsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "fonts");
if (Directory.Exists(fontsPath)) {
    var fontFiles = Directory.GetFiles(fontsPath, "*.*", SearchOption.AllDirectories)
        .Where(f => f.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase)
                 || f.EndsWith(".otf", StringComparison.OrdinalIgnoreCase));

    foreach (var fontFile in fontFiles) {
        QuestPDF.Drawing.FontManager.RegisterFontFromFile(fontFile);
    }

#if DEBUG
    var registered = QuestPDF.Drawing.FontManager.GetRegisteredFonts();
    Console.WriteLine($"[QuestPDF] Registered fonts: {string.Join(", ", registered)}");
#endif
}

var app = builder.Build();

// ── Pipeline ─────────────────────────────────────────────────────────
// Красивые страницы ошибок — и в dev, и в prod.
app.UseExceptionHandler("/error");
app.UseStatusCodePagesWithReExecute("/error/{0}");

if (!app.Environment.IsDevelopment()) {
    // HSTS + HTTPS-редирект — только в Production.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseResponseCompression();
app.UseSecurityHeaders();
app.UseStaticFiles();

app.UseRouting();

// Rate Limiter — после UseRouting, до UseAuthorization
app.UseRateLimiter();

app.UseAuthentication();   // обязательно ДО UseAuthorization
app.UseAuthorization();

app.UseOutputCache();

// Маршрутизация: сначала Areas, потом default
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions {
    ResponseWriter = async (context, report) => {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        });
        await context.Response.WriteAsync(result);
    }
});

// ── Seed ─────────────────────────────────────────────────────────────
// Отдельный scope: DbContext и UserManager — scoped.
try {
    using var scope = app.Services.CreateScope();
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}
catch (Exception ex) {
    Log.Fatal(ex, "Ошибка при применении миграций/сидинге");
    throw;
}

try {
    app.Run();
}
catch (Exception ex) {
    Log.Fatal(ex, "Приложение упало при старте");
}
finally {
    Log.CloseAndFlush();
}