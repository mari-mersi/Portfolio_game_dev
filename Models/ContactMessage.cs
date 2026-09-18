using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.Models;

/// <summary>
/// Сообщение из формы обратной связи. Отправляется с /Home/Contact,
/// просматривается в админке (/Admin/ContactMessages).
/// </summary>
public class ContactMessage : BaseEntity {
    [Required, MaxLength(100)]
    /// <summary>Имя отправителя.</summary>
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(150), EmailAddress]
    /// <summary>Email отправителя. EmailAddress — валидация формата.</summary>
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    /// <summary>Текст сообщения.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Прочитано ли сообщение админом.</summary>
    public bool IsRead { get; set; }

    [MaxLength(64)]
    /// <summary>
    /// SHA-256 от IP (с солью). Сырой IP не храним — GDPR-приличия.
    /// Нужно для антиспам-аналитики: один IP — много сообщений.
    /// </summary>
    public string? IpHash { get; set; }
}