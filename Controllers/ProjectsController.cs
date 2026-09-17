using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Controllers {
    /// <summary>
    /// Портфолио — список и детали
    /// </summary>
    public class ProjectsController : Controller {
        // В реальном приложении здесь инжектируется DbContext или IProjectService
        // private readonly IProjectService _projectService;

        public IActionResult Index(string? tag, int page = 1) {
            int pageSize = 6;
            var allProjects = GetMockProjects();

            // Получаем список всех уникальных тегов
            var allTags = allProjects
                .SelectMany(p => p.Tags)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(t => t)
                .ToList();

            // Фильтрация по тегу
            var filtered = string.IsNullOrWhiteSpace(tag)
                ? allProjects
                : allProjects.Where(p => p.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase))).ToList();

            int totalProjects = filtered.Count;
            var pagedProjects = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new ProjectsIndexViewModel {
                Projects = pagedProjects,
                AllTags = allTags,
                SelectedTag = tag,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalProjects / (double)pageSize)
            };

            return View(viewModel);
        }

        [HttpGet("projects/{id:int}")]
        public IActionResult Details(int id) {
            var projects = GetMockProjects();
            var project = projects.FirstOrDefault(p => p.Id == id);

            if (project == null) {
                return NotFound();
            }

            var relatedProjects = projects
                .Where(p => p.Id != project.Id && p.Tags.Any(t => project.Tags.Contains(t)))
                .Take(3)
                .ToList();

            var viewModel = new ProjectDetailsViewModel {
                Project = project,
                RelatedProjects = relatedProjects
            };

            return View(viewModel);
        }

        [HttpGet("projects/{slug}")]
        public IActionResult DetailsBySlug(string slug) {
            var projects = GetMockProjects();
            var project = projects.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

            if (project == null) {
                return NotFound();
            }

            var relatedProjects = projects
                .Where(p => p.Id != project.Id && p.Tags.Any(t => project.Tags.Contains(t)))
                .Take(3)
                .ToList();

            var viewModel = new ProjectDetailsViewModel {
                Project = project,
                RelatedProjects = relatedProjects
            };

            return View("Details", viewModel);
        }

        public IActionResult ByTag(string tag) {
            return RedirectToAction(nameof(Index), new { tag = tag });
        }

        #region Mock Data
        private List<Project> GetMockProjects() {
            return new List<Project>
            {
                new Project
                {
                    Id = 1,
                    Title = "Cyber Odyssey",
                    Slug = "cyber-odyssey",
                    ShortDescription = "Футуристический 3D-экшен платформер с процедурной генерацией уровней и динаминами боями.",
                    FullDescription = "## О проекте\n**Cyber Odyssey** — это мой основной проект за последний год...\n\n### Реализованный функционал:\n- Кастомная физика персонажа\n- Поведенческое дерево ИИ (Behavior Tree)\n- Оптимизация шейдеров и освещения",
                    CoverImageUrl = "/images/projects/cyber.jpg",
                    Genre = "Action / Platformer",
                    Role = "Lead Game Developer",
                    TechStack = "Unity, C#, Shader Graph, HLSL",
                    SteamUrl = "https://store.steampowered.com",
                    GitHubUrl = "https://github.com",
                    ReleaseDate = DateTime.Now.AddMonths(-3),
                    IsFeatured = true,
                    Tags = new List<string> { "Unity", "C#", "3D", "Action" }
                },
                new Project
                {
                    Id = 2,
                    Title = "Shadows of Eldoria",
                    Slug = "shadows-of-eldoria",
                    ShortDescription = "Пошаговая тактическая RPG в жанре тёмного фэнтези.",
                    FullDescription = "Тактический рогалик с акцентом на комбинирование стихийных заклинаний...",
                    CoverImageUrl = "/images/projects/eldoria.jpg",
                    Genre = "Tactical RPG",
                    Role = "Game Designer & Developer",
                    TechStack = "Unreal Engine 5, C++, Blueprints",
                    ItchIoUrl = "https://itch.io",
                    ReleaseDate = DateTime.Now.AddYears(-1),
                    IsFeatured = true,
                    Tags = new List<string> { "Unreal Engine", "C++", "RPG" }
                }
            };
        }
        #endregion
    }
}