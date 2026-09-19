using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Areas.Admin.ViewModels;
using Portfolio_game_dev.Extensions;
using Portfolio_game_dev.Services.Abstractions;

namespace Portfolio_game_dev.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("admin/skills")]
public class SkillsController : Controller {

    public SkillsController() {
    }

    // GET: /admin/skills
    [HttpGet("")]
    public async Task<IActionResult> Index(string? search, string? category) {

        return View();
    }
}