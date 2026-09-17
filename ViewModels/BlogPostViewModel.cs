using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.ViewModels {
    public class BlogPostViewModel {
        public BlogPost Post { get; set; } = new();

        /// <summary>HTML, полученный из Markdown-контента.</summary>
        public string RenderedContent { get; set; } = string.Empty;

        public List<BlogPost> RelatedPosts { get; set; } = new();
    }
}