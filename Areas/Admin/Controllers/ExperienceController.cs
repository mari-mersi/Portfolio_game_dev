using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Areas.Admin.ViewModels;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ExperienceController : Controller {
    private readonly AppDbContext _db;

    public ExperienceController(AppDbContext db) {
        _db = db;
    }

    // ── Index ────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? company, CancellationToken ct) {
        var query = _db.Experiences.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search)) {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(e =>
                e.Company.ToLower().Contains(s) ||
                e.Position.ToLower().Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(company))
            query = query.Where(e => e.Company == company);

        var experiences = await query
            .OrderByDescending(e => e.StartDate)
            .ToListAsync(ct);

        var allCompanies = await _db.Experiences
            .AsNoTracking()
            .Select(e => e.Company)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(ct);

        var vm = new ExperiencesListViewModel {
            Experiences = experiences,
            SearchTerm = search,
            CompanyFilter = company,
            AvailableCompanies = allCompanies
        };

        ViewData["Title"] = "Опыт работы";
        return View(vm);
    }

    // ── Create GET ───────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Create() {
        var vm = new ExperienceEditViewModel {
            StartDate = DateTime.Today
        };
        ViewData["Title"] = "Новое место работы";
        return View(vm);
    }

    // ── Create POST ──────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExperienceEditViewModel vm, CancellationToken ct) {
        // Валидация дат вручную
        if (vm.EndDate.HasValue && vm.EndDate < vm.StartDate)
            ModelState.AddModelError(nameof(vm.EndDate), "Дата окончания не может быть раньше даты начала");

        if (vm.IsCurrent && vm.EndDate.HasValue)
            ModelState.AddModelError(nameof(vm.EndDate), "Для текущего места работы дата окончания не указывается");

        if (!ModelState.IsValid)
            return View(vm);

        var experience = new Experience {
            Company = vm.Company.Trim(),
            Position = vm.Position.Trim(),
            EmploymentType = vm.EmploymentType?.Trim(),
            Location = vm.Location?.Trim(),
            StartDate = vm.StartDate,
            EndDate = vm.IsCurrent ? null : vm.EndDate,
            IsCurrent = vm.IsCurrent,
            Description = vm.Description?.Trim(),
            Highlights = ParseHighlights(vm.HighlightsText),
            TechStack = vm.TechStack?.Trim(),
            SortOrder = vm.SortOrder
        };

        _db.Experiences.Add(experience);
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"Опыт «{experience.Position} — {experience.Company}» добавлен.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit GET ─────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct) {
        var experience = await _db.Experiences.FindAsync(new object[] { id }, ct);
        if (experience is null)
            return NotFound();

        var vm = new ExperienceEditViewModel {
            Id = experience.Id,
            Company = experience.Company,
            Position = experience.Position,
            EmploymentType = experience.EmploymentType,
            Location = experience.Location,
            StartDate = experience.StartDate,
            EndDate = experience.EndDate,
            IsCurrent = experience.IsCurrent,
            Description = experience.Description,
            HighlightsText = experience.Highlights.Count == 0
                ? string.Empty
                : string.Join(Environment.NewLine, experience.Highlights),
            TechStack = experience.TechStack,
            SortOrder = experience.SortOrder
        };

        ViewData["Title"] = $"Редактирование: {experience.Position}";
        return View(vm);
    }

    // ── Edit POST ────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExperienceEditViewModel vm, CancellationToken ct) {
        if (id != vm.Id)
            return BadRequest();

        var experience = await _db.Experiences.FindAsync(new object[] { id }, ct);
        if (experience is null)
            return NotFound();

        if (vm.EndDate.HasValue && vm.EndDate < vm.StartDate)
            ModelState.AddModelError(nameof(vm.EndDate), "Дата окончания не может быть раньше даты начала");

        if (vm.IsCurrent && vm.EndDate.HasValue)
            ModelState.AddModelError(nameof(vm.EndDate), "Для текущего места работы дата окончания не указывается");

        if (!ModelState.IsValid)
            return View(vm);

        experience.Company = vm.Company.Trim();
        experience.Position = vm.Position.Trim();
        experience.EmploymentType = vm.EmploymentType?.Trim();
        experience.Location = vm.Location?.Trim();
        experience.StartDate = vm.StartDate;
        experience.EndDate = vm.IsCurrent ? null : vm.EndDate;
        experience.IsCurrent = vm.IsCurrent;
        experience.Description = vm.Description?.Trim();
        experience.Highlights = ParseHighlights(vm.HighlightsText);
        experience.TechStack = vm.TechStack?.Trim();
        experience.SortOrder = vm.SortOrder;
        experience.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"Опыт «{experience.Position} — {experience.Company}» обновлён.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete GET ───────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) {
        var experience = await _db.Experiences.FindAsync(new object[] { id }, ct);
        if (experience is null)
            return NotFound();

        ViewData["Title"] = "Удаление опыта";
        return View(experience);
    }

    // ── Delete POST ──────────────────────────────────────────────────
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct) {
        var experience = await _db.Experiences.FindAsync(new object[] { id }, ct);
        if (experience is null)
            return NotFound();

        _db.Experiences.Remove(experience);
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"Опыт «{experience.Position} — {experience.Company}» удалён.";
        return RedirectToAction(nameof(Index));
    }

    // ── Helper ───────────────────────────────────────────────────────
    private static List<string> ParseHighlights(string? text) {
        if (string.IsNullOrWhiteSpace(text))
            return new List<string>();

        return text
            .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l))
            .ToList();
    }
}