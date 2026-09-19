using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Services.Abstractions;

/// <summary>
/// Работа с навыками: топ для главной, группировка по категориям.
/// </summary>
public interface ISkillService {
    /// <summary>
    /// Топ навыков для главной: IsFeatured=true, сортировка по SortOrder.
    /// Максимум <paramref name="count"/> штук.
    /// </summary>
    Task<List<Skill>> GetTopAsync(int count = 8, CancellationToken ct = default);
}