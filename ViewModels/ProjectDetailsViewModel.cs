using System.Collections.Generic;

namespace Portfolio_game_dev.Models {
    public class ProjectDetailsViewModel {
        public Project Project { get; set; } = null!;
        public List<Project> RelatedProjects { get; set; } = new();
    }
}