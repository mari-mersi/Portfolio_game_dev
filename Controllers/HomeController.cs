using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Controllers {
    /// <summary>
    /// ѕубличный контроллер главной части сайта: главна€, "ќ себе", контакты.
    /// </summary>
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IContentService _content;
        private readonly IExperienceService _experience;

        public HomeController(
            ILogger<HomeController> logger,
            IContentService content,
            IExperienceService experience) {
            _logger = logger;
            _content = content;
            _experience = experience;
        }

        public async Task<IActionResult> Index() {
            var vm = new HomeViewModel {
                HeroTagline = "Game Designer, который превращает идеи в играбельные системы.",
                HeroSubline = "5 лет в геймдизайне: от прототипов на Unity и Godot до релизов и плейтестов.",
                FeaturedProjects = await _content.GetFeaturedProjectsAsync(3),
                TopSkills = await _content.GetTopSkillsAsync(8),
                RecentExperience = await _experience.GetRecentAsync(3)
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
            // Honeypot: если бот заполнил скрытое поле Ч тихо редиректим "как будто ок".
            if (!string.IsNullOrWhiteSpace(vm.Website)) {
                _logger.LogWarning("Honeypot сработал: {Email}", vm.Email);
                return RedirectToAction(nameof(Contact));
            }

            if (!ModelState.IsValid)
                return View(vm);

            await _content.SaveContactMessageAsync(new ContactMessage {
                Name = vm.Name,
                Email = vm.Email,
                Message = vm.Message,
                CreatedAt = DateTime.UtcNow
            });

            TempData["ContactSuccess"] = "—пасибо! —ообщение отправлено Ч отвечу в ближайшее врем€.";
            return RedirectToAction(nameof(Contact));
        }
    }
}