using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Services;

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
builder.Services.AddControllersWithViews();

// ── Наши сервисы ─────────────────────────────────────────────────────
builder.Services.AddScoped<IPdfResumeService, PdfResumeService>();

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
