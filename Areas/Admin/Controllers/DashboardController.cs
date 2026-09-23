using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio_game_dev.Areas.Admin.Controllers;

/// <summary>
/// Главная панель админа. Пока — заглушка, статистика будет позже.
/// </summary>
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller {
    public IActionResult Index() {
        ViewData["Title"] = "Дашборд";
        return View();
    }
}