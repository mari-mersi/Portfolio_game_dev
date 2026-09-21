using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.ViewModels;
using Portfolio_game_dev.Services.Abstractions;

// Конфликт имён: QuestPDF.Fluent.Document vs System.Reflection.Metadata.Document
using Document = QuestPDF.Fluent.Document;

namespace Portfolio_game_dev.Services.Implementations;

/// <summary>
/// Генерация PDF-резюме через QuestPDF. Данные — из AppDbContext.
/// </summary>
public class PdfResumeService : IPdfResumeService {
    // Имя шрифта, зарегистрированного в wwwroot/fonts/
    private const string FontFamily = "Inter 18pt";

    private readonly AppDbContext _db;

    public PdfResumeService(AppDbContext db) {
        _db = db;
    }

    // ────────────────────────────────────────────────────────────
    //  Публичные методы
    // ────────────────────────────────────────────────────────────

    public async Task<byte[]> GeneratePdfAsync(CancellationToken ct = default) {
        var model = await BuildViewModelAsync(ct);

        var document = Document.Create(container => {
            container.Page(page => {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(FontFamily));

                page.Header().Element(c => ComposeHeader(c, model));
                page.Content().Element(c => ComposeContent(c, model));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    public async Task<ResumeViewModel> BuildViewModelAsync(CancellationToken ct = default) {
        // Опыт: свежие сверху
        var experiences = await _db.Experiences
            .AsNoTracking()
            .OrderByDescending(e => e.StartDate)
            .ToListAsync(ct);

        // Навыки: все, сгруппируем во вьюхе
        var skills = await _db.Skills
            .AsNoTracking()
            .OrderBy(s => s.Category)
            .ThenBy(s => s.SortOrder)
            .ToListAsync(ct);

        // Проекты: featured, до 4 штук
        var projects = await _db.Projects
            .AsNoTracking()
            .Where(p => p.IsPublished && p.IsFeatured)
            .OrderBy(p => p.SortOrder)
            .Take(4)
            .ToListAsync(ct);

        return new ResumeViewModel {
            // ── Профиль (хардкод на старте; позже вынести в appsettings или отдельную таблицу)
            FullName = "Ваше Имя",
            Title = "Game Developer / Unity Developer",
            Email = "your@email.com",
            Phone = "+7 (999) 000-00-00",
            Location = "Москва, Россия (Remote)",
            GitHubUrl = "https://github.com/your-username",
            TelegramUrl = "https://t.me/your-username",
            Summary = "Game-разработчик с опытом создания игровых систем на Unity (C#) " +
                      "и Unreal Engine. Специализируюсь на архитектуре, оптимизации и геймдизайне.",

            Experiences = experiences,
            Skills = skills,
            Projects = projects
        };
    }

    // ────────────────────────────────────────────────────────────
    //  Header
    // ────────────────────────────────────────────────────────────

    private void ComposeHeader(IContainer container, ResumeViewModel model) {
        container.Column(col => {
            col.Item().Text(model.FullName).Bold().FontSize(22).FontColor(Colors.Blue.Darken3);
            col.Item().Text(model.Title).FontSize(12).FontColor(Colors.Grey.Darken2);

            col.Item().PaddingTop(6).Row(row => {
                if (!string.IsNullOrEmpty(model.Email))
                    row.RelativeItem().Text($"Email: {model.Email}").FontSize(9);
                if (!string.IsNullOrEmpty(model.Phone))
                    row.RelativeItem().Text($"Телефон: {model.Phone}").FontSize(9);
                if (!string.IsNullOrEmpty(model.Location))
                    row.RelativeItem().Text($"Локация: {model.Location}").FontSize(9);
            });

            col.Item().PaddingTop(2).Row(row => {
                if (!string.IsNullOrEmpty(model.GitHubUrl))
                    row.RelativeItem().Text($"GitHub: {model.GitHubUrl}").FontSize(9);
                if (!string.IsNullOrEmpty(model.TelegramUrl))
                    row.RelativeItem().Text($"Telegram: {model.TelegramUrl}").FontSize(9);
            });

            col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
        });
    }

    // ────────────────────────────────────────────────────────────
    //  Content
    // ────────────────────────────────────────────────────────────

    private void ComposeContent(IContainer container, ResumeViewModel model) {
        container.PaddingTop(10).Column(col => {
            // О себе
            if (!string.IsNullOrEmpty(model.Summary)) {
                col.Item().Text("О себе").Bold().FontSize(13).FontColor(Colors.Blue.Darken3);
                col.Item().PaddingTop(3).Text(model.Summary).FontSize(10);
                col.Item().PaddingVertical(8).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
            }

            // Опыт
            if (model.Experiences.Any()) {
                col.Item().Text("Опыт работы").Bold().FontSize(13).FontColor(Colors.Blue.Darken3);

                foreach (var exp in model.Experiences) {
                    col.Item().PaddingTop(6).Column(item => {
                        // Позиция — компания | период
                        item.Item().Row(r => {
                            r.RelativeItem().Text($"{exp.Position} — {exp.Company}")
                                .Bold().FontSize(10.5f);
                            r.ConstantItem(160).AlignRight()
                                .Text($"{exp.Period} ({exp.DurationText})")
                                .FontSize(9).FontColor(Colors.Grey.Darken1);
                        });

                        // Тип занятости | локация
                        var metaLine = string.Join(" · ",
                            new[] { exp.EmploymentType, exp.Location }
                                .Where(s => !string.IsNullOrWhiteSpace(s)));
                        if (!string.IsNullOrEmpty(metaLine))
                            item.Item().Text(metaLine).FontSize(8.5f).Italic()
                                .FontColor(Colors.Grey.Darken1);

                        // Описание
                        if (!string.IsNullOrEmpty(exp.Description))
                            item.Item().PaddingTop(2).Text(exp.Description).FontSize(9.5f);

                        // Достижения
                        foreach (var highlight in exp.Highlights)
                            item.Item().Text($"• {highlight}").FontSize(9);

                        // Стек
                        if (!string.IsNullOrEmpty(exp.TechStack))
                            item.Item().PaddingTop(2)
                                .Text($"Стек: {exp.TechStack}")
                                .FontSize(8.5f).FontColor(Colors.Grey.Darken2);
                    });
                }

                col.Item().PaddingVertical(8).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
            }

            // Навыки (сгруппированы по категориям)
            if (model.Skills.Any()) {
                col.Item().Text("Навыки и технологии").Bold().FontSize(13).FontColor(Colors.Blue.Darken3);

                var grouped = model.Skills.GroupBy(s => s.Category);
                foreach (var group in grouped) {
                    var skillLine = string.Join(", ",
                        group.Select(s => $"{s.Name} ({s.Level}/5)"));

                    col.Item().PaddingTop(3).Text(t => {
                        t.Span($"{group.Key}: ").Bold().FontSize(9.5f);
                        t.Span(skillLine).FontSize(9.5f);
                    });
                }

                col.Item().PaddingVertical(8).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
            }

            // Проекты
            if (model.Projects.Any()) {
                col.Item().Text("Избранные проекты").Bold().FontSize(13).FontColor(Colors.Blue.Darken3);

                foreach (var project in model.Projects) {
                    col.Item().PaddingTop(5).Column(item => {
                        item.Item().Row(r => {
                            r.RelativeItem().Text(project.Title).Bold().FontSize(10.5f);
                            if (project.ReleaseDate.HasValue)
                                r.ConstantItem(100).AlignRight()
                                    .Text(project.ReleaseDate.Value.Year.ToString())
                                    .FontSize(9).FontColor(Colors.Grey.Darken1);
                        });

                        // Жанр / роль
                        var metaLine = string.Join(" · ",
                            new[] { project.Genre, project.Role }
                                .Where(s => !string.IsNullOrWhiteSpace(s)));
                        if (!string.IsNullOrEmpty(metaLine))
                            item.Item().Text(metaLine).FontSize(8.5f).Italic()
                                .FontColor(Colors.Grey.Darken1);

                        // Краткое описание
                        if (!string.IsNullOrEmpty(project.ShortDescription))
                            item.Item().PaddingTop(2).Text(project.ShortDescription).FontSize(9);

                        // Стек
                        if (!string.IsNullOrEmpty(project.TechStack))
                            item.Item().PaddingTop(2)
                                .Text($"Стек: {project.TechStack}")
                                .FontSize(8.5f).FontColor(Colors.Grey.Darken2);
                    });
                }
            }
        });
    }

    // ────────────────────────────────────────────────────────────
    //  Footer
    // ────────────────────────────────────────────────────────────

    private void ComposeFooter(IContainer container) {
        container.AlignCenter().Text(x => {
            x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Darken1);
            x.Span(" / ").FontSize(8).FontColor(Colors.Grey.Darken1);
            x.TotalPages().FontSize(8).FontColor(Colors.Grey.Darken1);
        });
    }
}