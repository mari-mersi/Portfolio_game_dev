using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Services.Abstractions;

/// <summary>
/// Работа со статьями блога: последние для главной, пагинация, детали.
/// </summary>
public interface IBlogService {
    /// <summary>
    /// Последние опубликованные статьи (IsPublished=true, PublishedAt != null),
    /// отсортированы по убыванию даты. Максимум <paramref name="count"/> штук.
    /// </summary>
    Task<List<BlogPost>> GetLatestAsync(int count = 3, CancellationToken ct = default);
}