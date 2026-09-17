using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.ViewModels {
    public class ExperienceIndexViewModel {
        /// <summary>Все места работы, отсортированные по дате (свежие сверху).</summary>
        public List<Experience> Items { get; set; } = new();

        /// <summary>Опыт, сгруппированный по годам начала (для вертикального таймлайна).</summary>
        public List<ExperienceYearGroup> Groups { get; set; } = new();

        /// <summary>Суммарный стаж в месяцах (с учётом пересечений — по объединению интервалов).</summary>
        public int TotalMonths { get; set; }

        public string TotalExperienceText {
            get {
                var years = TotalMonths / 12;
                var rest = TotalMonths % 12;

                if (years == 0)
                    return $"{rest} мес.";
                if (rest == 0)
                    return $"{years} г.";
                return $"{years} г. {rest} мес.";
            }
        }

        /// <summary>Уникальные компании (для строки "работал в N студиях").</summary>
        public int CompaniesCount => Items.Select(i => i.Company).Distinct().Count();

        /// <summary>Уникальные технологии из всех мест работы.</summary>
        public List<string> AllTech { get; set; } = new();
    }

    public class ExperienceYearGroup {
        public int Year { get; set; }
        public List<Experience> Items { get; set; } = new();
    }
}