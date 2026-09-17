using System.Collections.Generic;

namespace Portfolio_game_dev.Models {
    public class ResumeViewModel {
        public string FullName { get; set; } = "Иван Иванов";
        public string Title { get; set; } = "Game Developer / Game Designer";
        public string Email { get; set; } = "developer@example.com";
        public string Phone { get; set; } = "+7 (999) 000-00-00";
        public string Location { get; set; } = "Москва, Россия (Remote)";
        public string GitHubUrl { get; set; } = "https://github.com/example";
        public string TelegramUrl { get; set; } = "https://t.me/example";
        public string Summary { get; set; } = "Разработчик игр с опытом создания систем на Unity (C#) и Unreal Engine. Специализируюсь на геймдизайне, архитектуре игровых систем и оптимизации.";

        public List<Experience> Experiences { get; set; } = new();
        public List<Skill> Skills { get; set; } = new();
    }
}