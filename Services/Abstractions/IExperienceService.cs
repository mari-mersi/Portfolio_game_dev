using Portfolio_game_dev.Models;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Services.Abstractions;

/// <summary>
/// Работа с опытом работы: таймлайн, группировка по годам.
/// </summary>
public interface IExperienceService {
    /// <summary>
    /// Все места работы, отсортированные по StartDate desc (свежие сверху).
    /// </summary>
    Task<List<Experience>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// ViewModel для страницы /Experience — с группировкой по годам,
    /// подсчётом стажа и уникальных компаний.
    /// </summary>
    Task<ExperienceIndexViewModel> GetTimelineAsync(CancellationToken ct = default);
}