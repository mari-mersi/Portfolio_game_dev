using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Areas.Admin.ViewModels;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services.Abstractions;

namespace Portfolio_game_dev.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProjectsController : Controller {
    private readonly AppDbContext _db;
    private readonly IImageService _images;
    private readonly ISlugService _slugs;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(
        AppDbContext db,
        IImageService images,
        ISlugService slugs,
        ILogger<ProjectsController> logger) {
        _db = db;
        _images = images;
        _slugs = slugs;
        _logger = logger;
    }

    // ── Index ────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Index(string? search, CancellationToken ct) {
        var query = _db.Projects.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search)) {
            var s = search.Trim();
            query = query.Where(p => p.Title.Contains(s) || p.Slug.Contains(s));
        }

        var projects = await query
            .Include(p => p.Tags)
            .OrderBy(p => p.SortOrder)
            .ThenByDescending(p => p.ReleaseDate)
            .ToListAsync(ct);

        ViewData["Title"] = "Проекты";
        ViewData["SearchTerm"] = search;
        return View(projects);
    }

    // ── Create GET ───────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct) {
        var vm = new ProjectEditViewModel {
            IsPublished = true,
            AvailableTags = await LoadTagOptionsAsync(null, ct)
        };

        ViewData["Title"] = "Новый проект";
        return View(vm);
    }

    // ── Create POST ──────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectEditViewModel vm, CancellationToken ct) {
        if (!ModelState.IsValid) {
            vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);
            return View(vm);
        }

        // Slug: если не введён — генерируем из Title; если введён — нормализуем
        var slug = string.IsNullOrWhiteSpace(vm.Slug)
            ? await _slugs.GenerateUniqueProjectSlugAsync(vm.Title, null, ct)
            : vm.Slug.Trim().ToLowerInvariant();

        // Если slug введён вручную — проверить уникальность
        if (!string.IsNullOrWhiteSpace(vm.Slug) &&
            await _db.Projects.AnyAsync(p => p.Slug == slug, ct)) {
            ModelState.AddModelError(nameof(vm.Slug), "Такой slug уже занят");
            vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);
            return View(vm);
        }

        var project = new Project {
            Title = vm.Title.Trim(),
            Slug = slug,
            ShortDescription = vm.ShortDescription?.Trim(),
            FullDescription = vm.FullDescription?.Trim(),
            Genre = vm.Genre?.Trim(),
            Role = vm.Role?.Trim(),
            TechStack = vm.TechStack?.Trim(),
            ReleaseDate = vm.ReleaseDate,
            IsPublished = vm.IsPublished,
            IsFeatured = vm.IsFeatured,
            SortOrder = vm.SortOrder,
            SteamUrl = vm.SteamUrl,
            ItchIoUrl = vm.ItchIoUrl,
            GooglePlayUrl = vm.GooglePlayUrl,
            GitHubUrl = vm.GitHubUrl,
            DemoUrl = vm.DemoUrl
        };

        // Обложка
        if (vm.CoverImage is not null && vm.CoverImage.Length > 0) {
            try {
                project.CoverImageUrl = await _images.SaveProjectCoverAsync(vm.CoverImage, project.Slug, ct);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Ошибка сохранения обложки при создании проекта");
                ModelState.AddModelError(nameof(vm.CoverImage), "Не удалось обработать изображение");
                vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);
                return View(vm);
            }
        }

        // Теги
        var tagIds = vm.SelectedTagIds.Distinct().ToList();
        if (tagIds.Count > 0) {
            project.Tags = await _db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync(ct);
        }

        _db.Projects.Add(project);
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"Проект «{project.Title}» создан.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit GET ─────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct) {
        var project = await _db.Projects
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (project is null)
            return NotFound();

        var vm = new ProjectEditViewModel {
            Id = project.Id,
            Title = project.Title,
            Slug = project.Slug,
            ShortDescription = project.ShortDescription,
            FullDescription = project.FullDescription,
            Genre = project.Genre,
            Role = project.Role,
            TechStack = project.TechStack,
            ReleaseDate = project.ReleaseDate,
            IsPublished = project.IsPublished,
            IsFeatured = project.IsFeatured,
            SortOrder = project.SortOrder,
            SteamUrl = project.SteamUrl,
            ItchIoUrl = project.ItchIoUrl,
            GooglePlayUrl = project.GooglePlayUrl,
            GitHubUrl = project.GitHubUrl,
            DemoUrl = project.DemoUrl,
            ExistingCoverImageUrl = project.CoverImageUrl,
            SelectedTagIds = project.Tags.Select(t => t.Id).ToList()
        };
        vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);

        ViewData["Title"] = $"Редактирование: {project.Title}";
        return View(vm);
    }

    // ── Edit POST ────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProjectEditViewModel vm, CancellationToken ct) {
        if (id != vm.Id)
            return BadRequest();

        var project = await _db.Projects
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (project is null)
            return NotFound();

        if (!ModelState.IsValid) {
            vm.ExistingCoverImageUrl = project.CoverImageUrl;
            vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);
            return View(vm);
        }

        // Slug: если не введён — оставляем старый; если введён — нормализуем
        var newSlug = string.IsNullOrWhiteSpace(vm.Slug)
            ? project.Slug
            : vm.Slug.Trim().ToLowerInvariant();

        // Уникальность slug (исключая текущий)
        if (newSlug != project.Slug &&
            await _db.Projects.AnyAsync(p => p.Slug == newSlug && p.Id != id, ct)) {
            ModelState.AddModelError(nameof(vm.Slug), "Такой slug уже занят");
            vm.ExistingCoverImageUrl = project.CoverImageUrl;
            vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);
            return View(vm);
        }

        var oldSlug = project.Slug;

        project.Title = vm.Title.Trim();
        project.Slug = newSlug;
        project.ShortDescription = vm.ShortDescription?.Trim();
        project.FullDescription = vm.FullDescription?.Trim();
        project.Genre = vm.Genre?.Trim();
        project.Role = vm.Role?.Trim();
        project.TechStack = vm.TechStack?.Trim();
        project.ReleaseDate = vm.ReleaseDate;
        project.IsPublished = vm.IsPublished;
        project.IsFeatured = vm.IsFeatured;
        project.SortOrder = vm.SortOrder;
        project.SteamUrl = vm.SteamUrl;
        project.ItchIoUrl = vm.ItchIoUrl;
        project.GooglePlayUrl = vm.GooglePlayUrl;
        project.GitHubUrl = vm.GitHubUrl;
        project.DemoUrl = vm.DemoUrl;
        project.UpdatedAt = DateTime.UtcNow;

        // Обложка: если загружена новая — сохранить, удалить старую
        if (vm.CoverImage is not null && vm.CoverImage.Length > 0) {
            try {
                if (!string.IsNullOrEmpty(project.CoverImageUrl)) {
                    // Удаляем старую обложку (по старому slug)
                    await _images.DeleteProjectCoverAsync(oldSlug, ct);
                }

                project.CoverImageUrl = await _images.SaveProjectCoverAsync(vm.CoverImage, newSlug, ct);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Ошибка сохранения обложки при редактировании");
                ModelState.AddModelError(nameof(vm.CoverImage), "Не удалось обработать изображение");
                vm.ExistingCoverImageUrl = project.CoverImageUrl;
                vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);
                return View(vm);
            }
        }

        // Slug изменился, но обложка старая — НЕ трогаем файлы.
        // Путь в БД остаётся валидным, файл на диске лежит под старым slug.
        // Это компромисс: файл не переименован, но ссылка рабочая.
        // TODO: если понадобится чистота — добавить RenameProjectCoverAsync в IImageService.

        // Теги
        project.Tags.Clear();
        var tagIds = vm.SelectedTagIds.Distinct().ToList();
        if (tagIds.Count > 0) {
            var tags = await _db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync(ct);
            foreach (var t in tags)
                project.Tags.Add(t);
        }

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"Проект «{project.Title}» обновлён.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete GET (подтверждение) ───────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) {
        var project = await _db.Projects
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (project is null)
            return NotFound();

        ViewData["Title"] = "Удаление проекта";
        return View(project);
    }

    // ── Delete POST ──────────────────────────────────────────────────
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct) {
        var project = await _db.Projects.FindAsync(new object[] { id }, ct);
        if (project is null)
            return NotFound();

        // Удаляем файлы обложки
        if (!string.IsNullOrEmpty(project.CoverImageUrl)) {
            try { await _images.DeleteProjectCoverAsync(project.Slug, ct); }
            catch (Exception ex) { _logger.LogWarning(ex, "Не удалось удалить файлы обложки"); }
        }

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"Проект «{project.Title}» удалён.";
        return RedirectToAction(nameof(Index));
    }

    // ── Helper: список тегов с отметками ─────────────────────────────
    private async Task<List<TagOption>> LoadTagOptionsAsync(List<int>? selectedIds, CancellationToken ct) {
        var selected = selectedIds?.ToHashSet() ?? new HashSet<int>();

        return await _db.Tags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new TagOption {
                Id = t.Id,
                Name = t.Name,
                Selected = selected.Contains(t.Id)
            })
            .ToListAsync(ct);
    }
}