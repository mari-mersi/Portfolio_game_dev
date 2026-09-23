using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Portfolio_game_dev.ViewModels;

/// <summary>
/// Форма обратной связи. Data Annotations + IValidatableObject для антиспама.
/// </summary>
public class ContactViewModel : IValidatableObject {
    [Required(ErrorMessage = "Укажите имя")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Имя от 2 до 80 символов")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите e-mail")]
    [EmailAddress(ErrorMessage = "Некорректный e-mail")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите сообщение")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Сообщение от 10 до 2000 символов")]
    public string Message { get; set; } = string.Empty;

    /// <summary>Honeypot-поле. Настоящий пользователь его не видит и не заполняет.</summary>
    public string? Website { get; set; }

    /// <summary>Токен reCAPTCHA v3. Заполняется JS на клиенте.</summary>
    public string? RecaptchaToken { get; set; }

    /// <summary>
    /// Антиспам-эвристика. Не блокирует жёстко, а подсвечивает подозрительные сообщения.
    /// Если найдены признаки спама — считаем форму невалидной (или помечаем в сервисе).
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
        // 1. Honeypot
        if (!string.IsNullOrWhiteSpace(Website)) {
            yield return new ValidationResult(
                "Форма заполнена некорректно.",
                new[] { nameof(Website) });
        }

        // 2. Ссылки в первых 5 символах сообщения
        if (!string.IsNullOrWhiteSpace(Message)) {
            var head = Message.TrimStart().Substring(0, Math.Min(5, Message.TrimStart().Length));
            if (head.StartsWith("http", StringComparison.OrdinalIgnoreCase) ||
                head.Contains("www.", StringComparison.OrdinalIgnoreCase)) {
                yield return new ValidationResult(
                    "Сообщение выглядит как спам. Уберите ссылки в начале.",
                    new[] { nameof(Message) });
            }
        }

        // 3. Спам-ключевые слова
        var spamKeywords = new[]
        {
            "casino", "viagra", "porn", "seo-услуги", "купить ссылки",
            "заработок в интернете", "биткоин-миксер", "t.me/joinchat"
        };

        var lowerMessage = Message?.ToLowerInvariant() ?? "";
        var lowerName = Name?.ToLowerInvariant() ?? "";

        foreach (var keyword in spamKeywords) {
            if (lowerMessage.Contains(keyword) || lowerName.Contains(keyword)) {
                yield return new ValidationResult(
                    "Сообщение содержит запрещённый контент.",
                    new[] { nameof(Message) });
                break;
            }
        }

        // 4. Слишком много ссылок в сообщении (>= 3)
        var linkCount = Regex.Matches(Message ?? "", @"https?://", RegexOptions.IgnoreCase).Count;
        if (linkCount >= 3) {
            yield return new ValidationResult(
                "Слишком много ссылок в сообщении.",
                new[] { nameof(Message) });
        }
    }
}