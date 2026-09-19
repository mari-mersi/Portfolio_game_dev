using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Controllers {
    /// <summary>
    /// Таймлайн опыта работы.
    /// </summary>
    public class ExperienceController : Controller {
        

        public ExperienceController() {
        }

        // GET: /experience
        public async Task<IActionResult> Index() {

            return View();
        }
    }
}