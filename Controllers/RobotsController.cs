using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Portfolio_game_dev.Controllers;

public class RobotsController : Controller {
    [HttpGet("/robots.txt")]
    [OutputCache(PolicyName = "public")]
    public IActionResult Index() {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var sb = new StringBuilder();
        sb.AppendLine("User-agent: *");
        sb.AppendLine("Allow: /");
        sb.AppendLine("Disallow: /Admin/");
        sb.AppendLine("Disallow: /error/");
        sb.AppendLine("Disallow: /health");
        sb.AppendLine();
        sb.AppendLine($"Sitemap: {baseUrl}/sitemap.xml");

        return Content(sb.ToString(), "text/plain", Encoding.UTF8);
    }
}