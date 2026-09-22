using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Services.Abstractions;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Controllers;

/// <summary>
/// Публичный блог: список статей и детальная страница.
/// </summary>
public class BlogController : Controller {
    private readonly IBlogService _blog;

    public BlogController(IBlogService blog) {
        _blog = blog;
    }

    /// <summary>
    /// GET: /Blog
    /// GET: /Blog?page=2
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default) {
        var paged = await _blog.GetPagedAsync(page, pageSize: 6, ct);

        var vm = new BlogIndexViewModel {
            Paged = paged
        };

        ViewData["Title"] = "Блог";
        ViewData["Description"] = "Заметки о геймдизайне, разработке и технических экспериментах.";
        return View(vm);
    }

    /// <summary>
    /// GET: /Blog/{slug}
    /// </summary>
    [HttpGet("blog/{slug}")]
    public async Task<IActionResult> Post(string slug, CancellationToken ct = default) {
        var vm = await _blog.GetBySlugAsync(slug, ct);
        if (vm is null)
            return NotFound();

        ViewData["Title"] = vm.Post.Title;
        ViewData["Description"] = vm.Post.Summary ?? vm.Post.Title;
        return View(vm);
    }
}