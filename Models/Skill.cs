using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.Models;

/// <summary>
/// Навык (технология, инструмент, софт-скилл). Отображается на /Skills,
/// группируется по Category.
/// </summary>
public class Skill : BaseEntity {
    [Required, MaxLength(100)]
    /// <summary>Название: "C#", "Unity", "Shader Graph".</summary>
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    /// <summary>Категория для группировки: "Programming", "Tools", "Graphics", "Design".</summary>
    public string Category { get; set; } = "General";

    [Range(1, 5)]
    /// <summary>Уровень владения: 1 (начинающий) — 5 (эксперт).</summary>
    public int Level { get; set; } = 3;

    [MaxLength(300)]
    /// <summary>Путь к иконке (SVG/PNG). Опционально.</summary>
    public string? IconUrl { get; set; }

    /// <summary>Опыт в годах. Отображается рядом с навыком на /Skills.</summary>
    public int YearsOfExperience { get; set; }

    /// <summary>Показывать на главной в блоке топ-навыков.</summary>
    public bool IsFeatured { get; set; }

    /// <summary>Ручная сортировка внутри категории (меньше — выше).</summary>
    public int SortOrder { get; set; }
}