using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio_game_dev.Areas.Admin.Controllers {
    /// <summary>
    /// CRUD для проектов
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProjectsController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
