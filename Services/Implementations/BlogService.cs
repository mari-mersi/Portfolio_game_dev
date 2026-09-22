using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services.Abstractions;
using Portfolio_game_dev.ViewModels;
using Portfolio_game_dev.ViewModels.Shared;

namespace Portfolio_game_dev.Services.Implementations;

/// <summary>
/// Реализация IBlogService поверх EF Core.
/// </summary>
public class BlogService : IBlogService {
    private const int DefaultPageSize = 6;

    private readonly AppDbContext _db;

    public BlogService(AppDbContext db) {
        _db = db;
    }

    public async Task<List<BlogPost>> GetLatestAsync(int count = 3, CancellationToken ct = default) {
        return await _db.BlogPosts
            .AsNoTracking()
            .Where(p => p.IsPublished && p.PublishedAt != null)
            .OrderByDescending(p => p.PublishedAt)
            .Take(count)
            .ToListAsync(ct);
    }

    public async Task<PagedResult<BlogPost>> GetPagedAsync(
        int page,
        int pageSize = DefaultPageSize,
        CancellationToken ct = default) {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = DefaultPageSize;

        var query = _db.BlogPosts
            .AsNoTracking()
            .Where(p => p.IsPublished && p.PublishedAt != null);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Include(p => p.Tags)
            .OrderByDescending(p => p.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return PagedResult<BlogPost>.Create(items, page, pageSize, totalCount);
    }

    public async Task<BlogPostViewModel?> GetBySlugAsync(string slug, CancellationToken ct = default) {
        if (string.IsNullOrWhiteSpace(slug))
            return null;

        var normalized = slug.Trim().ToLowerInvariant();

        var post = await _db.BlogPosts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Slug == normalized && p.IsPublished, ct);

        if (post is null)
            return null;

        // Похожие: с тегами текущей статьи, кроме самой статьи
        var tagIds = post.Tags.Select(t => t.Id).ToList();

        var related = await _db.BlogPosts
            .AsNoTracking()
            .Where(p => p.Id != post.Id
                     && p.IsPublished
                     && p.Tags.Any(t => tagIds.Contains(t.Id)))
            .OrderByDescending(p => p.PublishedAt)
            .Take(3)
            .Include(p => p.Tags)
            .ToListAsync(ct);

        // Markdown → HTML
        var html = Portfolio_game_dev.Services.MarkdownRenderer
            .ToHtml(post.Content ?? post.ContentMd ?? string.Empty);

        return new BlogPostViewModel {
            Post = post,
            RenderedContent = html,
            RelatedPosts = related
        };
    }
}