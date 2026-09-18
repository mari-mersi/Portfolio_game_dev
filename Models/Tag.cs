using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.Models;

/// <summary>
/// Тег для проектов и статей блога. M2M с обеими сущностями.
/// </summary>
public class Tag : BaseEntity {
    [Required, MaxLength(50)]
    /// <summary>Отображаемое имя тега: "Unity", "C#", "Unreal Engine".</summary>
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    /// <summary>URL-идентификатор. Уникален. Используется в /Projects?tag=unity.</summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>Обратная сторона M2M с Project. Настраивается в AppDbContext.</summary>
    public ICollection<Project> Projects { get; set; } = new List<Project>();

    /// <summary>Обратная сторона M2M с BlogPost. Настраивается в AppDbContext.</summary>
    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}