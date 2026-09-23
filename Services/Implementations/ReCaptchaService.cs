using System.Text.Json;
using System.Text.Json.Serialization;
using Portfolio_game_dev.Services.Abstractions;

namespace Portfolio_game_dev.Services.Implementations;

/// <summary>
/// Проверка reCAPTCHA v3 через Google siteverify API.
/// </summary>
public class ReCaptchaService : IReCaptchaService {
    private const string VerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<ReCaptchaService> _logger;

    public ReCaptchaService(
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        ILogger<ReCaptchaService> logger) {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }

    public async Task<bool> ValidateAsync(string? token, CancellationToken ct = default) {
        var enabled = _config.GetValue<bool>("ReCaptcha:Enabled");
        if (!enabled) {
            _logger.LogDebug("ReCaptcha отключён (Enabled=false)");
            return true;
        }

        if (string.IsNullOrWhiteSpace(token)) {
            _logger.LogWarning("ReCaptcha включён, но токен не пришёл");
            return false;
        }

        var secret = _config["ReCaptcha:SecretKey"];
        if (string.IsNullOrWhiteSpace(secret)) {
            _logger.LogError("ReCaptcha:SecretKey не задан в конфиге");
            return false;
        }

        try {
            var client = _httpClientFactory.CreateClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", secret),
                new KeyValuePair<string, string>("response", token)
            });

            var response = await client.PostAsync(VerifyUrl, content, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<RecaptchaResponse>(json);

            if (result is null) {
                _logger.LogWarning("ReCaptcha: пустой ответ от Google");
                return false;
            }

            if (!result.Success) {
                _logger.LogWarning("ReCaptcha failed: {Errors}",
                    string.Join(", ", result.ErrorCodes ?? Array.Empty<string>()));
                return false;
            }

            // v3: score от 0.0 (бот) до 1.0 (человек). 0.5 — рекомендуемый порог.
            if (result.Score < 0.5) {
                _logger.LogWarning("ReCaptcha score слишком низкий: {Score}", result.Score);
                return false;
            }

            return true;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Ошибка проверки ReCaptcha");
            // При ошибке сети — не блокируем пользователя (fail-open).
            return true;
        }
    }

    private class RecaptchaResponse {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("action")]
        public string? Action { get; set; }

        [JsonPropertyName("challenge_ts")]
        public DateTimeOffset? ChallengeTs { get; set; }

        [JsonPropertyName("hostname")]
        public string? Hostname { get; set; }

        [JsonPropertyName("error-codes")]
        public string[]? ErrorCodes { get; set; }
    }
}