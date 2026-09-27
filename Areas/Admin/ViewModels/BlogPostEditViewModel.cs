using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Portfolio_game_dev.Areas.Admin.ViewModels;

public class BlogPostEditViewModel {
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите заголовок")]
    [StringLength(200, MinimumLength = 2)]
    [Display(Name = "Заголовок")]
    public string Title { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Slug (URL)")]
    [RegularExpression(@"^[a-z0-9]*(-[a-z0-9]+)*$",
        ErrorMessage = "Slug может содержать только a-z, 0-9 и дефисы")]
    public string? Slug { get; set; }

    [StringLength(500)]
    [Display(Name = "Краткое описание")]
    public string? Summary { get; set; }

    [Display(Name = "Полный текст (Markdown)")]
    public string? Content { get; set; }

    [Display(Name = "Дата публикации")]
    [DataType(DataType.DateTime)]
    public DateTime? PublishedAt { get; set; }

    [Display(Name = "Опубликована")]
    public bool IsPublished { get; set; }

    [Range(0, 120)]
    [Display(Name = "Время чтения (мин)")]
    public int ReadTimeMinutes { get; set; } = 5;

    // ── Обложка ──
    [Display(Name = "Обложка")]
    public IFormFile? CoverImage { get; set; }

    public string? ExistingCoverImageUrl { get; set; }

    // ── Теги ──
    [Display(Name = "Теги")]
    public List<int> SelectedTagIds { get; set; } = new();

    public List<TagOption> AvailableTags { get; set; } = new();
}