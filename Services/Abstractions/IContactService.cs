using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Services.Abstractions;

/// <summary>
/// Сохранение сообщений из формы обратной связи.
/// </summary>
public interface IContactService {
    /// <summary>
    /// Сохранить сообщение, вычислив хэш IP для антиспам-аналитики.
    /// </summary>
    /// <param name="vm">Данные формы.</param>
    /// <param name="ipHash">SHA-256 от IP (с солью), необязательно.</param>
    Task SaveAsync(ContactViewModel vm, string? ipHash, CancellationToken ct = default);
}