using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Services {
    public interface IContentService {
        Task<List<Project>> GetFeaturedProjectsAsync(int count = 3);
        Task<List<Skill>> GetTopSkillsAsync(int count = 8);
        Task<List<Experience>> GetRecentExperienceAsync(int count = 3);
        Task SaveContactMessageAsync(ContactMessage message);
    }
}