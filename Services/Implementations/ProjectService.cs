using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services.Abstractions;

namespace Portfolio_game_dev.Services.Implementations;

/// <summary>
/// Реализация IProjectService поверх EF Core.
/// </summary>
public class ProjectService : IProjectService {
    private readonly AppDbContext _db;

    public ProjectService(AppDbContext db) {
        _db = db;
    }

    public async Task<List<Project>> GetFeaturedAsync(int count = 3, CancellationToken ct = default) {
        return await _db.Projects
            .Where(p => p.IsPublished && p.IsFeatured)
            .OrderBy(p => p.SortOrder)
            .ThenByDescending(p => p.ReleaseDate)   // при равном SortOrder — свежие выше
            .Take(count)
            .ToListAsync(ct);
    }
}