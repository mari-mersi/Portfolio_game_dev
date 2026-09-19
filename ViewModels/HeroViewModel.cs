namespace Portfolio_game_dev.ViewModels {
    /// <summary>
    /// Данные hero-секции главной. Отдельный класс, чтобы не плодить Hero*-свойства
    /// в HomeViewModel и переиспользовать, если hero понадобится на других страницах.
    /// </summary>
    public class HeroViewModel {
        /// <summary>Короткий слоган (1 строка).</summary>
        public string Tagline { get; set; } = string.Empty;

        /// <summary>Пояснение под слоганом (1–2 предложения).</summary>
        public string Subline { get; set; } = string.Empty;

        /// <summary>Текст кнопки «Смотреть проекты» (или другой CTA).</summary>
        public string PrimaryCtaText { get; set; } = "Смотреть проекты";

        /// <summary>Куда ведёт primary CTA (обычно /Projects).</summary>
        public string PrimaryCtaUrl { get; set; } = "/Projects";

        /// <summary>Текст второй кнопки (например, «Скачать резюме»).</summary>
        public string SecondaryCtaText { get; set; } = "Скачать резюме";

        /// <summary>Куда ведёт secondary CTA (обычно /Resume/Download).</summary>
        public string SecondaryCtaUrl { get; set; } = "/Resume/Download";
    }
}
