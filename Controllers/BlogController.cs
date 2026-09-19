using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Services;
using Portfolio_game_dev.Services.Abstractions;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Controllers {
    /// <summary>
    /// Статьи и девлоги.
    /// </summary>
    public class BlogController : Controller {
        public BlogController() {
        }

        // GET: /blog
        // GET: /blog?page=2
        public async Task<IActionResult> Index(int page = 1) {

            return View();
        }
    }
}