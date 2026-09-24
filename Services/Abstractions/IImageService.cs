namespace Portfolio_game_dev.Services.Abstractions;

/// <summary>
/// Обработка изображений: ресайз и сохранение в wwwroot.
/// </summary>
public interface IImageService {
    /// <summary>
    /// Сохранить обложку: ресайзит в thumb (400px) и full (1200px), сохраняет как WebP.
    /// Возвращает относительный путь к full-версии (для хранения в БД).
    /// </summary>
    /// <param name="file">Загруженный файл.</param>
    /// <param name="slug">Slug проекта для имени файла.</param>
    /// <returns>Путь вида /uploads/projects/{slug}-full.webp</returns>
    Task<string> SaveProjectCoverAsync(IFormFile file, string slug, CancellationToken ct = default);

    /// <summary>
    /// Удалить все версии обложки проекта (thumb, full).
    /// </summary>
    Task DeleteProjectCoverAsync(string slug, CancellationToken ct = default);
}