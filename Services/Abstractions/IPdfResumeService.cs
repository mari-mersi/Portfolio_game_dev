using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Services.Abstractions;

/// <summary>
/// Генерация PDF-резюме из данных БД.
/// </summary>
public interface IPdfResumeService {
    /// <summary>
    /// Собрать PDF-резюме и вернуть байты. Данные берутся из БД:
    /// Experience (свежие сверху), Skill (все), Project (featured).
    /// </summary>
    Task<byte[]> GeneratePdfAsync(CancellationToken ct = default);

    /// <summary>
    /// Собрать модель резюме для веб-превью (без генерации PDF).
    /// </summary>
    Task<ResumeViewModel> BuildViewModelAsync(CancellationToken ct = default);
}