using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.Models;

/// <summary>
/// Статья блога. Публичная часть — /Blog и /Blog/{slug}.
/// </summary>
public class BlogPost : BaseEntity {
    [Required, MaxLength(200)]
    /// <summary>Заголовок статьи.</summary>
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    /// <summary>URL-идентификатор. Уникален.</summary>
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    /// <summary>Краткое описание (для превью в списке и meta description).</summary>
    public string? Summary { get; set; }

    /// <summary>Полный текст статьи в Markdown. Рендерится через Markdig.</summary>
    public string? Content { get; set; }

    /// <summary>Устаревшее поле. Оставлено для совместимости; используй Content.</summary>
    public string? ContentMd { get; set; }

    [MaxLength(300)]
    /// <summary>Путь к обложке.</summary>
    public string? CoverImageUrl { get; set; }

    /// <summary>Примерное время чтения в минутах. Можно считать вручную или в сервисе.</summary>
    public int ReadTimeMinutes { get; set; }

    /// <summary>Дата публикации. null — черновик.</summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>Опубликована ли статья. false — не видна на /Blog.</summary>
    public bool IsPublished { get; set; }

    /// <summary>Теги статьи. M2M через join-таблицу BlogPostTags.</summary>
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}