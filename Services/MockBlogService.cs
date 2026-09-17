using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Services {
    public class MockBlogService : IBlogService {
        private static readonly List<BlogPost> _posts = new()
        {
            new BlogPost
            {
                Id = 1,
                Title = "Как я проектирую core loop в прототипе",
                Slug = "core-loop-prototyping",
                Summary = "Короткий гайд: с чего начать, какие вопросы задать, когда остановиться и показать игру людям.",
                PublishedAt = new DateTime(2025, 4, 10),
                Tags = new() { "Game Design", "Prototyping" },
                Content = """
                # Как я проектирую core loop в прототипе

                Core loop — это то, что игрок делает снова и снова. Если он не цепляет за первые 30 секунд,
                дальше можно не считать баланс.

                ## Чек-лист перед стартом

                1. **Одно действие.** Что игрок делает 80% времени?
                2. **Одна награда.** Что он получает за это действие?
                3. **Одна причина продолжать.** Что меняется после награды?

                ## Как выглядит мой прототип

                Обычно это серый куб, кубик-цель и счётчик. Никаких моделей, никаких звуков.
                Задача — за вечер проверить, интересно ли вообще.

                > Если скучно на кубах — на красивых моделях тоже будет скучно.

                ## Когда останавливаться

                Как только плейтестер задаёт вопрос «а что дальше?» — вы на верном пути.
                Как только он молча ставит геймпад — стоп, идите переделывать.
                """
            },
            new BlogPost
            {
                Id = 2,
                Title = "Левел-дизайн: три уровня на одну идею",
                Slug = "three-levels-one-idea",
                Summary = "Почему один уровень — это всегда мало, и как строить обучающую прогрессию.",
                PublishedAt = new DateTime(2025, 2, 22),
                Tags = new() { "Level Design", "Tutorial" },
                Content = """
                # Левел-дизайн: три уровня на одну идею

                Классическая ошибка новичка — объяснить механику один раз и идти дальше.
                Игрок не запомнит. Нужно три касания.

                ## Схема

                - **Уровень 1.** Механика без риска. Игрок знакомится.
                - **Уровень 2.** Механика + небольшое давление. Игрок применяет.
                - **Уровень 3.** Механика + комбинация с другой. Игрок экспериментирует.

                ## Пример

                Если механика — «прыжок от стены», то:

                1. Стена одна, пропасть узкая.
                2. Две стены, пропасть шире, появляется таймер.
                3. Стена + движущаяся платформа + враг, которого нельзя бить.

                Просто, но работает.
                """
            },
            new BlogPost
            {
                Id = 3,
                Title = "Godot vs Unity для прототипов в 2025",
                Slug = "godot-vs-unity-2025",
                Summary = "Субъективный разбор: что быстрее для геймдизайнера, который пишет код сам.",
                PublishedAt = new DateTime(2025, 1, 5),
                Tags = new() { "Tools", "Godot", "Unity" },
                Content = """
                # Godot vs Unity для прототипов в 2025

                Коротко: **Godot — быстрее стартовать, Unity — больше готовых ассетов.**

                ## Когда Godot

                - Нужен прототип за вечер.
                - Проект 2D или стилизованный 3D.
                - Хочется лёгкий редактор и быстрые сцены.

                ## Когда Unity

                - Нужны Asset Store-паки и сторонние SDK.
                - Целевые платформы — консоли.
                - В команде уже есть люди с опытом Unity.

                ## Мой вывод

                Для геймдизайнера, который сам делает прототипы, **Godot выигрывает по скорости**.
                Для продакшена с командой из 5+ человек чаще выигрывает Unity.
                """
            }
        };

        public Task<List<BlogPost>> GetPublishedAsync(int page = 1, int pageSize = 6) {
            var items = _posts
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult(items);
        }

        public Task<int> GetPublishedCountAsync() =>
            Task.FromResult(_posts.Count(p => p.IsPublished));

        public Task<BlogPost?> GetBySlugAsync(string slug) =>
            Task.FromResult(_posts.FirstOrDefault(p =>
                p.IsPublished &&
                string.Equals(p.Slug, slug, StringComparison.OrdinalIgnoreCase)));

        public Task<List<BlogPost>> GetRelatedAsync(BlogPost post, int count = 3) {
            // Простая эвристика: сначала по общим тегам, добираем остальными.
            var byTags = _posts
                .Where(p => p.Id != post.Id && p.IsPublished &&
                            p.Tags.Intersect(post.Tags, StringComparer.OrdinalIgnoreCase).Any())
                .ToList();

            if (byTags.Count < count) {
                byTags.AddRange(_posts
                    .Where(p => p.Id != post.Id && p.IsPublished && !byTags.Contains(p))
                    .Take(count - byTags.Count));
            }

            return Task.FromResult(byTags.Take(count).ToList());
        }
    }
}