using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.Areas.Admin.ViewModels;

public class LoginViewModel {
    [Required(ErrorMessage = "Укажите логин или email")]
    [EmailAddress(ErrorMessage = "Некорректный логин/email")]
    [Display(Name = "Логин / Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Запомнить меня")]
    public bool RememberMe { get; set; }
}