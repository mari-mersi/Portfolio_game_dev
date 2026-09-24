using Portfolio_game_dev.Services.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Portfolio_game_dev.Services.Implementations;

/// <summary>
/// Обработка изображений через ImageSharp.
/// Сохраняет в wwwroot/uploads/projects/{slug}-{thumb|full}.webp
/// </summary>
public class ImageService : IImageService {
    private const int ThumbWidth = 400;
    private const int FullWidth = 1200;
    private const int WebpQuality = 85;

    private readonly IWebHostEnvironment _env;

    public ImageService(IWebHostEnvironment env) {
        _env = env;
    }

    public async Task<string> SaveProjectCoverAsync(IFormFile file, string slug, CancellationToken ct = default) {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Файл не передан", nameof(file));

        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "projects");
        Directory.CreateDirectory(uploadsDir);

        var safeSlug = SanitizeSlug(slug);

        // Загружаем оригинал в память
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

        // Возвращаем относительный путь для БД
        return $"/uploads/projects/{safeSlug}-full.webp";
    }

    public Task DeleteProjectCoverAsync(string slug, CancellationToken ct = default) {
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "projects");
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