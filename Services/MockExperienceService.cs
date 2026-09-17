using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Services {
    public class MockExperienceService : IExperienceService {
        private static readonly List<Experience> _items = new()
        {
            new Experience
            {
                Id = 1,
                Company = "Pixel Forge Studio",
                Position = "Game Designer",
                EmploymentType = "Full-time",
                Location = "Remote",
                StartDate = new DateTime(2023, 6, 1),
                EndDate = null,
                Description = "Отвечаю за core loop и экономику в мобильном roguelike-проекте. " +
                              "Веду документацию, провожу плейтесты и итерации по фидбэку.",
                Highlights = new()
                {
                    "Переработал core loop — retention D7 вырос с 18% до 31%.",
                    "Спроектировал систему прогрессии на 40+ часов геймплея.",
                    "Провёл 30+ плейтестов, выстроил регулярный цикл итераций."
                },
                TechStack = "Unity, C#, Figma, Notion, Miro"
            },
            new Experience
            {
                Id = 2,
                Company = "Indie Team «Paperworks»",
                Position = "Level Designer",
                EmploymentType = "Contract",
                Location = "Remote",
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2023, 5, 1),
                Description = "Проектировал уровни для пошаговой тактики, строил блокинги и " +
                              "итерировал на основе телеметрии.",
                Highlights = new()
                {
                    "Сделал 18 уровней кампании и 6 обучающих.",
                    "Снизил долю отвалов на 3-м уровне с 42% до 19%.",
                    "Собрал внутренний гайд по блокингу для команды."
                },
                TechStack = "Godot, Blender, Figma, Trello"
            },
            new Experience
            {
                Id = 3,
                Company = "Freelance",
                Position = "Game Designer / Prototyper",
                EmploymentType = "Freelance",
                Location = "Remote",
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2021, 12, 31),
                Description = "Делал быстрые прототипы для инди-команд и студий, писал GDD " +
                              "и настраивал баланс.",
                Highlights = new()
                {
                    "12 прототипов за год, 3 из них выросли в полноценные проекты.",
                    "Написал GDD для двух мобильных казуалок."
                },
                TechStack = "Unity, Godot, GDScript"
            }
        };

        public Task<List<Experience>> GetAllAsync() =>
            Task.FromResult(_items
                .OrderByDescending(e => e.StartDate)
                .ToList());

        public Task<List<Experience>> GetRecentAsync(int count = 3) =>
            Task.FromResult(_items
                .OrderByDescending(e => e.StartDate)
                .Take(count)
                .ToList());
    }
}