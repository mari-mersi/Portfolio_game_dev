using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.Areas.Admin.ViewModels;

public class ExperienceEditViewModel {
    public int Id { get; set; }

    [Required(ErrorMessage = "Укажите компанию")]
    [StringLength(150, MinimumLength = 2)]
    [Display(Name = "Компания")]
    public string Company { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите должность")]
    [StringLength(150, MinimumLength = 2)]
    [Display(Name = "Должность")]
    public string Position { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Тип занятости")]
    public string? EmploymentType { get; set; }

    [StringLength(150)]
    [Display(Name = "Локация")]
    public string? Location { get; set; }

    [Required(ErrorMessage = "Укажите дату начала")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата начала")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    [Display(Name = "Дата окончания")]
    public DateTime? EndDate { get; set; }

    [Display(Name = "Текущее место работы")]
    public bool IsCurrent { get; set; }

    [Display(Name = "Описание")]
    public string? Description { get; set; }

    [Display(Name = "Достижения (по одному в строке)")]
    public string? HighlightsText { get; set; }

    [StringLength(300)]
    [Display(Name = "Стек (через запятую)")]
    public string? TechStack { get; set; }

    [Display(Name = "Порядок сортировки")]
    public int SortOrder { get; set; }
}