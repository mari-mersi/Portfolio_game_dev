using Portfolio_game_dev.Models;
using Portfolio_game_dev.ViewModels;
using Portfolio_game_dev.ViewModels.Shared;

namespace Portfolio_game_dev.Services.Abstractions;

/// <summary>
/// Работа с проектами: списки, детали, фильтры.
/// </summary>
public interface IProjectService {
    /// <summary>
    /// Избранные проекты для главной: IsPublished=true, IsFeatured=true,
    /// отсортированы по SortOrder. Максимум <paramref name="count"/> штук.
    /// </summary>
    Task<List<Project>> GetFeaturedAsync(int count = 3, CancellationToken ct = default);

    /// <summary>
    /// Постраничный список опубликованных проектов с фильтром по тегу.
    /// </summary>
    /// <param name="page">Номер страницы (1-based). Значения &lt; 1 приводятся к 1.</param>
    /// <param name="tag">Slug тега для фильтра. null/пусто — без фильтра.</param>
    /// <param name="pageSize">Размер страницы. По умолчанию 6.</param>
    Task<PagedResult<Project>> GetPagedAsync(
        int page,
        string? tag,
        int pageSize = 6,
        CancellationToken ct = default);

    /// <summary>
    /// Проект по slug с загруженными Media и Tags, плюс до 3 похожих проектов
    /// по общим тегам. Возвращает null, если проект не найден или не опубликован.
    /// </summary>
    Task<ProjectDetailsViewModel?> GetBySlugAsync(string slug, CancellationToken ct = default);

    /// <summary>
    /// Все теги, которые используются хотя бы в одном опубликованном проекте,
    /// отсортированные по имени.
    /// </summary>
    Task<List<Tag>> GetAllTagsAsync(CancellationToken ct = default);
}