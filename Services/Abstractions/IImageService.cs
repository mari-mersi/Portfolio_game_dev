namespace Portfolio_game_dev.Services.Abstractions;

public interface IImageService {
    /// <summary>Сохранить обложку проекта (thumb 400 + full 1200).</summary>
    Task<string> SaveProjectCoverAsync(IFormFile file, string slug, CancellationToken ct = default);
    Task DeleteProjectCoverAsync(string slug, CancellationToken ct = default);

    /// <summary>Сохранить обложку статьи (thumb 400 + full 1200).</summary>
    Task<string> SavePostCoverAsync(IFormFile file, string slug, CancellationToken ct = default);
    Task DeletePostCoverAsync(string slug, CancellationToken ct = default);
}