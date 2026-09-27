using Portfolio_game_dev.Services.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Portfolio_game_dev.Services.Implementations;

public class ImageService : IImageService {
    private const int ThumbWidth = 400;
    private const int FullWidth = 1200;
    private const int WebpQuality = 85;

    private readonly IWebHostEnvironment _env;

    public ImageService(IWebHostEnvironment env) {
        _env = env;
    }

    // ── Project covers ───────────────────────────────────────────────
    public Task<string> SaveProjectCoverAsync(IFormFile file, string slug, CancellationToken ct = default)
        => SaveCoverAsync(file, slug, "projects", ct);

    public Task DeleteProjectCoverAsync(string slug, CancellationToken ct = default)
        => DeleteCoverAsync(slug, "projects");

    // ── Blog post covers ─────────────────────────────────────────────
    public Task<string> SavePostCoverAsync(IFormFile file, string slug, CancellationToken ct = default)
        => SaveCoverAsync(file, slug, "blog", ct);

    public Task DeletePostCoverAsync(string slug, CancellationToken ct = default)
        => DeleteCoverAsync(slug, "blog");

    // ── Общая логика ─────────────────────────────────────────────────
    private async Task<string> SaveCoverAsync(IFormFile file, string slug, string subfolder, CancellationToken ct) {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Файл не передан", nameof(file));

        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", subfolder);
        Directory.CreateDirectory(uploadsDir);

        var safeSlug = SanitizeSlug(slug);

        using var stream = file.OpenReadStream();
        using var image = await Image.LoadAsync(stream, ct);

        // thumb
        using (var thumb = image.Clone(ctx => ctx.Resize(new ResizeOptions {
            Size = new Size(ThumbWidth, 0),
            Mode = ResizeMode.Max
        }))) {
            var thumbPath = Path.Combine(uploadsDir, $"{safeSlug}-thumb.webp");
            await thumb.SaveAsync(thumbPath, new WebpEncoder { Quality = WebpQuality }, ct);
        }

        // full
        using (var full = image.Clone(ctx => ctx.Resize(new ResizeOptions {
            Size = new Size(FullWidth, 0),
            Mode = ResizeMode.Max
        }))) {
            var fullPath = Path.Combine(uploadsDir, $"{safeSlug}-full.webp");
            await full.SaveAsync(fullPath, new WebpEncoder { Quality = WebpQuality }, ct);
        }

        return $"/uploads/{subfolder}/{safeSlug}-full.webp";
    }

    private Task DeleteCoverAsync(string slug, string subfolder) {
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", subfolder);
        var safeSlug = SanitizeSlug(slug);

        foreach (var suffix in new[] { "-thumb.webp", "-full.webp" }) {
            var path = Path.Combine(uploadsDir, safeSlug + suffix);
            if (File.Exists(path))
                File.Delete(path);
        }

        return Task.CompletedTask;
    }

    private static string SanitizeSlug(string slug) {
        if (string.IsNullOrWhiteSpace(slug))
            return "untitled";
        var chars = slug.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').ToArray();
        return new string(chars).ToLowerInvariant();
    }
}