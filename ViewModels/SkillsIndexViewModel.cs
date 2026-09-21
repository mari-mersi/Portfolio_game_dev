using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.ViewModels;

public class SkillsIndexViewModel {
    public List<SkillCategoryGroup> Categories { get; set; } = new();
    public List<Skill> FeaturedSkills { get; set; } = new();
    public int TotalSkills => Categories.Sum(c => c.Skills.Count);
}

public class SkillCategoryGroup {
    public string Category { get; set; } = string.Empty;
    public List<Skill> Skills { get; set; } = new();
    public double AverageLevel => Skills.Count == 0 ? 0 : Skills.Average(s => s.Level);
}