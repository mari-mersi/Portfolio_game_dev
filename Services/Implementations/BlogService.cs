using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services.Abstractions;

namespace Portfolio_game_dev.Services.Implementations;

/// <summary>
/// Реализация IBlogService поверх EF Core.
/// </summary>
public class BlogService : IBlogService {
    private readonly AppDbContext _db;

    public BlogService(AppDbContext db) {
        _db = db;
    }

    public async Task<List<BlogPost>> GetLatestAsync(int count = 3, CancellationToken ct = default) {
        return await _db.BlogPosts
            .Where(p => p.IsPublished && p.PublishedAt != null)
            .OrderByDescending(p => p.PublishedAt)
            .Take(count)
            .ToListAsync(ct);
    }
}