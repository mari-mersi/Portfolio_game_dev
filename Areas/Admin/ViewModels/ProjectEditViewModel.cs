using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Portfolio_game_dev.Areas.Admin.ViewModels;

public class ProjectEditViewModel {
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название")]
    [StringLength(200, MinimumLength = 2)]
    [Display(Name = "Название")]
    public string Title { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Slug (URL)")]
    [RegularExpression(@"^[a-z0-9]*(-[a-z0-9]+)*$",
    ErrorMessage = "Slug может содержать только a-z, 0-9 и дефисы")]
    public string? Slug { get; set; }

    [StringLength(500)]
    [Display(Name = "Короткое описание")]
    public string? ShortDescription { get; set; }

    [Display(Name = "Полное описание (Markdown)")]
    public string? FullDescription { get; set; }

    [StringLength(100)]
    [Display(Name = "Жанр")]
    public string? Genre { get; set; }

    [StringLength(100)]
    [Display(Name = "Роль")]
    public string? Role { get; set; }

    [StringLength(300)]
    [Display(Name = "Стек (через запятую)")]
    public string? TechStack { get; set; }

    [Display(Name = "Дата релиза")]
    [DataType(DataType.Date)]
    public DateTime? ReleaseDate { get; set; }

    [Display(Name = "Опубликован")]
    public bool IsPublished { get; set; } = true;

    [Display(Name = "На главной")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Порядок сортировки")]
    public int SortOrder { get; set; }

    // ── Ссылки ──
    [StringLength(300)][Display(Name = "Steam")] public string? SteamUrl { get; set; }
    [StringLength(300)][Display(Name = "itch.io")] public string? ItchIoUrl { get; set; }
    [StringLength(300)][Display(Name = "Google Play")] public string? GooglePlayUrl { get; set; }
    [StringLength(300)][Display(Name = "GitHub")] public string? GitHubUrl { get; set; }
    [StringLength(300)][Display(Name = "Демо")] public string? DemoUrl { get; set; }

    // ── Обложка ──
    [Display(Name = "Обложка (jpg/png/webp)")]
    public IFormFile? CoverImage { get; set; }

    /// <summary>Текущий путь к обложке (для превью при редактировании).</summary>
    public string? ExistingCoverImageUrl { get; set; }

    // ── Теги ──
    [Display(Name = "Теги")]
    public List<int> SelectedTagIds { get; set; } = new();

    /// <summary>Все доступные теги для мультиселекта.</summary>
    public List<TagOption> AvailableTags { get; set; } = new();
}

public class TagOption {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Selected { get; set; }
}