namespace Portfolio_game_dev.Middleware;

/// <summary>
/// Добавляет базовые security-заголовки ко всем ответам.
/// </summary>
public class SecurityHeadersMiddleware {
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) {
        var headers = context.Response.Headers;

        // Не даём MIME-sniffing'у переопределить Content-Type
        headers["X-Content-Type-Options"] = "nosniff";

        // Запрещаем встраивать сайт в iframe с других доменов
        headers["X-Frame-Options"] = "SAMEORIGIN";

        // Ограничиваем передачу referrer на внешние сайты
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Отключаем неиспользуемые API браузера
        headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

        await _next(context);
    }
}