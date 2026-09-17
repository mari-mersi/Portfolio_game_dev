using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Areas.Admin.ViewModels;

public class SkillsListViewModel {
    public List<Skill> Skills { get; set; } = new();
    public string? SearchTerm { get; set; }
    public string? CategoryFilter { get; set; }
    public List<string> AvailableCategories { get; set; } = new();

    // Статистика для верхней плашки
    public int TotalCount => Skills.Count;
    public int FeaturedCount => Skills.Count(s => s.IsFeatured);
    public double AverageLevel => Skills.Count == 0 ? 0 : Skills.Average(s => s.Level);
}