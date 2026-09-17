using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.ViewModels {
    public class HomeViewModel {
        public string HeroTagline { get; set; } = string.Empty;
        public string HeroSubline { get; set; } = string.Empty;
        public List<Project> FeaturedProjects { get; set; } = new();
        public List<Skill> TopSkills { get; set; } = new();
        public List<Experience> RecentExperience { get; set; } = new();
    }
}