using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services.Abstractions;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Services.Implementations;

/// <summary>
/// Реализация IExperienceService поверх EF Core.
/// </summary>
public class ExperienceService : IExperienceService {
    private readonly AppDbContext _db;

    public ExperienceService(AppDbContext db) {
        _db = db;
    }

    public async Task<List<Experience>> GetAllAsync(CancellationToken ct = default) {
        return await _db.Experiences
            .AsNoTracking()
            .OrderByDescending(e => e.StartDate)
            .ToListAsync(ct);
    }

    public async Task<ExperienceIndexViewModel> GetTimelineAsync(CancellationToken ct = default) {
        var items = await GetAllAsync(ct);

        // Группируем по году начала (свежие сверху).
        var groups = items
            .GroupBy(e => e.StartDate.Year)
            .OrderByDescending(g => g.Key)
            .Select(g => new ExperienceYearGroup {
                Year = g.Key,
                Items = g.OrderByDescending(e => e.StartDate).ToList()
            })
            .ToList();

        // Суммарный стаж — считаем по объединению интервалов, чтобы
        // пересекающиеся периоды не удваивались.
        var totalMonths = CalculateTotalMonths(items);

        // Уникальные технологии из всех мест работы.
        var allTech = items
            .SelectMany(e => e.TechList)
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t)
            .ToList();

        return new ExperienceIndexViewModel {
            Items = items,
            Groups = groups,
            TotalMonths = totalMonths,
            AllTech = allTech
        };
    }

    /// <summary>
    /// Считает общий стаж в месяцах через объединение пересекающихся интервалов.
    /// Например, [2020-01 .. 2022-01] и [2021-06 .. настоящее] не удвоят середину 2021.
    /// </summary>
    private static int CalculateTotalMonths(List<Experience> items) {
        if (items.Count == 0)
            return 0;

        // Строим интервалы (месяцы от 0001-01-01, просто для сортировки).
        var intervals = items
            .Select(e => (
                Start: e.StartDate.Year * 12 + e.StartDate.Month,
                End: (e.EndDate ?? DateTime.UtcNow).Year * 12 + (e.EndDate ?? DateTime.UtcNow).Month
            ))
            .Where(i => i.End >= i.Start)
            .OrderBy(i => i.Start)
            .ToList();

        if (intervals.Count == 0)
            return 0;

        int total = 0;
        var currentStart = intervals[0].Start;
        var currentEnd = intervals[0].End;

        for (int i = 1; i < intervals.Count; i++) {
            var (start, end) = intervals[i];

            if (start <= currentEnd + 1) {
                // Пересекается или примыкает — расширяем текущий интервал.
                currentEnd = Math.Max(currentEnd, end);
            }
            else {
                // Разрыв — закрываем текущий интервал, начинаем новый.
                total += currentEnd - currentStart;
                currentStart = start;
                currentEnd = end;
            }
        }

        // Закрываем последний интервал.
        total += currentEnd - currentStart;

        return total;
    }
}