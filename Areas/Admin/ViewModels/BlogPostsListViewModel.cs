using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Areas.Admin.ViewModels;

public class BlogPostsListViewModel {
    public List<BlogPost> Posts { get; set; } = new();
    public string? SearchTerm { get; set; }
    public string? StatusFilter { get; set; }  // "published" | "draft" | null

    public int TotalCount => Posts.Count;
    public int PublishedCount => Posts.Count(p => p.IsPublished);
    public int DraftCount => Posts.Count(p => !p.IsPublished);
}