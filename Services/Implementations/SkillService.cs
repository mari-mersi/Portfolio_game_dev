using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services.Abstractions;

namespace Portfolio_game_dev.Services.Implementations;

/// <summary>
/// Реализация ISkillService поверх EF Core.
/// </summary>
public class SkillService : ISkillService {
    private readonly AppDbContext _db;

    public SkillService(AppDbContext db) {
        _db = db;
    }

    public async Task<List<Skill>> GetTopAsync(int count = 8, CancellationToken ct = default) {
        return await _db.Skills
            .Where(s => s.IsFeatured)
            .OrderBy(s => s.SortOrder)
            .ThenByDescending(s => s.Level)         // при равном SortOrder — сильнейшие выше
            .Take(count)
            .ToListAsync(ct);
    }
}