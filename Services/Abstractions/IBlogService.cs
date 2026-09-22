using Portfolio_game_dev.Models;
using Portfolio_game_dev.ViewModels;
using Portfolio_game_dev.ViewModels.Shared;

namespace Portfolio_game_dev.Services.Abstractions;

/// <summary>
/// Работа со статьями блога: последние для главной, пагинация, детали.
/// </summary>
public interface IBlogService {
    /// <summary>
    /// Последние опубликованные статьи для главной. Максимум <paramref name="count"/> штук.
    /// </summary>
    Task<List<BlogPost>> GetLatestAsync(int count = 3, CancellationToken ct = default);

    /// <summary>
    /// Постраничный список опубликованных статей (PublishedAt desc).
    /// </summary>
    Task<PagedResult<BlogPost>> GetPagedAsync(int page, int pageSize = 6, CancellationToken ct = default);

    /// <summary>
    /// Статья по slug с тегами + до 3 похожих по общим тегам.
    /// Возвращает null, если статьи нет или она не опубликована.
    /// </summary>
    Task<BlogPostViewModel?> GetBySlugAsync(string slug, CancellationToken ct = default);
}