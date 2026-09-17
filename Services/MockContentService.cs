using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Services {
    public class MockContentService : IContentService {
        private static readonly List<Project> _projects = new()
        {
            new Project
            {
                Id = 1,
                Title = "Neon Drift",
                Slug = "neon-drift",
                ShortDescription = "Аркадный roguelike-рейсинг с процедурными трассами.",
                Genre = "Roguelike / Racing",
                Role = "Game Designer",
                TechStack = "Unity, C#, Figma",
                ReleaseDate = new DateTime(2025, 3, 12),
                IsFeatured = true,
                Tags = new() { "Unity", "Roguelike", "Procedural" }
            },
            new Project
            {
                Id = 2,
                Title = "Paper Kingdom",
                Slug = "paper-kingdom",
                ShortDescription = "Пошаговая тактика с бумажной эстетикой и физикой.",
                Genre = "Tactics",
                Role = "Level Designer",
                TechStack = "Godot, GDScript, Blender",
                ReleaseDate = new DateTime(2024, 10, 1),
                IsFeatured = true,
                Tags = new() { "Godot", "Tactics", "Paper" }
            },
            new Project
            {
                Id = 3,
                Title = "Echo Chamber",
                Slug = "echo-chamber",
                ShortDescription = "Головоломка про звук и отражения.",
                Genre = "Puzzle",
                Role = "Game Designer",
                TechStack = "Unreal, Blueprints",
                ReleaseDate = new DateTime(2024, 5, 20),
                IsFeatured = true,
                Tags = new() { "Unreal", "Puzzle", "Sound" }
            }
        };

        private static readonly List<Skill> _skills = new()
        {
            new Skill { Id = 1, Name = "Game Design",     Category = "Design",      Level = 5 },
            new Skill { Id = 2, Name = "Level Design",    Category = "Design",      Level = 4 },
            new Skill { Id = 3, Name = "Economy Balance", Category = "Design",      Level = 4 },
            new Skill { Id = 4, Name = "Unity",           Category = "Tools",       Level = 4 },
            new Skill { Id = 5, Name = "Godot",           Category = "Tools",       Level = 3 },
            new Skill { Id = 6, Name = "Figma",           Category = "Tools",       Level = 4 },
            new Skill { Id = 7, Name = "C#",              Category = "Programming", Level = 4 },
            new Skill { Id = 8, Name = "GDScript",        Category = "Programming", Level = 3 }
        };

        private static readonly List<Experience> _experience = new()
        {
            new Experience
            {
                Id = 1,
                Company = "Pixel Forge Studio",
                Position = "Game Designer",
                StartDate = new DateTime(2023, 6, 1),
                EndDate = null,
                Description = "Проектирование core loop, баланс экономики, документирование фич."
            },
            new Experience
            {
                Id = 2,
                Company = "Indie Team «Paperworks»",
                Position = "Level Designer",
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2023, 5, 1),
                Description = "Уровни для тактики, плейтесты, итерации на основе фидбэка."
            },
            new Experience
            {
                Id = 3,
                Company = "Freelance",
                Position = "Game Designer / Prototyper",
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2021, 12, 31),
                Description = "Быстрые прототипы на Unity и Godot, документирование GDD."
            }
        };

        public Task<List<Project>> GetFeaturedProjectsAsync(int count = 3) =>
            Task.FromResult(_projects.Where(p => p.IsFeatured).Take(count).ToList());

        public Task<List<Skill>> GetTopSkillsAsync(int count = 8) =>
            Task.FromResult(_skills.OrderByDescending(s => s.Level).Take(count).ToList());

        public Task<List<Experience>> GetRecentExperienceAsync(int count = 3) =>
            Task.FromResult(_experience.OrderByDescending(e => e.StartDate).Take(count).ToList());

        public Task SaveContactMessageAsync(ContactMessage message) {
            // TODO: заменить на EF Core — сохранение в БД + отправка e-mail.
            Console.WriteLine($"[Contact] {message.Name} <{message.Email}>: {message.Message}");
            return Task.CompletedTask;
        }
    }
}