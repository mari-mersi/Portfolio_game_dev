using Portfolio_game_dev.Services;
using Portfolio_game_dev.Validators;
using Portfolio_game_dev.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IContentService, MockContentService>();
builder.Services.AddScoped<IBlogService, MockBlogService>();
builder.Services.AddScoped<IExperienceService, MockExperienceService>();
builder.Services.AddScoped<IPdfResumeService, PdfResumeService>();
builder.Services.AddScoped<ISkillService, MockSkillService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    // 500-е: редирект на ErrorController.Error
    app.UseExceptionHandler("/error");

    // 404-е и прочие статусы без тела: ре-выполнение запроса на ErrorController
    app.UseStatusCodePagesWithReExecute("/error/{0}");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
