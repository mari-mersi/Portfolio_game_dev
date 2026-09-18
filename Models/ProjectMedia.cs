using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.Models;

/// <summary>Тип медиа. Сохраняется в БД как int (HasConversion в AppDbContext).</summary>
public enum MediaType {
    /// <summary>Изображение (png, jpg, webp).</summary>
    Image = 0,

    /// <summary>Видео (mp4, webm) или ссылка на YouTube.</summary>
    Video = 1
}

/// <summary>
/// Медиа-элемент проекта: скриншот или видео. Дочерняя сущность Project.
/// </summary>
public class ProjectMedia : BaseEntity {
    [Required, MaxLength(300)]
    /// <summary>URL медиа: локальный (/uploads/...) или внешний (https://...).</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>Тип: картинка или видео. По умолчанию — картинка.</summary>
    public MediaType Type { get; set; } = MediaType.Image;

    /// <summary>Порядок отображения в галерее (меньше — раньше).</summary>
    public int SortOrder { get; set; }

    /// <summary>FK на Project. Проставляется EF автоматически.</summary>
    public int ProjectId { get; set; }

    /// <summary>Навигационное свойство на родителя. null! — EF гарантирует, что не null после загрузки.</summary>
    public Project Project { get; set; } = null!;
}