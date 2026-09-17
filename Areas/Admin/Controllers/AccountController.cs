using Microsoft.AspNetCore.Mvc;

namespace Portfolio_game_dev.Areas.Admin.Controllers {
    /// <summary>
    /// Логин/логаут (Identity)
    /// </summary>
    [Area("Admin")]
    public class AccountController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
