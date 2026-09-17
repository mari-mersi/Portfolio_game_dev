using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Services {
    public interface IExperienceService {
        /// <summary>Все места работы, свежие сверху.</summary>
        Task<List<Experience>> GetAllAsync();

        /// <summary>Последние N мест — для главной страницы.</summary>
        Task<List<Experience>> GetRecentAsync(int count = 3);
    }
}