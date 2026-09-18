using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.Models;

/// <summary>
/// Запись об опыте работы. Отображается таймлайном на /Experience.
/// </summary>
public class Experience : BaseEntity {
    [Required, MaxLength(150)]
    /// <summary>Компания или название проекта.</summary>
    public string Company { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    /// <summary>Должность: "Middle Unity Developer", "Game Designer".</summary>
    public string Position { get; set; } = string.Empty;

    [MaxLength(50)]
    /// <summary>Тип занятости: "Full-time", "Contract", "Freelance".</summary>
    public string? EmploymentType { get; set; }

    [MaxLength(150)]
    /// <summary>Локация: "Москва", "Remote".</summary>
    public string? Location { get; set; }

    /// <summary>Дата начала работы.</summary>
    public DateTime StartDate { get; set; }

    /// <summary>Дата окончания. null — если IsCurrent=true или опыт всё ещё продолжается.</summary>
    public DateTime? EndDate { get; set; }

    /// <summary>Описание обязанностей/достижений. Markdown.</summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ключевые достижения списком. Хранится в БД как TEXT с разделителем '\n'
    /// (HasConversion в AppDbContext). В коде — обычный List&lt;string&gt;.
    /// </summary>
    public List<string> Highlights { get; set; } = new();

    /// <summary>Стек строкой через запятую: "Unity, C#, Zenject". Разбивается через TechList.</summary>
    public string? TechStack { get; set; }

    /// <summary>true — текущее место работы (EndDate игнорируется).</summary>
    public bool IsCurrent { get; set; }

    /// <summary>Ручная сортировка (меньше — выше).</summary>
    public int SortOrder { get; set; }

    // ─── Вычисляемые свойства (не мапятся в БД) ─────────────────────

    /// <summary>TechStack, разбитый на элементы.</summary>
    public IEnumerable<string> TechList =>
        string.IsNullOrWhiteSpace(TechStack)
            ? Enumerable.Empty<string>()
            : TechStack.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    /// <summary>Период работы в формате "июн 2023 — настоящее" / "янв 2020 — дек 2022".</summary>
    public string Period =>
        $"{StartDate:MMM yyyy} — {(IsCurrent || EndDate is null ? "настоящее" : EndDate.Value.ToString("MMM yyyy"))}";

    /// <summary>Длительность в формате "8 мес." / "1 г. 3 мес.".</summary>
    public string DurationText {
        get {
            var end = EndDate ?? DateTime.UtcNow;
            var months = ((end.Year - StartDate.Year) * 12) + end.Month - StartDate.Month;
            if (months < 12)
                return $"{months} мес.";
            var years = months / 12;
            var rem = months % 12;
            return rem == 0 ? $"{years} г." : $"{years} г. {rem} мес.";
        }
    }
}