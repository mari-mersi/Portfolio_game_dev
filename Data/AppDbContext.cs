using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Portfolio_game_dev.Models;

namespace Portfolio_game_dev.Data;

/// <summary>
/// Контекст EF Core. Единая точка доступа к БД для всего проекта.
/// Наследует IdentityDbContext — Identity использует те же таблицы (AspNetUsers и т.д.).
/// </summary>
public class AppDbContext : IdentityDbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── DbSet'ы: по одному на каждую агрегатную сущность ──────────────
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMedia> ProjectMedia => Set<ProjectMedia>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    /// <summary>
    /// Здесь описываем связи, индексы и конвертеры, которые нельзя (или неудобно)
    /// выразить через Data Annotations на моделях.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // ВАЖНО: сначала Identity, потом свои конфигурации.
        // Иначе Identity может перезаписать наши настройки для AspNet*-таблиц.
        base.OnModelCreating(modelBuilder);

        // ── Project ─────────────────────────────────────────────────
        modelBuilder.Entity<Project>(e => {
            e.HasIndex(p => p.Slug).IsUnique();     // /Projects/{slug} — slug должен быть уникален
            e.Property(p => p.Title).IsRequired().HasMaxLength(200);
            e.Property(p => p.Slug).IsRequired().HasMaxLength(200);

            // M2M: Project <-> Tag через join-таблицу ProjectTags.
            // Явно описываем имена FK и составной PK, чтобы миграция была предсказуемой.
            // Cascade на join-таблице: удалили проект — ушли связи. Сами Tag остаются жить
            // (они могут использоваться в других проектах и в статьях блога).
            e.HasMany(p => p.Tags)
             .WithMany(t => t.Projects)
             .UsingEntity<Dictionary<string, object>>(
                 "ProjectTag",
                 j => j.HasOne<Tag>()
                       .WithMany()
                       .HasForeignKey("TagId")
                       .OnDelete(DeleteBehavior.Cascade),
                 j => j.HasOne<Project>()
                       .WithMany()
                       .HasForeignKey("ProjectId")
                       .OnDelete(DeleteBehavior.Cascade),
                 j => {
                     j.HasKey("ProjectId", "TagId");
                     j.ToTable("ProjectTags");
                 });
        });

        // ── ProjectMedia (1:M, каскад) ──────────────────────────────
        // Медиа — дочерняя сущность. Без проекта жить не может, поэтому Cascade:
        // удалили Project — убрались все его ProjectMedia.
        modelBuilder.Entity<ProjectMedia>(e => {
            e.HasOne(m => m.Project)
             .WithMany(p => p.Media)
             .HasForeignKey(m => m.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);

            e.Property(m => m.Url).IsRequired().HasMaxLength(300);

            // Enum храним как int (0 — Image, 1 — Video). Так проще фильтровать и сортировать.
            e.Property(m => m.Type).HasConversion<int>();
        });

        // ── Tag ─────────────────────────────────────────────────────
        modelBuilder.Entity<Tag>(e => {
            e.HasIndex(t => t.Slug).IsUnique();    // /Projects?tag=unity — slug уникален
            e.Property(t => t.Name).IsRequired().HasMaxLength(50);
            e.Property(t => t.Slug).IsRequired().HasMaxLength(50);
        });

        // ── BlogPost ────────────────────────────────────────────────
        modelBuilder.Entity<BlogPost>(e => {
            e.HasIndex(p => p.Slug).IsUnique();                          // /Blog/{slug}
            e.HasIndex(p => new { p.IsPublished, p.PublishedAt });       // для «опубликованные, свежие»

            e.Property(p => p.Title).IsRequired().HasMaxLength(200);
            e.Property(p => p.Slug).IsRequired().HasMaxLength(200);

            // M2M: BlogPost <-> Tag. Логика та же, что у Project↔Tag.
            e.HasMany(p => p.Tags)
             .WithMany(t => t.BlogPosts)
             .UsingEntity<Dictionary<string, object>>(
                 "BlogPostTag",
                 j => j.HasOne<Tag>()
                       .WithMany()
                       .HasForeignKey("TagId")
                       .OnDelete(DeleteBehavior.Cascade),
                 j => j.HasOne<BlogPost>()
                       .WithMany()
                       .HasForeignKey("BlogPostId")
                       .OnDelete(DeleteBehavior.Cascade),
                 j => {
                     j.HasKey("BlogPostId", "TagId");
                     j.ToTable("BlogPostTags");
                 });
        });

        // ── Skill ───────────────────────────────────────────────────
        modelBuilder.Entity<Skill>(e => {
            e.Property(s => s.Name).IsRequired().HasMaxLength(100);
            e.Property(s => s.Category).IsRequired().HasMaxLength(50);

            // Составной индекс: группировка по категории + сортировка внутри неё.
            e.HasIndex(s => new { s.Category, s.SortOrder });
        });

        // ── Experience ──────────────────────────────────────────────
        modelBuilder.Entity<Experience>(e => {
            e.Property(x => x.Company).IsRequired().HasMaxLength(150);
            e.Property(x => x.Position).IsRequired().HasMaxLength(150);
            e.HasIndex(x => x.StartDate);           // таймлайн сортируется по StartDate

            // List<string> в SQLite не влезает «как есть».
            // Конвертер превращает список в строку с разделителем '\n' (и обратно).
            // ValueComparer нужен, чтобы EF корректно определял изменения списка
            // при SaveChanges (без него EF может пропустить или лишний раз записать).
            var highlightsComparer = new ValueComparer<List<string>>(
                (a, b) => a!.SequenceEqual(b!),
                v => v.Aggregate(0, (acc, s) => HashCode.Combine(acc, s.GetHashCode())),
                v => v.ToList());

            e.Property(x => x.Highlights)
             .HasConversion(
                 v => string.Join('\n', v),
                 v => string.IsNullOrWhiteSpace(v)
                         ? new List<string>()
                         : v.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList())
             .Metadata.SetValueComparer(highlightsComparer);
        });

        // ── ContactMessage ──────────────────────────────────────────
        modelBuilder.Entity<ContactMessage>(e => {
            e.Property(m => m.Name).IsRequired().HasMaxLength(100);
            e.Property(m => m.Email).IsRequired().HasMaxLength(150);
            e.Property(m => m.Message).IsRequired().HasMaxLength(2000);

            // В админке фильтр «непрочитанные, свежие».
            e.HasIndex(m => new { m.IsRead, m.CreatedAt });
        });
    }
}