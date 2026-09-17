using System.ComponentModel.DataAnnotations;

namespace Portfolio_game_dev.ViewModels {
    public class ContactViewModel {
        [Required(ErrorMessage = "Укажите имя")]
        [StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите e-mail")]
        [EmailAddress(ErrorMessage = "Некорректный e-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите сообщение")]
        [StringLength(2000, MinimumLength = 10)]
        public string Message { get; set; } = string.Empty;

        /// <summary>Honeypot-поле для отсечения ботов.</summary>
        public string? Website { get; set; }
    }
}