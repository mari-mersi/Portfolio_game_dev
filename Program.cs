using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Services.Abstractions;
using Portfolio_game_dev.Services.Implementations;
using System.Globalization;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;

// до builder.Build():
var ruCulture = new CultureInfo("ru-RU");
CultureInfo.DefaultThreadCurrentCulture = ruCulture;
CultureInfo.DefaultThreadCurrentUICulture = ruCulture;

var builder = WebApplication.CreateBuilder(args);

// ── DbContext ────────────────────────────────────────────────────────
// Строим путь к БД от ContentRootPath, а не от текущей рабочей директории.
// Иначе dotnet ef и dotnet run будут смотреть в разные portfolio.db.
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "portfolio.db");
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite($"Data Source={dbPath}"));

// ── Identity ─────────────────────────────────────────────────────────
// IdentityUser/IdentityRole — стандартные модели. Своих пока не заводим.
builder.Services.AddIdentity<IdentityUser, IdentityRole>(o => {
    o.User.RequireUniqueEmail = true;      // один email = один аккаунт
    o.SignIn.RequireConfirmedAccount = false; // без подтверждения email (локально удобнее)
})
.AddEntityFrameworkStores<AppDbContext>()  // Identity хранит данные в нашей БД
.AddDefaultTokenProviders();               // токены для сброса пароля и т.п.

// Куда редиректить, если юзер не залогинен или не имеет доступа.
builder.Services.ConfigureApplicationCookie(o => {
    o.LoginPath = "/Admin/Account/Login";
    o.AccessDeniedPath = "/Admin/Account/Login";
});

// ── MVC ──────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options => {
        // Искать partial view'хи в Views/Shared/Partials/ по короткому имени
        options.ViewLocationFormats.Add("/Views/{1}/Partials/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Shared/Partials/{0}.cshtml");

        // И для Areas (пригодится в День 8+)
        options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/Partials/{0}.cshtml");
        options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/Partials/{0}.cshtml");
    });

// ── Наши сервисы ─────────────────────────────────────────────────────
builder.Services.AddScoped<IPdfResumeService, PdfResumeService>();

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();

// ── QuestPDF: регистрация встроенного шрифта ─────────────────────────
// Без регистрации QuestPDF не сможет найти "Inter" в момент генерации PDF.
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

//QuestPDF.Settings.UseEnvironmentFonts = false;

var fontsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "fonts");
// стало — рекурсивная регистрация
if (Directory.Exists(fontsPath)) {
    var fontFiles = Directory.GetFiles(fontsPath, "*.*", SearchOption.AllDirectories)
        .Where(f => f.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase)
                 || f.EndsWith(".otf", StringComparison.OrdinalIgnoreCase));

    foreach (var fontFile in fontFiles) {
        QuestPDF.Drawing.FontManager.RegisterFontFromFile(fontFile);
    }
    // Временная отладка — увидеть, что реально зарегистрировано
#if DEBUG
    var registered = QuestPDF.Drawing.FontManager.GetRegisteredFonts();
    Console.WriteLine($"[QuestPDF] Registered fonts: {string.Join(", ", registered)}");
#endif
}

var app = builder.Build();

// ── Pipeline ─────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/error");            // 500 → ErrorController.Error
    app.UseStatusCodePagesWithReExecute("/error/{0}"); // 404 и прочие → ErrorController
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();   // обязательно ДО UseAuthorization
app.UseAuthorization();

// Маршрутизация: сначала Areas, потом default.
// {area:exists} — совпадает, только если Area с таким именем зарегистрирована.
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ── Seed ─────────────────────────────────────────────────────────────
// Отдельный scope: DbContext и UserManager — scoped, вне scope их не достать.
// await обязателен, иначе приложение стартует до заполнения БД.
using (var scope = app.Services.CreateScope()) {
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
