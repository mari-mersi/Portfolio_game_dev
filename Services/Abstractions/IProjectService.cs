using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Services.Abstractions;

/// <summary>
/// Работа с проектами: списки, детали, фильтры.
/// На Дне 2 нужен только GetFeaturedAsync — остальное добавим в День 3.
/// </summary>
public interface IProjectService {
    /// <summary>
    /// Избранные проекты для главной: IsPublished=true, IsFeatured=true,
    /// отсортированы по SortOrder. Максимум <paramref name="count"/> штук.
    /// </summary>
    Task<List<Project>> GetFeaturedAsync(int count = 3, CancellationToken ct = default);
}