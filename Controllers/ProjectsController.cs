using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Controllers;

/// <summary>
/// Портфолио — список и детали.
/// TODO: заменить заглушки на IProjectService / AppDbContext.
/// </summary>
public class ProjectsController : Controller {
    // Временно — пустой список, чтобы контроллер компилировался.
    // Когда подключишь БД, замени на _db.Projects.Include(...) или IProjectService.
    private static List<Project> GetProjects() => new();

    public IActionResult Index(string? tag, int page = 1) {
        const int pageSize = 6;

        var allProjects = GetProjects();

        var allTags = allProjects
            .SelectMany(p => p.Tags)
            .Select(t => t.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t)
            .ToList();

        var filtered = string.IsNullOrWhiteSpace(tag)
            ? allProjects
            : allProjects
                .Where(p => p.Tags.Any(t =>
                    string.Equals(t.Name, tag, StringComparison.OrdinalIgnoreCase)))
                .ToList();

        var totalProjects = filtered.Count;
        var pagedProjects = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var viewModel = new ProjectsIndexViewModel {
            Projects = pagedProjects,
            AllTags = allTags,
            SelectedTag = tag,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalProjects / (double)pageSize)
        };

        return View(viewModel);
    }

    [HttpGet("projects/{id:int}")]
    public IActionResult Details(int id) {
        var projects = GetProjects();
        var project = projects.FirstOrDefault(p => p.Id == id);
        if (project == null)
            return NotFound();

        var relatedProjects = projects
            .Where(p => p.Id != project.Id &&
                        p.Tags.Any(t => project.Tags.Any(pt => pt.Id == t.Id)))
            .Take(3)
            .ToList();

        var viewModel = new ProjectDetailsViewModel {
            Project = project,
            RelatedProjects = relatedProjects
        };

        return View(viewModel);
    }

    [HttpGet("projects/{slug}")]
    public IActionResult DetailsBySlug(string slug) {
        var projects = GetProjects();
        var project = projects.FirstOrDefault(p =>
            string.Equals(p.Slug, slug, StringComparison.OrdinalIgnoreCase));
        if (project == null)
            return NotFound();

        var relatedProjects = projects
            .Where(p => p.Id != project.Id &&
                        p.Tags.Any(t => project.Tags.Any(pt => pt.Id == t.Id)))
            .Take(3)
            .ToList();

        var viewModel = new ProjectDetailsViewModel {
            Project = project,
            RelatedProjects = relatedProjects
        };

        return View("Details", viewModel);
    }

    public IActionResult ByTag(string tag) =>
        RedirectToAction(nameof(Index), new { tag });
}