using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Services.Abstractions;

namespace Portfolio_game_dev.Controllers;

/// <summary>
/// Таймлайн опыта работы.
/// </summary>
public class ExperienceController : Controller {
    private readonly IExperienceService _experience;

    public ExperienceController(IExperienceService experience) {
        _experience = experience;
    }

    /// <summary>
    /// GET: /experience
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default) {
        var vm = await _experience.GetTimelineAsync(ct);

        ViewData["Title"] = "Опыт работы";
        ViewData["Description"] = "Таймлайн студий и проектов, в которых я работал.";
        return View(vm);
    }
}