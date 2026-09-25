using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Data;

/// <summary>
/// Заполняет БД начальными данными при старте приложения.
/// Идемпотентен: повторный вызов не создаёт дубликатов.
/// </summary>
public static class DbSeeder {
    /// <summary>
    /// Создать роль Admin, пользователя-админа (если нет) и демо-данные (если таблица Projects пуста).
    /// Вызывается один раз в Program.cs при старте приложения.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider services) {
        var db = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var config = services.GetRequiredService<IConfiguration>();

        // Применяем миграции — на случай, если БД создаётся впервые или отстаёт по версии.
        await db.Database.MigrateAsync();

        // ── Роль Admin ──────────────────────────────────────────────
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        // ── Пользователь-админ ──────────────────────────────────────
        // Email/пароль берём из appsettings (AdminCredentials).
        // Для прода — через переменные окружения, не в appsettings.
        var adminEmail = config["AdminCredentials:Email"] ?? "admin@local";
        var adminPassword = config["AdminCredentials:Password"] ?? "Admin123!";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null) {
            admin = new IdentityUser {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true   // иначе SignInManager заблокирует вход
            };
            await userManager.CreateAsync(admin, adminPassword);
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ── Демо-данные ─────────────────────────────────────────────
        // Защита от повторного запуска: если Projects уже есть — сидер молчит.
        // Именно поэтому при правках сидера надо удалять portfolio.db, иначе
        // новые данные не появятся.
        if (!await db.Projects.AnyAsync()) {
            // Сначала теги: на них ссылаются и проекты, и статьи.
            var tags = new List<Tag>
            {
                new() { Name = "Unity", Slug = "unity" },
                new() { Name = "C#", Slug = "csharp" },
                new() { Name = "Unreal Engine", Slug = "unreal-engine" },
                new() { Name = "Game Design", Slug = "game-design" },
            };
            db.Tags.AddRange(tags);

            // Проекты. Tags проставляем ссылками на уже созданные объекты —
            // EF сам разрулит вставку в Projects и ProjectTags.
            db.Projects.AddRange(
                new Project {
                    Title = "Cyber Odyssey",
                    Slug = "cyber-odyssey",
                    ShortDescription = "Футуристический 3D-экшн платформер",
                    FullDescription = """
                        ## О проекте
                        **Cyber Odyssey** — мой основной проект.

                        ### Реализованный функционал
                        - Кастомная физика персонажа
                        - Поведенческое дерево ИИ (Behavior Tree)
                        - Оптимизация шейдеров и освещения

                        ### Технические детали
                        Проект использует URP с кастомными шейдерами. Система абилок построена на ScriptableObjects.
                        """,
                    Genre = "Action / Platformer",
                    Role = "Lead Game Developer",
                    TechStack = "Unity, C#, Shader Graph",
                    ReleaseDate = new DateTime(2025, 6, 15),
                    IsFeatured = true,
                    Tags = new List<Tag> { tags[0], tags[1] }
                },
                new Project {
                    Title = "Shadows of Eldoria",
                    Slug = "shadows-of-eldoria",
                    ShortDescription = "Пошаговая тактическая RPG",
                    FullDescription = """
                        ## О проекте
                        Тактический рогалик в тёмном фэнтези с пошаговыми боями.

                        ### Ключевые механики
                        - Стихийные заклинания с комбинированием
                        - Процедурно генерируемые подземелья
                        - Система морали и репутации

                        ### Особенности
                        Использую **UE5** с C++ и Blueprints. Оптимизировано под мобильные устройства.
                        """,
                    Genre = "Tactical RPG",
                    Role = "Game Designer & Developer",
                    TechStack = "Unreal Engine 5, C++",
                    ReleaseDate = new DateTime(2025, 6, 18),
                    IsFeatured = true,
                    Tags = new List<Tag> { tags[2] }
                },
                new Project {
                    Title = "Pixel Dungeon",
                    Slug = "pixel-dungeon",
                    ShortDescription = "Пиксельный рогалик с процедурной генерацией",
                    FullDescription = """
                        ## О проекте
                        Пиксельный рогалик с процедурной генерацией подземелий.

                        ### Реализовано
                        - Генерация уровней через клеточные автоматы
                        - Система предметов и лут-таблиц
                        - Мобильное управление

                        ### Особенности
                        Написан на **Godot 4** с GDScript. Графика — пиксель-арт.
                        """,
                    Genre = "Roguelike",
                    Role = "Solo Developer",
                    TechStack = "Godot, GDScript",
                    ReleaseDate = new DateTime(2025, 5, 25),
                    IsFeatured = false,
                    Tags = new List<Tag> { tags[0], tags[1] }
                });

            // Навыки. IsFeatured=true попадают на главную в блок «Топ навыков».
            db.Skills.AddRange(
                new Skill { Name = "C#", Category = "Программирование", Level = 5, YearsOfExperience = 4, IsFeatured = true },
                new Skill { Name = "Unity", Category = "Инструменты", Level = 5, YearsOfExperience = 4, IsFeatured = true },
                new Skill { Name = "Unreal Engine", Category = "Инструменты", Level = 3, YearsOfExperience = 1 },
                new Skill { Name = "Godot", Category = "Инструменты", Level = 4, YearsOfExperience = 2 },
                new Skill { Name = "Shader Graph", Category = "Графика", Level = 3, YearsOfExperience = 2, IsFeatured = true },
                new Skill { Name = "Git / Git LFS", Category = "Инструменты", Level = 4, YearsOfExperience = 4 },
                new Skill { Name = "Zenject / DI", Category = "Программирование", Level = 4, YearsOfExperience = 3 },
                new Skill { Name = "UniTask", Category = "Программирование", Level = 3, YearsOfExperience = 2 },
                new Skill { Name = "Blender", Category = "Графика", Level = 2, YearsOfExperience = 1 },
                new Skill { Name = "Game Design", Category = "Дизайн", Level = 4, YearsOfExperience = 5, IsFeatured = true });

            // Опыт. Текущая работа — IsCurrent=true, EndDate=null.
            db.Experiences.AddRange(
                new Experience {
                    Company = "Game Studio Alpha",
                    Position = "Middle Unity Developer",
                    EmploymentType = "Full-time",
                    Location = "Remote",
                    StartDate = DateTime.UtcNow.AddYears(-2),
                    IsCurrent = true,
                    Description = "Разработка боевой системы и механик.",
                    Highlights = new List<string> { "Оптимизировал Draw Calls на 30%" },
                    TechStack = "Unity, C#, Zenject"
                },
                new Experience {
                    Company = "Indie Studio Beta",
                    Position = "Junior Unity Developer",
                    EmploymentType = "Full-time",
                    Location = "Москва",
                    StartDate = DateTime.UtcNow.AddYears(-4),
                    EndDate = DateTime.UtcNow.AddYears(-2),
                    Description = "Разработка мобильных казуальных игр.",
                    Highlights = new List<string>
                    {
                        "Реализовал 5 мини-игр в одном проекте",
                        "Участвовал в оптимизации под слабые Android-устройства"
                    },
                    TechStack = "Unity, C#, DOTween"
                },
                new Experience {
                    Company = "Freelance",
                    Position = "Game Developer",
                    EmploymentType = "Contract",
                    Location = "Remote",
                    StartDate = DateTime.UtcNow.AddYears(-5),
                    EndDate = DateTime.UtcNow.AddYears(-4),
                    Description = "Разработка прототипов на заказ.",
                    Highlights = new List<string> { "Сделал 12+ прототипов для разных заказчиков" },
                    TechStack = "Unity, Godot"
                });

            // Статьи. IsPublished=true и PublishedAt в прошлом → видны на /Blog.
            db.BlogPosts.AddRange(
                new BlogPost {
                    Title = "Как я делаю процедурную генерацию",
                    Slug = "procedural-generation",
                    Summary = "Краткий разбор подхода к генерации уровней.",
                    Content = "## Введение\nТут будет статья...",
                    PublishedAt = DateTime.UtcNow.AddDays(-7),
                    IsPublished = true,
                    ReadTimeMinutes = 5,
                    Tags = new List<Tag> { tags[0] }
                },
                new BlogPost {
                    Title = "DI в Unity: зачем и как",
                    Slug = "di-in-unity",
                    Summary = "Разбираю Zenject и VContainer на примерах.",
                    Content = "## Введение\nКогда проект растёт, ручная подвязка зависимостей становится болью...",
                    PublishedAt = DateTime.UtcNow.AddDays(-14),
                    IsPublished = true,
                    ReadTimeMinutes = 8,
                    Tags = new List<Tag> { tags[0], tags[1] }
                });

            // Один SaveChanges на всё — EF выполнит вставки в правильном порядке
            // (Tags → Projects → ProjectTags → ...) в одной транзакции.
            await db.SaveChangesAsync();
        }
    }
}