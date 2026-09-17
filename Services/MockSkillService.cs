using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Services;

/// <summary>
/// Временная реализация на in-memory списке.
/// Позже заменяется на SkillService с EF Core — интерфейс остаётся тем же.
/// </summary>
public class MockSkillService : ISkillService {
    private readonly List<Skill> _skills;
    private int _nextId = 1;

    public MockSkillService() {
        _skills = new List<Skill>
        {
            // Backend
            new() { Id = _nextId++, Name = "C# / .NET",   Category = "Backend",  Level = 5, YearsOfExperience = 5, IsFeatured = true,  IconUrl = "/img/skills/dotnet.svg" },
            new() { Id = _nextId++, Name = "ASP.NET Core",Category = "Backend",  Level = 5, YearsOfExperience = 4, IsFeatured = true,  IconUrl = "/img/skills/aspnet.svg" },
            new() { Id = _nextId++, Name = "EF Core",     Category = "Backend",  Level = 4, YearsOfExperience = 4, IconUrl = "/img/skills/efcore.svg" },
            new() { Id = _nextId++, Name = "PostgreSQL",  Category = "Backend",  Level = 4, YearsOfExperience = 3, IconUrl = "/img/skills/postgres.svg" },
            new() { Id = _nextId++, Name = "REST API",    Category = "Backend",  Level = 5, YearsOfExperience = 4, IsFeatured = true,  IconUrl = "/img/skills/rest.svg" },

            // Frontend
            new() { Id = _nextId++, Name = "Razor / HTML5", Category = "Frontend", Level = 5, YearsOfExperience = 4, IconUrl = "/img/skills/html.svg" },
            new() { Id = _nextId++, Name = "Tailwind CSS",  Category = "Frontend", Level = 4, YearsOfExperience = 2, IsFeatured = true, IconUrl = "/img/skills/tailwind.svg" },
            new() { Id = _nextId++, Name = "JavaScript / TS",Category = "Frontend",Level = 4, YearsOfExperience = 4, IconUrl = "/img/skills/js.svg" },

            // DevOps
            new() { Id = _nextId++, Name = "Docker",   Category = "DevOps", Level = 4, YearsOfExperience = 3, IconUrl = "/img/skills/docker.svg" },
            new() { Id = _nextId++, Name = "GitHub Actions", Category = "DevOps", Level = 3, YearsOfExperience = 2, IconUrl = "/img/skills/gha.svg" },
            new() { Id = _nextId++, Name = "Linux",    Category = "DevOps", Level = 4, YearsOfExperience = 5, IconUrl = "/img/skills/linux.svg" },

            // Tools
            new() { Id = _nextId++, Name = "Git",       Category = "Tools", Level = 5, YearsOfExperience = 6, IconUrl = "/img/skills/git.svg" },
            new() { Id = _nextId++, Name = "Rider / VS",Category = "Tools", Level = 5, YearsOfExperience = 5, IconUrl = "/img/skills/rider.svg" },
            new() { Id = _nextId++, Name = "Unity",     Category = "Tools", Level = 4, YearsOfExperience = 3, IsFeatured = true, IconUrl = "/img/skills/unity.svg" },

            // Soft skills
            new() { Id = _nextId++, Name = "Code Review",  Category = "Soft Skills", Level = 5, YearsOfExperience = 4 },
            new() { Id = _nextId++, Name = "Mentoring",    Category = "Soft Skills", Level = 4, YearsOfExperience = 3 },
        };
    }

    public Task<List<Skill>> GetAllAsync()
        => Task.FromResult(_skills
            .OrderBy(s => s.Category)
            .ThenByDescending(s => s.Level)
            .ToList());

    public Task<List<Skill>> GetFeaturedAsync()
        => Task.FromResult(_skills
            .Where(s => s.IsFeatured)
            .OrderByDescending(s => s.Level)
            .ToList());

    public Task<List<Skill>> GetByCategoryAsync(string category)
        => Task.FromResult(_skills
            .Where(s => string.Equals(s.Category, category, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(s => s.Level)
            .ToList());

    public Task<Skill?> GetByIdAsync(int id)
        => Task.FromResult(_skills.FirstOrDefault(s => s.Id == id));

    public Task CreateAsync(Skill skill) {
        skill.Id = _nextId++;
        _skills.Add(skill);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Skill skill) {
        var existing = _skills.FirstOrDefault(s => s.Id == skill.Id);
        if (existing is null)
            return Task.CompletedTask;

        existing.Name = skill.Name;
        existing.Category = skill.Category;
        existing.Level = skill.Level;
        existing.IconUrl = skill.IconUrl;
        existing.YearsOfExperience = skill.YearsOfExperience;
        existing.IsFeatured = skill.IsFeatured;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id) {
        var s = _skills.FirstOrDefault(x => x.Id == id);
        if (s is not null)
            _skills.Remove(s);
        return Task.CompletedTask;
    }
}