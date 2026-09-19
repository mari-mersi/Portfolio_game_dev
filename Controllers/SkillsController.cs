using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.ViewModels;
using Portfolio_game_dev.Services.Abstractions;

namespace Portfolio_game_dev.Controllers;

public class SkillsController : Controller {

    public SkillsController() {
    }

    // GET: /skills
    [HttpGet]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Index() {
        

        return View();
    }
}