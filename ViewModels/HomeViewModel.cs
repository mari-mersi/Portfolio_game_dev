using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.ViewModels;

/// <summary>
/// ViewModel главной страницы. Собирается одним запросом в HomeController.Index.
/// </summary>
public class HomeViewModel {
    /// <summary>Данные hero-блока (заголовок, подзаголовок, CTA).</summary>
    public HeroViewModel Hero { get; set; } = new();

    /// <summary>Избранные проекты (Project.IsFeatured = true), до 3 штук.</summary>
    public List<Project> FeaturedProjects { get; set; } = new();

    /// <summary>Топ навыков (Skill.IsFeatured = true), до 8 штук.</summary>
    public List<Skill> FeaturedSkills { get; set; } = new();

    /// <summary>Последние опубликованные статьи блога, до 3 штук.</summary>
    public List<BlogPost> LatestPosts { get; set; } = new();
}