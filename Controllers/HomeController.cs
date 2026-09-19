using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services.Abstractions;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Controllers;

/// <summary>
/// Публичный контроллер главной части сайта: главная, "О себе", контакты.
/// </summary>
public class HomeController : Controller {
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _db;                    // для Contact (сохранение сообщения)
    private readonly IProjectService _projects;
    private readonly ISkillService _skills;
    private readonly IBlogService _blog;

    public HomeController(
        ILogger<HomeController> logger,
        AppDbContext db,
        IProjectService projects,
        ISkillService skills,
        IBlogService blog) {
        _logger = logger;
        _db = db;
        _projects = projects;
        _skills = skills;
        _blog = blog;
    }

    public async Task<IActionResult> Index() {
        var vm = new HomeViewModel {
            Hero = new HeroViewModel {
                Tagline = "Game Designer, который превращает идеи в играбельные системы.",
                Subline = "5 лет в геймдизайне: от прототипов на Unity и Godot до релизов и плейтестов.",
                PrimaryCtaText = "Смотреть проекты",
                PrimaryCtaUrl = "/Projects",
                SecondaryCtaText = "Скачать резюме",
                SecondaryCtaUrl = "/Resume/Download",
            },

            FeaturedProjects = await _projects.GetFeaturedAsync(3),
            FeaturedSkills = await _skills.GetTopAsync(8),
            LatestPosts = await _blog.GetLatestAsync(3)
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