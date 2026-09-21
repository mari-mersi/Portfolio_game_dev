using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Services.Abstractions;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Controllers;

public class SkillsController : Controller {
    private readonly ISkillService _skills;

    public SkillsController(ISkillService skills) {
        _skills = skills;
    }

    [HttpGet]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Index(CancellationToken ct = default) {
        var grouped = await _skills.GetGroupedByCategoryAsync(ct);
        var featured = await _skills.GetTopAsync(8, ct);

        var vm = new SkillsIndexViewModel {
            Categories = grouped
                .Select(kv => new SkillCategoryGroup {
                    Category = kv.Key,
                    Skills = kv.Value
                })
                .ToList(),
            FeaturedSkills = featured
        };

        ViewData["Title"] = "Навыки и стек";
        ViewData["Description"] = "Технологии, инструменты и навыки: Unity, C#, Unreal Engine, Godot.";
        return View(vm);
    }

    [HttpGet("skills/category/{category}")]
    public async Task<IActionResult> ByCategory(string category, CancellationToken ct = default) {
        if (string.IsNullOrWhiteSpace(category))
            return RedirectToAction(nameof(Index));

        var grouped = await _skills.GetGroupedByCategoryAsync(ct);

        var match = grouped
            .FirstOrDefault(kv => string.Equals(kv.Key, category, StringComparison.OrdinalIgnoreCase));

        if (match.Key is null)
            return NotFound();

        var vm = new SkillsIndexViewModel {
            Categories = new List<SkillCategoryGroup>
            {
                new() { Category = match.Key, Skills = match.Value }
            },
            FeaturedSkills = new List<Models.Skill>()
        };

        ViewData["Title"] = $"Навыки — {match.Key}";
        return View("Index", vm);
    }
}