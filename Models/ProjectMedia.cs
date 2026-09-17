namespace Portfolio_game_dev.Models {
    public class ProjectMedia {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Url { get; set; } = string.Empty;
        public MediaType Type { get; set; }
        public string? Caption { get; set; }
    }

    public enum MediaType {
        Image,
        Gif,
        Video
    }
}