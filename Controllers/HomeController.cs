using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Controllers;

/// <summary>
/// Публичный контроллер главной части сайта: главная, "О себе", контакты.
/// TODO (День 2): заменить _db на IProjectService/ISkillService/IBlogService.
/// </summary>
public class HomeController : Controller {
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _db;

    public HomeController(ILogger<HomeController> logger, AppDbContext db) {
        _logger = logger;
        _db = db;
    }

    public async Task<IActionResult> Index() {
        var vm = new HomeViewModel {
            HeroTagline = "Game Designer, который превращает идеи в играбельные системы.",
            HeroSubline = "5 лет в геймдизайне: от прототипов на Unity и Godot до релизов и плейтестов.",

            FeaturedProjects = await _db.Projects
                .Where(p => p.IsPublished && p.IsFeatured)
                .OrderBy(p => p.SortOrder)
                .Take(3)
                .ToListAsync(),

            TopSkills = await _db.Skills
                .Where(s => s.IsFeatured)
                .OrderBy(s => s.SortOrder)
                .Take(8)
                .ToListAsync(),

            RecentExperience = await _db.Experiences
                .OrderByDescending(e => e.StartDate)
                .Take(3)
                .ToListAsync()
        };

        return View(vm);
    }

    // GET: /Home/About
    public IActionResult About() => View();

    // GET: /Home/Contact
    public IActionResult Contact() => View(new ContactViewModel());

    // POST: /Home/Contact
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactViewModel vm) {
        if (!string.IsNullOrWhiteSpace(vm.Website)) {
            _logger.LogWarning("Honeypot сработал: {Email}", vm.Email);
            return RedirectToAction(nameof(Contact));
        }

        if (!ModelState.IsValid)
            return View(vm);

        _db.ContactMessages.Add(new ContactMessage {
            Name = vm.Name,
            Email = vm.Email,
            Message = vm.Message
        });
        await _db.SaveChangesAsync();

        TempData["ContactSuccess"] = "Спасибо! Сообщение отправлено — отвечу в ближайшее время.";
        return RedirectToAction(nameof(Contact));
    }
}