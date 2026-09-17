namespace Portfolio_game_dev.ViewModels {
    public class ErrorViewModel {
        public string? RequestId { get; set; }

        /// <summary>Показывать ли RequestId пользователю.</summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}