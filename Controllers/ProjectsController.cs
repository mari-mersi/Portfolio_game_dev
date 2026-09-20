using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Services.Abstractions;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Controllers;

/// <summary>
/// Портфолио: список опубликованных проектов, детальная страница, фильтр по тегу.
/// </summary>
public class ProjectsController : Controller {
    private readonly IProjectService _projects;

    public ProjectsController(IProjectService projects) {
        _projects = projects;
    }

    /// <summary>
    /// GET: /Projects
    /// GET: /Projects?tag=unity
    /// GET: /Projects?tag=unreal-engine
    /// GET: /Projects?page=1&amp;tag=unity
    /// Список опубликованных проектов с фильтром по тегу и пагинацией.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, string? tag = null, CancellationToken ct = default) {
        // Если пришёл пустой tag (например, ?tag=), приводим к null
        var normalizedTag = string.IsNullOrWhiteSpace(tag) ? null : tag.Trim().ToLowerInvariant();

        var paged = await _projects.GetPagedAsync(page, normalizedTag, pageSize: 6, ct);
        var allTags = await _projects.GetAllTagsAsync(ct);

        var vm = new ProjectsIndexViewModel {
            Paged = paged,
            AllTags = allTags,
            SelectedTag = normalizedTag
        };

        ViewData["Title"] = "Проекты";
        ViewData["Description"] = "Портфолио игровых проектов: Unity, Unreal, Godot.";
        return View(vm);
    }

    /// <summary>
    /// GET: /Projects/{slug}
    /// GET: /Projects/cyber-odyssey
    /// Детальная страница проекта. 404, если slug не найден или проект не опубликован.
    /// </summary>
    [HttpGet("projects/{slug}")]
    public async Task<IActionResult> DetailsBySlug(string slug, CancellationToken ct = default) {
        var vm = await _projects.GetBySlugAsync(slug, ct);
        if (vm is null)
            return NotFound();

        ViewData["Title"] = vm.Project.Title;
        ViewData["Description"] = vm.Project.ShortDescription ?? vm.Project.Title;
        return View("Details", vm);
    }

    /// <summary>
    /// GET: /Projects/ByTag/{tag}
    /// Редирект на /Projects?tag={tag} — чтобы фильтр был в query string,
    /// а не в пути (удобнее для кэша и SEO).
    /// </summary>
    [HttpGet("projects/tag/{tag}")]
    public IActionResult ByTag(string tag) {
        if (string.IsNullOrWhiteSpace(tag))
            return RedirectToAction(nameof(Index));

        return RedirectToAction(nameof(Index), new { tag = tag.Trim().ToLowerInvariant() });
    }
}