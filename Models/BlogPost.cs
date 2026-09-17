using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.Models {
    public class BlogPost {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>URL-идентификатор: /blog/{slug}.</summary>
        [Required, StringLength(200)]
        public string Slug { get; set; } = string.Empty;

        /// <summary>Краткое описание для списка и meta-тега.</summary>
        [StringLength(400)]
        public string Summary { get; set; } = string.Empty;

        /// <summary>Тело статьи в Markdown.</summary>
        public string Content { get; set; } = string.Empty;

        public string? CoverImageUrl { get; set; }

        public DateTime PublishedAt { get; set; }

        /// <summary>Черновик не показываем в публичном списке.</summary>
        public bool IsPublished { get; set; } = true;

        public List<string> Tags { get; set; } = new();

        /// <summary>Примерное время чтения (мин), считаем из длины контента.</summary>
        public int ReadTimeMinutes =>
            Math.Max(1, (int)Math.Ceiling(Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length / 200.0));
    }
}