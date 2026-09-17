using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.Models {
    public class Experience {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Company { get; set; } = string.Empty;

        [Required, StringLength(120)]
        public string Position { get; set; } = string.Empty;

        /// <summary>Тип занятости: "Full-time", "Contract", "Freelance".</summary>
        [StringLength(40)]
        public string EmploymentType { get; set; } = string.Empty;

        /// <summary>Локация или "Remote".</summary>
        [StringLength(80)]
        public string Location { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        /// <summary>null = "по настоящее время".</summary>
        public DateTime? EndDate { get; set; }

        /// <summary>Markdown-описание обязанностей и достижений.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Ключевые достижения — по строке на пункт.</summary>
        public List<string> Highlights { get; set; } = new();

        /// <summary>CSV: "Unity, Figma, Notion".</summary>
        public string TechStack { get; set; } = string.Empty;

        /// <summary>Ссылка на проект/компанию (опционально).</summary>
        public string? Link { get; set; }

        // — вычисляемые —

        public bool IsCurrent => EndDate is null;

        public string Period =>
            $"{StartDate:MMM yyyy} — {(EndDate.HasValue ? EndDate.Value.ToString("MMM yyyy") : "наст. время")}";

        /// <summary>Длительность в месяцах (минимум 1).</summary>
        public int DurationMonths {
            get {
                var end = EndDate ?? DateTime.UtcNow;
                var months = ((end.Year - StartDate.Year) * 12) + end.Month - StartDate.Month;
                return Math.Max(1, months);
            }
        }

        public string DurationText {
            get {
                var months = DurationMonths;
                var years = months / 12;
                var rest = months % 12;

                if (years == 0)
                    return $"{rest} мес.";
                if (rest == 0)
                    return $"{years} г.";
                return $"{years} г. {rest} мес.";
            }
        }

        public IEnumerable<string> TechList =>
            string.IsNullOrWhiteSpace(TechStack)
                ? Enumerable.Empty<string>()
                : TechStack.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}