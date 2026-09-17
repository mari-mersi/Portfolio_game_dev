using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Services;

public interface ISkillService {
    Task<List<Skill>> GetAllAsync();
    Task<List<Skill>> GetFeaturedAsync();
    Task<List<Skill>> GetByCategoryAsync(string category);
    Task<Skill?> GetByIdAsync(int id);
    Task CreateAsync(Skill skill);
    Task UpdateAsync(Skill skill);
    Task DeleteAsync(int id);
}