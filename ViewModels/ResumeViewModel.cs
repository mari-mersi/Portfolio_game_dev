namespace Portfolio_game_dev.ViewModels;

/// <summary>
/// ViewModel резюме — используется и для веб-превью, и для PDF.
/// </summary>
public class ResumeViewModel {
    // ── Профиль
    public string FullName { get; set; } = "Ваше Имя";
    public string Title { get; set; } = "Game Developer / Game Designer";
    public string Email { get; set; } = "your@email.com";
    public string Phone { get; set; } = "+7 (999) 000-00-00";
    public string Location { get; set; } = "Москва, Россия (Remote)";
    public string GitHubUrl { get; set; } = "";
    public string TelegramUrl { get; set; } = "";
    public string Summary { get; set; } = "";

    // ── Разделы
    public List<Models.Experience> Experiences { get; set; } = new();
    public List<Models.Skill> Skills { get; set; } = new();
    public List<Models.Project> Projects { get; set; } = new();
}