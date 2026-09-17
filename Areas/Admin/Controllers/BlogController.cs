using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio_game_dev.Areas.Admin.Controllers {
    [Area("Admin")]
    //[Authorize] // если админка защищена — оставить; пока не настроен Identity, закомментируйте
    public class BlogController : Controller {
        public IActionResult Index() => View();
        // ... остальные экшены
    }
}