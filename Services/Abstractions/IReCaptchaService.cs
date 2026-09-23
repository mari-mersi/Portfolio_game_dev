namespace Portfolio_game_dev.Services.Abstractions;

/// <summary>
/// Проверка токена Google reCAPTCHA v3.
/// </summary>
public interface IReCaptchaService {
    /// <summary>
    /// Проверить токен. Возвращает true, если:
    /// - валидация отключена в настройках (ReCaptcha:Enabled = false), ИЛИ
    /// - токен валиден и score >= 0.5.
    /// </summary>
    Task<bool> ValidateAsync(string? token, CancellationToken ct = default);
}