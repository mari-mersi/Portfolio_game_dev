using Microsoft.AspNetCore.Mvc;

namespace Portfolio_game_dev.Areas.Admin.Controllers {
    /// <summary>
    /// Главная панель админа со статистикой
    /// </summary>
    [Area("Admin")]
    public class DashboardController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
