using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Areas.Admin.ViewModels;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services.Abstractions;

namespace Portfolio_game_dev.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BlogController : Controller {
    private readonly AppDbContext _db;
    private readonly IImageService _images;
    private readonly ISlugService _slugs;
    private readonly IOutputCacheStore _cache;
    private readonly ILogger<BlogController> _logger;

    public BlogController(
        AppDbContext db,
        IImageService images,
        ISlugService slugs,
        IOutputCacheStore cache,
        ILogger<BlogController> logger) {
        _db = db;
        _images = images;
        _slugs = slugs;
        _cache = cache;
        _logger = logger;
    }

    // ── Index ────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? status, CancellationToken ct) {
        var query = _db.BlogPosts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search)) {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(p =>
                p.Title.ToLower().Contains(s) ||
                p.Slug.ToLower().Contains(s));
        }

        if (status == "published")
            query = query.Where(p => p.IsPublished);
        else if (status == "draft")
            query = query.Where(p => !p.IsPublished);

        var posts = await query
            .Include(p => p.Tags)
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .ToListAsync(ct);

        var vm = new BlogPostsListViewModel {
            Posts = posts,
            SearchTerm = search,
            StatusFilter = status
        };

        ViewData["Title"] = "Статьи";
        return View(vm);
    }

    // ── Create GET ───────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct) {
        var vm = new BlogPostEditViewModel {
            IsPublished = true,
            ReadTimeMinutes = 5,
            AvailableTags = await LoadTagOptionsAsync(null, ct)
        };

        ViewData["Title"] = "Новая статья";
        return View(vm);
    }

    // ── Create POST ──────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogPostEditViewModel vm, CancellationToken ct) {
        if (!ModelState.IsValid) {
            vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);
            return View(vm);
        }

        var slug = string.IsNullOrWhiteSpace(vm.Slug)
            ? await _slugs.GenerateUniqueProjectSlugAsync(vm.Title, null, ct)
            : vm.Slug.Trim().ToLowerInvariant();

        // Проверка уникальности slug среди постов
        if (await _db.BlogPosts.AnyAsync(p => p.Slug == slug, ct)) {
            ModelState.AddModelError(nameof(vm.Slug), "Такой slug уже занят");
            vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);
            return View(vm);
        }

        var post = new BlogPost {
            Title = vm.Title.Trim(),
            Slug = slug,
            Summary = vm.Summary?.Trim(),
            Content = vm.Content?.Trim(),
            PublishedAt = vm.IsPublished ? (vm.PublishedAt ?? DateTime.UtcNow) : vm.PublishedAt,
            IsPublished = vm.IsPublished,
            ReadTimeMinutes = vm.ReadTimeMinutes
        };

        // Обложка
        if (vm.CoverImage is not null && vm.CoverImage.Length > 0) {
            try {
                post.CoverImageUrl = await _images.SavePostCoverAsync(vm.CoverImage, post.Slug, ct);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Ошибка сохранения обложки при создании статьи");
                ModelState.AddModelError(nameof(vm.CoverImage), "Не удалось обработать изображение");
                vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);
                return View(vm);
            }
        }

        // Теги
        var tagIds = vm.SelectedTagIds.Distinct().ToList();
        if (tagIds.Count > 0)
            post.Tags = await _db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync(ct);

        _db.BlogPosts.Add(post);
        await _db.SaveChangesAsync(ct);

        // Сброс кэша
        await _cache.EvictByTagAsync("public", ct);

        TempData["Success"] = $"Статья «{post.Title}» создана.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit GET ─────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct) {
        var post = await _db.BlogPosts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (post is null)
            return NotFound();

        var vm = new BlogPostEditViewModel {
            Id = post.Id,
            Title = post.Title,
            Slug = post.Slug,
            Summary = post.Summary,
            Content = post.Content,
            PublishedAt = post.PublishedAt,
            IsPublished = post.IsPublished,
            ReadTimeMinutes = post.ReadTimeMinutes,
            ExistingCoverImageUrl = post.CoverImageUrl,
            SelectedTagIds = post.Tags.Select(t => t.Id).ToList()
        };
        vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);

        ViewData["Title"] = $"Редактирование: {post.Title}";
        return View(vm);
    }

    // ── Edit POST ────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BlogPostEditViewModel vm, CancellationToken ct) {
        if (id != vm.Id)
            return BadRequest();

        var post = await _db.BlogPosts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (post is null)
            return NotFound();

        if (!ModelState.IsValid) {
            vm.ExistingCoverImageUrl = post.CoverImageUrl;
            vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);
            return View(vm);
        }

        var newSlug = string.IsNullOrWhiteSpace(vm.Slug)
            ? post.Slug
            : vm.Slug.Trim().ToLowerInvariant();

        if (newSlug != post.Slug &&
            await _db.BlogPosts.AnyAsync(p => p.Slug == newSlug && p.Id != id, ct)) {
            ModelState.AddModelError(nameof(vm.Slug), "Такой slug уже занят");
            vm.ExistingCoverImageUrl = post.CoverImageUrl;
            vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);
            return View(vm);
        }

        var oldSlug = post.Slug;

        post.Title = vm.Title.Trim();
        post.Slug = newSlug;
        post.Summary = vm.Summary?.Trim();
        post.Content = vm.Content?.Trim();
        post.IsPublished = vm.IsPublished;
        post.ReadTimeMinutes = vm.ReadTimeMinutes;

        // PublishedAt: при первой публикации — текущая дата
        if (vm.IsPublished && !post.PublishedAt.HasValue)
            post.PublishedAt = vm.PublishedAt ?? DateTime.UtcNow;
        else
            post.PublishedAt = vm.PublishedAt;

        post.UpdatedAt = DateTime.UtcNow;

        // Обложка
        if (vm.CoverImage is not null && vm.CoverImage.Length > 0) {
            try {
                if (!string.IsNullOrEmpty(post.CoverImageUrl))
                    await _images.DeletePostCoverAsync(oldSlug, ct);

                post.CoverImageUrl = await _images.SavePostCoverAsync(vm.CoverImage, newSlug, ct);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Ошибка сохранения обложки при редактировании");
                ModelState.AddModelError(nameof(vm.CoverImage), "Не удалось обработать изображение");
                vm.ExistingCoverImageUrl = post.CoverImageUrl;
                vm.AvailableTags = await LoadTagOptionsAsync(vm.SelectedTagIds, ct);
                return View(vm);
            }
        }

        // Теги
        post.Tags.Clear();
        var tagIds = vm.SelectedTagIds.Distinct().ToList();
        if (tagIds.Count > 0) {
            var tags = await _db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync(ct);
            foreach (var t in tags)
                post.Tags.Add(t);
        }

        await _db.SaveChangesAsync(ct);

        await _cache.EvictByTagAsync("public", ct);

        TempData["Success"] = $"Статья «{post.Title}» обновлена.";
        return RedirectToAction(nameof(Index));
    }

    // ── TogglePublish ────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePublish(int id, CancellationToken ct) {
        var post = await _db.BlogPosts.FindAsync(new object[] { id }, ct);
        if (post is null)
            return NotFound();

        post.IsPublished = !post.IsPublished;

        if (post.IsPublished && !post.PublishedAt.HasValue)
            post.PublishedAt = DateTime.UtcNow;

        post.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        await _cache.EvictByTagAsync("public", ct);

        TempData["Success"] = post.IsPublished
            ? $"Статья «{post.Title}» опубликована."
            : $"Статья «{post.Title}» снята с публикации.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete GET ───────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) {
        var post = await _db.BlogPosts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (post is null)
            return NotFound();

        ViewData["Title"] = "Удаление статьи";
        return View(post);
    }

    // ── Delete POST ──────────────────────────────────────────────────
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct) {
        var post = await _db.BlogPosts.FindAsync(new object[] { id }, ct);
        if (post is null)
            return NotFound();

        if (!string.IsNullOrEmpty(post.CoverImageUrl)) {
            try { await _images.DeletePostCoverAsync(post.Slug, ct); }
            catch (Exception ex) { _logger.LogWarning(ex, "Не удалось удалить обложку"); }
        }

        _db.BlogPosts.Remove(post);
        await _db.SaveChangesAsync(ct);

        await _cache.EvictByTagAsync("public", ct);

        TempData["Success"] = $"Статья «{post.Title}» удалена.";
        return RedirectToAction(nameof(Index));
    }

    // ── Helper ───────────────────────────────────────────────────────
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