using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.Models;

public class Skill {
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Category { get; set; } = string.Empty;   // "Backend", "Frontend", "DevOps", "Tools", "Soft Skills"

    [Range(1, 5)]
    public int Level { get; set; }                          // 1..5

    public string? IconUrl { get; set; }

    public int YearsOfExperience { get; set; }              // опционально — для красоты

    public bool IsFeatured { get; set; }                    // показывать на главной
}