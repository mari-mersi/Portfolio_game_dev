using System.Collections.Generic;

namespace Portfolio_game_dev.Models {
    public class ProjectsIndexViewModel {
        public List<Project> Projects { get; set; } = new();
        public List<string> AllTags { get; set; } = new();
        public string? SelectedTag { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}