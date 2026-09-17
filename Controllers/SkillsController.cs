using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.ViewModels;
using Portfolio_game_dev.Services;

namespace Portfolio_game_dev.Controllers;

public class SkillsController : Controller {
    private readonly ISkillService _skillService;
    private readonly ILogger<SkillsController> _logger;

    public SkillsController(ISkillService skillService, ILogger<SkillsController> logger) {
        _skillService = skillService;
        _logger = logger;
    }

    // GET: /skills
    [HttpGet]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Index() {
        var skills = await _skillService.GetAllAsync();

        var vm = new SkillsIndexViewModel {
            FeaturedSkills = skills.Where(s => s.IsFeatured).ToList(),
            Categories = skills
                .GroupBy(s => s.Category)
                .Select(g => new SkillCategoryGroup {
                    Category = g.Key,
                    Skills = g.OrderByDescending(s => s.Level).ToList()
                })
                .OrderBy(g => CategoryOrder(g.Category))
                .ToList()
        };

        ViewData["Title"] = "Навыки и стек";
        ViewData["Description"] = "Технологии, инструменты и навыки, которые я использую в работе.";

        return View(vm);
    }

    // GET: /skills/category/backend
    [HttpGet("skills/category/{category}")]
    public async Task<IActionResult> ByCategory(string category) {
        var skills = await _skillService.GetByCategoryAsync(category);
        if (skills.Count == 0)
            return NotFound();

        ViewData["Title"] = $"Навыки: {category}";
        return View("Index", new SkillsIndexViewModel {
            Categories = new List<SkillCategoryGroup>
            {
                new() { Category = category, Skills = skills }
            },
            FeaturedSkills = skills.Where(s => s.IsFeatured).ToList()
        });
    }

    private static int CategoryOrder(string category) => category switch {
        "Backend" => 1,
        "Frontend" => 2,
        "DevOps" => 3,
        "Tools" => 4,
        "Soft Skills" => 5,
        _ => 99
    };
}