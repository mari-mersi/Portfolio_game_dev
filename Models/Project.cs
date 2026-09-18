using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio_game_dev.Models;

/// <summary>
/// Игровой проект в портфолио. Центральная сущность публичной части.
/// </summary>
public class Project : BaseEntity {
    [Required, MaxLength(200)]
    /// <summary>Название проекта (отображается в карточке и на детальной странице).</summary>
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    /// <summary>URL-идентификатор. Уникален. Используется в /Projects/{slug}.</summary>
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    /// <summary>Короткое описание для карточки (1–2 предложения).</summary>
    public string? ShortDescription { get; set; }

    /// <summary>Полное описание проекта в Markdown. Рендерится через Markdig на детальной странице.</summary>
    public string? FullDescription { get; set; }

    /// <summary>Устаревшее поле. Оставлено для совместимости; используй FullDescription.</summary>
    public string? DescriptionMd { get; set; }

    [MaxLength(300)]
    /// <summary>Путь к обложке (thumb или full). Относительный URL, например /uploads/projects/foo-full.webp.</summary>
    public string? CoverImageUrl { get; set; }

    [MaxLength(100)]
    /// <summary>Жанр: "Action / Platformer", "Tactical RPG" и т.п.</summary>
    public string? Genre { get; set; }

    [MaxLength(100)]
    /// <summary>Роль в проекте: "Lead Game Developer", "Solo Developer".</summary>
    public string? Role { get; set; }

    [MaxLength(300)]
    /// <summary>Стек строкой через запятую: "Unity, C#, Shader Graph". Разбивается через TechList.</summary>
    public string? TechStack { get; set; }

    [MaxLength(300)]
    /// <summary>Ссылка на страницу в Steam (опционально).</summary>
    public string? SteamUrl { get; set; }

    [MaxLength(300)]
    /// <summary>Ссылка на страницу в itch.io (опционально).</summary>
    public string? ItchIoUrl { get; set; }

    [MaxLength(300)]
    /// <summary>Ссылка на Google Play (опционально).</summary>
    public string? GooglePlayUrl { get; set; }

    [MaxLength(300)]
    /// <summary>Ссылка на GitHub (опционально).</summary>
    public string? GitHubUrl { get; set; }

    [MaxLength(300)]
    /// <summary>Общая ссылка на репозиторий (если не GitHub).</summary>
    public string? RepoUrl { get; set; }

    [MaxLength(300)]
    /// <summary>Ссылка на демо/билд (WebGL, itch и т.п.).</summary>
    public string? DemoUrl { get; set; }

    /// <summary>Дата релиза (опционально). null — проект в разработке.</summary>
    public DateTime? ReleaseDate { get; set; }

    /// <summary>Показывать на главной в блоке «Избранное».</summary>
    public bool IsFeatured { get; set; }

    /// <summary>Опубликован ли проект. false — черновик, не виден публично.</summary>
    public bool IsPublished { get; set; } = true;

    /// <summary>Ручная сортировка (меньше — выше). Используется в списках.</summary>
    public int SortOrder { get; set; }

    /// <summary>Медиа проекта (скриншоты, видео). 1:M, каскадное удаление при удалении проекта.</summary>
    public ICollection<ProjectMedia> Media { get; set; } = new List<ProjectMedia>();

    /// <summary>Теги проекта. M2M через join-таблицу ProjectTags.</summary>
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();

    /// <summary>TechStack, разбитый на элементы. Не мапится в БД (NotMapped).</summary>
    [NotMapped]
    public IEnumerable<string> TechList =>
        string.IsNullOrWhiteSpace(TechStack)
            ? Enumerable.Empty<string>()
            : TechStack.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}