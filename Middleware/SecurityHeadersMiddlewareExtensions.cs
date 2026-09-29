namespace Portfolio_game_dev.Middleware;

public static class SecurityHeadersMiddlewareExtensions {
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        => app.UseMiddleware<SecurityHeadersMiddleware>();
}