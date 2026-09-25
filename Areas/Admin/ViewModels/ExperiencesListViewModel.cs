using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Areas.Admin.ViewModels;

public class ExperiencesListViewModel {
    public List<Experience> Experiences { get; set; } = new();
    public string? SearchTerm { get; set; }
    public string? CompanyFilter { get; set; }
    public List<string> AvailableCompanies { get; set; } = new();

    public int TotalCount => Experiences.Count;
    public int CurrentCount => Experiences.Count(e => e.IsCurrent);
}