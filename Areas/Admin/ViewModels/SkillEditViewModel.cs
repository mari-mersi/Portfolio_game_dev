using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.Areas.Admin.ViewModels;

public class SkillEditViewModel {
    public int Id { get; set; }

    [Required(ErrorMessage = "Название обязательно")]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Категория обязательна")]
    [StringLength(50)]
    [Display(Name = "Категория")]
    public string Category { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "Уровень от 1 до 5")]
    [Display(Name = "Уровень (1-5)")]
    public int Level { get; set; } = 3;

    [Display(Name = "Иконка (URL)")]
    [Url(ErrorMessage = "Некорректный URL")]
    public string? IconUrl { get; set; }

    [Range(0, 50)]
    [Display(Name = "Лет опыта")]
    public int YearsOfExperience { get; set; }

    [Display(Name = "Показывать на главной")]
    public bool IsFeatured { get; set; }

    // Список для <select> категорий
    public List<string> AvailableCategories { get; set; } = new();
}