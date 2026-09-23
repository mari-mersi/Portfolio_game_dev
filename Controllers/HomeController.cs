using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio_game_dev.Services.Abstractions;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Controllers;

public class HomeController : Controller {
    private readonly ILogger<HomeController> _logger;
    private readonly IProjectService _projects;
    private readonly ISkillService _skills;
    private readonly IBlogService _blog;
    private readonly IContactService _contact;
    private readonly IReCaptchaService _recaptcha;

    public HomeController(
        ILogger<HomeController> logger,
        IProjectService projects,
        ISkillService skills,
        IBlogService blog,
        IContactService contact,
        IReCaptchaService recaptcha) {
        _logger = logger;
        _projects = projects;
        _skills = skills;
        _blog = blog;
        _contact = contact;
        _recaptcha = recaptcha;
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

    public IActionResult About() => View();

    // GET: /Contact
    [HttpGet]
    public IActionResult Contact() => View(new ContactViewModel());

    // POST: /Contact
    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("contact")]
    public async Task<IActionResult> Contact(ContactViewModel vm, CancellationToken ct) {
        // 1. Honeypot — если заполнено, тихо редиректим (не палим бота)
        if (!string.IsNullOrWhiteSpace(vm.Website)) {
            _logger.LogWarning("Honeypot сработал: {Email}", vm.Email);
            TempData["ContactSuccess"] = "Спасибо! Сообщение отправлено.";
            return RedirectToAction(nameof(Contact));
        }

        // 2. ModelState (Data Annotations + IValidatableObject антиспам)
        if (!ModelState.IsValid)
            return View(vm);

        // 3. reCAPTCHA
        var captchaOk = await _recaptcha.ValidateAsync(vm.RecaptchaToken, ct);
        if (!captchaOk) {
            ModelState.AddModelError("", "Проверка reCAPTCHA не пройдена. Попробуйте ещё раз.");
            return View(vm);
        }

        // 4. Хэш IP для аналитики (SHA-256 + соль)
        var ipHash = ComputeIpHash(HttpContext.Connection.RemoteIpAddress?.ToString());

        // 5. Сохраняем
        await _contact.SaveAsync(vm, ipHash, ct);

        TempData["ContactSuccess"] = "Спасибо! Сообщение отправлено — отвечу в ближайшее время.";
        return RedirectToAction(nameof(Contact));
    }

    private static string? ComputeIpHash(string? ip) {
        if (string.IsNullOrEmpty(ip))
            return null;

        // Соль — можно вынести в appsettings, но для пет-проекта хватит константы.
        const string salt = "portfolio-game-dev-salt";
        var bytes = System.Text.Encoding.UTF8.GetBytes(ip + salt);
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}