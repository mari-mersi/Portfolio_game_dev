using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Areas.Admin.ViewModels;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SkillsController : Controller {
    private readonly AppDbContext _db;

    public SkillsController(AppDbContext db) {
        _db = db;
    }

    // ── Index ────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? category, CancellationToken ct) {
        var query = _db.Skills.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search)) {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(x => x.Name.ToLower().Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(x => x.Category == category);

        var skills = await query
            .OrderBy(x => x.Category)
            .ThenBy(x => x.SortOrder)
            .ThenByDescending(x => x.Level)
            .ToListAsync(ct);

        // Список категорий — из всех навыков в БД (не из отфильтрованных!)
        var allCategories = await _db.Skills
            .AsNoTracking()
            .Select(x => x.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(ct);

        var vm = new SkillsListViewModel {
            Skills = skills,
            SearchTerm = search,
            CategoryFilter = category,
            AvailableCategories = allCategories
        };

        ViewData["Title"] = "Навыки";
        return View(vm);
    }

    // ── Create GET ───────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct) {
        var vm = new SkillEditViewModel {
            Level = 3,
            AvailableCategories = await LoadCategoryOptionsAsync(ct)
        };

        ViewData["Title"] = "Новый навык";
        return View(vm);
    }

    // ── Create POST ──────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SkillEditViewModel vm, CancellationToken ct) {
        if (!ModelState.IsValid) {
            vm.AvailableCategories = await LoadCategoryOptionsAsync(ct);
            return View(vm);
        }

        var skill = new Skill {
            Name = vm.Name.Trim(),
            Category = vm.Category.Trim(),
            Level = vm.Level,
            IconUrl = string.IsNullOrWhiteSpace(vm.IconUrl) ? null : vm.IconUrl.Trim(),
            YearsOfExperience = vm.YearsOfExperience,
            IsFeatured = vm.IsFeatured,
            SortOrder = 0
        };

        _db.Skills.Add(skill);
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"Навык «{skill.Name}» создан.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit GET ─────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct) {
        var skill = await _db.Skills.FindAsync(new object[] { id }, ct);
        if (skill is null)
            return NotFound();

        var vm = new SkillEditViewModel {
            Id = skill.Id,
            Name = skill.Name,
            Category = skill.Category,
            Level = skill.Level,
            IconUrl = skill.IconUrl,
            YearsOfExperience = skill.YearsOfExperience,
            IsFeatured = skill.IsFeatured,
            AvailableCategories = await LoadCategoryOptionsAsync(ct)
        };

        ViewData["Title"] = $"Редактирование: {skill.Name}";
        return View(vm);
    }

    // ── Edit POST ────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SkillEditViewModel vm, CancellationToken ct) {
        if (id != vm.Id)
            return BadRequest();

        var skill = await _db.Skills.FindAsync(new object[] { id }, ct);
        if (skill is null)
            return NotFound();

        if (!ModelState.IsValid) {
            vm.AvailableCategories = await LoadCategoryOptionsAsync(ct);
            return View(vm);
        }

        skill.Name = vm.Name.Trim();
        skill.Category = vm.Category.Trim();
        skill.Level = vm.Level;
        skill.IconUrl = string.IsNullOrWhiteSpace(vm.IconUrl) ? null : vm.IconUrl.Trim();
        skill.YearsOfExperience = vm.YearsOfExperience;
        skill.IsFeatured = vm.IsFeatured;
        skill.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"Навык «{skill.Name}» обновлён.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete GET ───────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) {
        var skill = await _db.Skills.FindAsync(new object[] { id }, ct);
        if (skill is null)
            return NotFound();

        ViewData["Title"] = "Удалить навык";
        return View(skill);
    }

    // ── Delete POST ──────────────────────────────────────────────────
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct) {
        var skill = await _db.Skills.FindAsync(new object[] { id }, ct);
        if (skill is null)
            return NotFound();

        _db.Skills.Remove(skill);
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"Навык «{skill.Name}» удалён.";
        return RedirectToAction(nameof(Index));
    }

    // ── Helper ───────────────────────────────────────────────────────
    private async Task<List<string>> LoadCategoryOptionsAsync(CancellationToken ct) {
        var fromDb = await _db.Skills
            .AsNoTracking()
            .Select(s => s.Category)
            .Distinct()
            .ToListAsync(ct);

        // Дефолтные категории на случай пустой БД
        var defaults = new[] { "Программирование", "Инструменты", "Графика", "Дизайн", "Общее"};

        return fromDb
            .Union(defaults)
            .OrderBy(c => c)
            .ToList();
    }
}