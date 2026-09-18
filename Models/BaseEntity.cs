namespace Portfolio_game_dev.Models;

/// <summary>
/// Базовая сущность. Все entity проекта наследуются от неё.
/// Даёт единый первичный ключ и аудит-поля — не надо дублировать в каждой таблице.
/// </summary>
public abstract class BaseEntity {
    /// <summary>Первичный ключ. EF по соглашению делает его auto-increment (INTEGER в SQLite).</summary>
    public int Id { get; set; }

    /// <summary>Момент создания записи (UTC). Ставится автоматически при создании объекта.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Момент последнего обновления (UTC). null — запись ещё не редактировали.</summary>
    /// <remarks>
    /// Автоматически не обновляется. Ставь вручную в сервисах или через override SaveChangesAsync в AppDbContext.
    /// </remarks>
    public DateTime? UpdatedAt { get; set; }
}