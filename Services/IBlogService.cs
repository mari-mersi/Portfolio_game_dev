using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Services {
    public interface IBlogService {
        Task<List<BlogPost>> GetPublishedAsync(int page = 1, int pageSize = 6);
        Task<int> GetPublishedCountAsync();
        Task<BlogPost?> GetBySlugAsync(string slug);
        Task<List<BlogPost>> GetRelatedAsync(BlogPost post, int count = 3);
    }
}