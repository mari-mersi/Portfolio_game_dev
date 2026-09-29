using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Portfolio_game_dev.Services.Abstractions;

namespace Portfolio_game_dev.Controllers;

public class SitemapController : Controller {
    private readonly IProjectService _projects;
    private readonly IBlogService _blog;

    public SitemapController(IProjectService projects, IBlogService blog) {
        _projects = projects;
        _blog = blog;
    }

    [HttpGet("/sitemap.xml")]
    [OutputCache(PolicyName = "public")]
    public async Task<IActionResult> Index(CancellationToken ct) {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var urls = new List<(string loc, DateTime? lastmod, string priority)>
        {
            ($"{baseUrl}/", null, "1.0"),
            ($"{baseUrl}/Projects", null, "0.9"),
            ($"{baseUrl}/Skills", null, "0.8"),
            ($"{baseUrl}/Experience", null, "0.8"),
            ($"{baseUrl}/Blog", null, "0.9"),
            ($"{baseUrl}/Resume", null, "0.7"),
            ($"{baseUrl}/Home/Contact", null, "0.6"),
        };

        // Публичные проекты
        var projects = await _projects.GetPagedAsync(1, null, pageSize: 500, ct);
        foreach (var p in projects.Items) {
            var lastmod = p.UpdatedAt ?? p.CreatedAt;
            // Защита от default(DateTime) — иначе в XML попадёт 0001-01-01
            if (lastmod == default)
                lastmod = DateTime.UtcNow;
            urls.Add(($"{baseUrl}/Projects/{p.Slug}", lastmod, "0.7"));
        }

        // Опубликованные статьи
        var posts = await _blog.GetPagedAsync(1, pageSize: 500, ct);
        foreach (var p in posts.Items) {
            var lastmod = p.UpdatedAt ?? p.PublishedAt ?? p.CreatedAt;
            if (lastmod == default)
                lastmod = DateTime.UtcNow;
            urls.Add(($"{baseUrl}/Blog/{p.Slug}", lastmod, "0.7"));
        }

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        // ВАЖНО: порядок элементов — loc, lastmod, priority (по стандарту sitemap.org)
        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),   // ← явно указываем utf-8
            new XElement(ns + "urlset",
                urls.Select(u =>
                    new XElement(ns + "url",
                        new XElement(ns + "loc", u.loc),
                        u.lastmod.HasValue
                            ? new XElement(ns + "lastmod", u.lastmod.Value.ToString("yyyy-MM-dd"))
                            : null,
                        new XElement(ns + "priority", u.priority)))));

        // Сохраняем в MemoryStream, а не в StringWriter —
        // так XDocument корректно запишет utf-8 в декларацию.
        using var ms = new MemoryStream();
        doc.Save(ms, SaveOptions.DisableFormatting);
        var xml = Encoding.UTF8.GetString(ms.ToArray());

        return Content(xml, "application/xml", Encoding.UTF8);
    }
}