using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio_game_dev.Areas.Admin.Controllers {
    /// <summary>
    /// CRUD для опыта работы
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ExperienceController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
