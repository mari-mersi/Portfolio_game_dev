using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services;
using Portfolio_game_dev.Areas.Admin.ViewModels;
using Portfolio_game_dev.Extensions;

namespace Portfolio_game_dev.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("admin/skills")]
public class SkillsController : Controller {
    private readonly ISkillService _skillService;
    private readonly IValidator<SkillEditViewModel> _validator;
    private readonly ILogger<SkillsController> _logger;

    public SkillsController(
        ISkillService skillService,
        IValidator<SkillEditViewModel> validator,
        ILogger<SkillsController> logger) {
        _skillService = skillService;
        _validator = validator;
        _logger = logger;
    }

    private static readonly string[] Categories =
    {
        "Backend", "Frontend", "DevOps", "Tools", "Soft Skills"
    };

    // GET: /admin/skills
    [HttpGet("")]
    public async Task<IActionResult> Index(string? search, string? category) {
        var all = await _skillService.GetAllAsync();

        var filtered = all.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
            filtered = filtered.Where(s =>
                s.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(category))
            filtered = filtered.Where(s =>
                string.Equals(s.Category, category, StringComparison.OrdinalIgnoreCase));

        var vm = new SkillsListViewModel {
            Skills = filtered.ToList(),
            SearchTerm = search,
            CategoryFilter = category,
            AvailableCategories = Categories.ToList()
        };

        return View(vm);
    }

    // GET: /admin/skills/create
    [HttpGet("create")]
    public IActionResult Create() {
        return View(new SkillEditViewModel {
            AvailableCategories = Categories.ToList(),
            Level = 3
        });
    }

    // POST: /admin/skills/create
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SkillEditViewModel vm) {
        var result = await _validator.ValidateAsync(vm);
        if (!result.IsValid) {
            result.AddToModelState(ModelState);
            vm.AvailableCategories = Categories.ToList();
            return View(vm);
        }

        await _skillService.CreateAsync(new Skill {
            Name = vm.Name.Trim(),
            Category = vm.Category,
            Level = vm.Level,
            IconUrl = string.IsNullOrWhiteSpace(vm.IconUrl) ? null : vm.IconUrl.Trim(),
            YearsOfExperience = vm.YearsOfExperience,
            IsFeatured = vm.IsFeatured
        });

        _logger.LogInformation("Создан навык {Name}", vm.Name);
        TempData["Success"] = $"Навык «{vm.Name}» добавлен.";

        return RedirectToAction(nameof(Index));
    }

    // GET: /admin/skills/edit/5
    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id) {
        var skill = await _skillService.GetByIdAsync(id);
        if (skill is null)
            return NotFound();

        return View(new SkillEditViewModel {
            Id = skill.Id,
            Name = skill.Name,
            Category = skill.Category,
            Level = skill.Level,
            IconUrl = skill.IconUrl,
            YearsOfExperience = skill.YearsOfExperience,
            IsFeatured = skill.IsFeatured,
            AvailableCategories = Categories.ToList()
        });
    }

    // POST: /admin/skills/edit/5
    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SkillEditViewModel vm) {
        if (id != vm.Id)
            return BadRequest();

        var result = await _validator.ValidateAsync(vm);
        if (!result.IsValid) {
            result.AddToModelState(ModelState);
            vm.AvailableCategories = Categories.ToList();
            return View(vm);
        }

        var existing = await _skillService.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        existing.Name = vm.Name.Trim();
        existing.Category = vm.Category;
        existing.Level = vm.Level;
        existing.IconUrl = string.IsNullOrWhiteSpace(vm.IconUrl) ? null : vm.IconUrl.Trim();
        existing.YearsOfExperience = vm.YearsOfExperience;
        existing.IsFeatured = vm.IsFeatured;

        await _skillService.UpdateAsync(existing);

        _logger.LogInformation("Обновлён навык {Id}", id);
        TempData["Success"] = $"Навык «{existing.Name}» обновлён.";

        return RedirectToAction(nameof(Index));
    }

    // GET: /admin/skills/delete/5
    [HttpGet("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        var skill = await _skillService.GetByIdAsync(id);
        if (skill is null)
            return NotFound();
        return View(skill);
    }

    // POST: /admin/skills/delete/5
    [HttpPost("delete/{id:int}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) {
        var skill = await _skillService.GetByIdAsync(id);
        if (skill is null)
            return NotFound();

        await _skillService.DeleteAsync(id);
        _logger.LogInformation("Удалён навык {Id}", id);
        TempData["Success"] = $"Навык «{skill.Name}» удалён.";

        return RedirectToAction(nameof(Index));
    }
}