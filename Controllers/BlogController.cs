using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Services;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Controllers {
    /// <summary>
    /// Статьи и девлоги.
    /// </summary>
    public class BlogController : Controller {
        private const int PageSize = 6;

        private readonly IBlogService _blog;
        private readonly ILogger<BlogController> _logger;

        public BlogController(IBlogService blog, ILogger<BlogController> logger) {
            _blog = blog;
            _logger = logger;
        }

        // GET: /blog
        // GET: /blog?page=2
        public async Task<IActionResult> Index(int page = 1) {
            if (page < 1)
                page = 1;

            var total = await _blog.GetPublishedCountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));

            if (page > totalPages)
                return RedirectToAction(nameof(Index), new { page = totalPages });

            var posts = await _blog.GetPublishedAsync(page, PageSize);

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            return View(posts);
        }

        // GET: /blog/{slug}
        [Route("blog/{slug}")]
        public async Task<IActionResult> Post(string slug) {
            if (string.IsNullOrWhiteSpace(slug))
                return NotFound();

            var post = await _blog.GetBySlugAsync(slug);
            if (post is null)
                return NotFound();

            var vm = new BlogPostViewModel {
                Post = post,
                RenderedContent = MarkdownRenderer.ToHtml(post.Content),
                RelatedPosts = await _blog.GetRelatedAsync(post, 3)
            };

            return View(vm);
        }
    }
}