using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services.Abstractions;
using Portfolio_game_dev.ViewModels;
using Portfolio_game_dev.ViewModels.Shared;

namespace Portfolio_game_dev.Services.Implementations;

/// <summary>
/// Реализация IProjectService поверх EF Core.
/// </summary>
public class ProjectService : IProjectService {
    private const int DefaultPageSize = 6;

    private readonly AppDbContext _db;

    public ProjectService(AppDbContext db) {
        _db = db;
    }

    public async Task<List<Project>> GetFeaturedAsync(int count = 3, CancellationToken ct = default) {
        return await _db.Projects
            .Where(p => p.IsPublished && p.IsFeatured)
            .OrderBy(p => p.SortOrder)
            .ThenByDescending(p => p.ReleaseDate)
            .Take(count)
            .ToListAsync(ct);
    }

    public async Task<PagedResult<Project>> GetPagedAsync(
        int page,
        string? tag,
        int pageSize = DefaultPageSize,
        CancellationToken ct = default) {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = DefaultPageSize;

        // Базовый запрос: только опубликованные
        var query = _db.Projects
            .AsNoTracking()
            .Where(p => p.IsPublished);

        // Фильтр по тегу — по slug
        if (!string.IsNullOrWhiteSpace(tag)) {
            var t = tag.Trim().ToLowerInvariant();
            query = query.Where(p => p.Tags.Any(projectTag => projectTag.Slug == t));
        }

        // Сколько всего подходит под фильтр
        var totalCount = await query.CountAsync(ct);

        // Текущая страница
        var items = await query
            .Include(p => p.Tags)                       // теги для карточки
            .OrderBy(p => p.SortOrder)
            .ThenByDescending(p => p.ReleaseDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return PagedResult<Project>.Create(items, page, pageSize, totalCount);
    }

    public async Task<ProjectDetailsViewModel?> GetBySlugAsync(
        string slug,
        CancellationToken ct = default) {
        if (string.IsNullOrWhiteSpace(slug))
            return null;

        var normalized = slug.Trim().ToLowerInvariant();

        var project = await _db.Projects
            .Include(p => p.Media.OrderBy(m => m.SortOrder))
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Slug == normalized && p.IsPublished, ct);

        if (project is null)
            return null;

        // Похожие проекты: те же теги, кроме текущего
        var tagIds = project.Tags.Select(t => t.Id).ToList();

        var related = await _db.Projects
            .AsNoTracking()
            .Where(p => p.Id != project.Id
                     && p.IsPublished
                     && p.Tags.Any(t => tagIds.Contains(t.Id)))
            .OrderByDescending(p => p.ReleaseDate)
            .Take(3)
            .Include(p => p.Tags)
            .ToListAsync(ct);

        return new ProjectDetailsViewModel {
            Project = project,
            RelatedProjects = related
        };
    }

    public async Task<List<Tag>> GetAllTagsAsync(CancellationToken ct = default) {
        return await _db.Tags
            .AsNoTracking()
            .Where(t => t.Projects.Any(p => p.IsPublished))
            .OrderBy(t => t.Name)
            .ToListAsync(ct);
    }
}