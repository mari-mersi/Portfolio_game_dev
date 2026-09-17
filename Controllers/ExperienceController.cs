using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services;
using Portfolio_game_dev.ViewModels;

namespace Portfolio_game_dev.Controllers {
    /// <summary>
    /// Таймлайн опыта работы.
    /// </summary>
    public class ExperienceController : Controller {
        private readonly IExperienceService _experience;

        public ExperienceController(IExperienceService experience) {
            _experience = experience;
        }

        // GET: /experience
        public async Task<IActionResult> Index() {
            var items = await _experience.GetAllAsync();

            var vm = new ExperienceIndexViewModel {
                Items = items,
                Groups = BuildYearGroups(items),
                TotalMonths = CalculateTotalMonths(items),
                AllTech = items
                    .SelectMany(i => i.TechList)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
                    .ToList()
            };

            return View(vm);
        }

        /// <summary>Группирует места работы по году начала, свежие сверху.</summary>
        private static List<ExperienceYearGroup> BuildYearGroups(List<Experience> items) =>
            items
                .GroupBy(i => i.StartDate.Year)
                .OrderByDescending(g => g.Key)
                .Select(g => new ExperienceYearGroup {
                    Year = g.Key,
                    Items = g.OrderByDescending(i => i.StartDate).ToList()
                })
                .ToList();

        /// <summary>
        /// Суммарный стаж в месяцах по объединению интервалов.
        /// Пример: две работы внахлёст на 2 месяца не превратятся в двойной стаж.
        /// </summary>
        private static int CalculateTotalMonths(List<Experience> items) {
            if (items.Count == 0)
                return 0;

            var intervals = items
                .Select(i => (
                    Start: new DateTime(i.StartDate.Year, i.StartDate.Month, 1),
                    End: i.EndDate.HasValue
                        ? new DateTime(i.EndDate.Value.Year, i.EndDate.Value.Month, 1)
                        : new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)))
                .OrderBy(x => x.Start)
                .ToList();

            var merged = new List<(DateTime Start, DateTime End)>();
            var current = intervals[0];

            foreach (var next in intervals.Skip(1)) {
                // Если следующий интервал начинается не позже конца текущего — сливаем.
                if (next.Start <= current.End) {
                    if (next.End > current.End)
                        current = (current.Start, next.End);
                }
                else {
                    merged.Add(current);
                    current = next;
                }
            }
            merged.Add(current);

            return merged.Sum(m =>
                ((m.End.Year - m.Start.Year) * 12) + m.End.Month - m.Start.Month + 1);
        }
    }
}