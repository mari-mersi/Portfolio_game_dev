namespace Portfolio_game_dev.ViewModels.Shared;

/// <summary>
/// Универсальный результат постраничной выдачи.
/// Используется в списках проектов, статей и т.п.
/// </summary>
public class PagedResult<T> {
    /// <summary>Элементы текущей страницы.</summary>
    public List<T> Items { get; set; } = new();

    /// <summary>Номер текущей страницы (1-based).</summary>
    public int Page { get; set; }

    /// <summary>Размер страницы (сколько элементов запрошено).</summary>
    public int PageSize { get; set; }

    /// <summary>Всего элементов во всей выборке (без учёта страницы).</summary>
    public int TotalCount { get; set; }

    /// <summary>Всего страниц.</summary>
    public int TotalPages => PageSize <= 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>Есть ли предыдущая страница.</summary>
    public bool HasPrevious => Page > 1;

    /// <summary>Есть ли следующая страница.</summary>
    public bool HasNext => Page < TotalPages;

    /// <summary>
    /// Удобный конструктор на случай ручной сборки.
    /// </summary>
    public static PagedResult<T> Create(List<T> items, int page, int pageSize, int totalCount)
        => new() { Items = items, Page = page, PageSize = pageSize, TotalCount = totalCount };
}