using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.Models {
    public class Project {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Title { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        [StringLength(300)]
        public string ShortDescription { get; set; } = string.Empty;

        /// <summary>Markdown-описание.</summary>
        public string FullDescription { get; set; } = string.Empty;

        public string CoverImageUrl { get; set; } = string.Empty;

        /// <summary>Жанр: "Roguelike", "Puzzle", "Action".</summary>
        public string Genre { get; set; } = string.Empty;

        /// <summary>Роль в проекте: "Game Designer", "Level Designer".</summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>CSV: "Unity, C#, Figma".</summary>
        public string TechStack { get; set; } = string.Empty;

        public string SteamUrl { get; set; } = string.Empty;
        public string GooglePlayUrl { get; set; } = string.Empty;
        public string ItchIoUrl { get; set; } = string.Empty;
        public string GitHubUrl { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; }
        public bool IsFeatured { get; set; }

        public List<string> Tags { get; set; } = new();

        /// <summary>Разбивает TechStack CSV в массив.</summary>
        public IEnumerable<string> TechList =>
            string.IsNullOrWhiteSpace(TechStack)
                ? Enumerable.Empty<string>()
                : TechStack.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}