using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.ViewModels;

/// <summary>
/// ViewModel детальной страницы проекта.
/// </summary>
public class ProjectDetailsViewModel {
    /// <summary>Сам проект со всеми медиа и тегами (загружен Include).</summary>
    public Project Project { get; set; } = null!;

    /// <summary>
    /// Похожие проекты — по общим тегам, кроме самого текущего.
    /// До 3 штук.
    /// </summary>
    public List<Project> RelatedProjects { get; set; } = new();
}