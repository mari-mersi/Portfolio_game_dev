using Portfolio_game_dev.Models;
using Portfolio_game_dev.ViewModels.Shared;

namespace Portfolio_game_dev.ViewModels;

/// <summary>
/// ViewModel страницы /Blog — список статей с пагинацией.
/// </summary>
public class BlogIndexViewModel {
    public PagedResult<BlogPost> Paged { get; set; } = new();

    // Шорткаты для вьюхи
    public List<BlogPost> Posts => Paged.Items;
    public int CurrentPage => Paged.Page;
    public int TotalPages => Paged.TotalPages;
    public int TotalCount => Paged.TotalCount;
}