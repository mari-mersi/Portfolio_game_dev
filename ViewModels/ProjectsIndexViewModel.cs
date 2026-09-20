using Portfolio_game_dev.Models;
using Portfolio_game_dev.ViewModels.Shared;

namespace Portfolio_game_dev.ViewModels;

/// <summary>
/// ViewModel страницы /Projects — список проектов с фильтром по тегу и пагинацией.
/// </summary>
public class ProjectsIndexViewModel {
    /// <summary>Постраничный результат с текущей страницы (Items, Page, TotalPages и т.д.).</summary>
    public PagedResult<Project> Paged { get; set; } = new();

    /// <summary>Все доступные теги (по ним строим панель фильтров).</summary>
    public List<Tag> AllTags { get; set; } = new();

    /// <summary>Slug активного тега или null, если фильтр не задан.</summary>
    public string? SelectedTag { get; set; }

    // Удобные шорткаты — чтобы во вьюхе писать Model.Projects, а не Model.Paged.Items
    public List<Project> Projects => Paged.Items;
    public int CurrentPage => Paged.Page;
    public int TotalPages => Paged.TotalPages;
    public int TotalCount => Paged.TotalCount;
    public bool HasPrevious => Paged.HasPrevious;
    public bool HasNext => Paged.HasNext;
}