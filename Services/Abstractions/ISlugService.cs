namespace Portfolio_game_dev.Services.Abstractions;

/// <summary>
/// Генерация URL-friendly slug из строки с проверкой уникальности.
/// </summary>
public interface ISlugService {
    /// <summary>
    /// Транслитерация + нормализация. Без проверки уникальности.
    /// </summary>
    string Slugify(string input);

    /// <summary>
    /// Уникальный slug для проекта: если занят, добавляет -2, -3 и т.д.
    /// </summary>
    Task<string> GenerateUniqueProjectSlugAsync(string title, int? excludeId = null, CancellationToken ct = default);
}