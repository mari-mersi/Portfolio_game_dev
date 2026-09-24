using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Areas.Admin.ViewModels;
using Portfolio_game_dev.Data;

namespace Portfolio_game_dev.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller {
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db) {
        _db = db;
    }

    public async Task<IActionResult> Index(CancellationToken ct) {
        var vm = new DashboardViewModel {
            ProjectsCount = await _db.Projects.CountAsync(ct),
            PublishedProjectsCount = await _db.Projects.CountAsync(p => p.IsPublished, ct),
            SkillsCount = await _db.Skills.CountAsync(ct),
            ExperiencesCount = await _db.Experiences.CountAsync(ct),
            PostsCount = await _db.BlogPosts.CountAsync(ct),
            PublishedPostsCount = await _db.BlogPosts.CountAsync(p => p.IsPublished, ct),
            UnreadMessagesCount = await _db.ContactMessages.CountAsync(m => !m.IsRead, ct),
            TotalMessagesCount = await _db.ContactMessages.CountAsync(ct),
        };

        ViewData["Title"] = "Дашборд";
        return View(vm);
    }
}