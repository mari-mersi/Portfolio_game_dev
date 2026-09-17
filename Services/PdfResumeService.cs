using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Portfolio_game_dev.Models;

// Избавляемся от конфликта между QuestPDF.Fluent.Document и System.Reflection.Metadata.Document
using Document = QuestPDF.Fluent.Document;

namespace Portfolio_game_dev.Services {
    public class PdfResumeService : IPdfResumeService {
        public PdfResumeService() {
            // Бесплатная лицензия QuestPDF для физических лиц и малого бизнеса
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GeneratePdfAsync() {
            // В продакшене данные берутся из БД (например, через DbContext)
            var model = GetMockResumeData();

            var document = Document.Create(container => {
                container.Page(page => {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Lato));

                    page.Header().Element(c => ComposeHeader(c, model));
                    page.Content().Element(c => ComposeContent(c, model));
                    page.Footer().Element(ComposeFooter);
                });
            });

            return await Task.FromResult(document.GeneratePdf());
        }

        private void ComposeHeader(IContainer container, ResumeViewModel model) {
            container.Column(col => {
                col.Item().Text(model.FullName).Bold().FontSize(22).FontColor(Colors.Blue.Darken3);
                col.Item().Text(model.Title).FontSize(13).FontColor(Colors.Grey.Darken2);

                col.Item().PaddingTop(6).Row(row => {
                    row.RelativeItem().Text($"Email: {model.Email}");
                    row.RelativeItem().Text($"Телефон: {model.Phone}");
                    row.RelativeItem().Text($"Локация: {model.Location}");
                });

                col.Item().PaddingTop(2).Row(row => {
                    if (!string.IsNullOrEmpty(model.GitHubUrl))
                        row.RelativeItem().Text($"GitHub: {model.GitHubUrl}");

                    if (!string.IsNullOrEmpty(model.TelegramUrl))
                        row.RelativeItem().Text($"Telegram: {model.TelegramUrl}");
                });

                col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
            });
        }

        private void ComposeContent(IContainer container, ResumeViewModel model) {
            container.PaddingTop(10).Column(col => {
                // О себе
                if (!string.IsNullOrEmpty(model.Summary)) {
                    col.Item().Text("О себе").Bold().FontSize(13).FontColor(Colors.Blue.Darken3);
                    col.Item().PaddingTop(3).Text(model.Summary);
                    col.Item().PaddingVertical(8).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                }

                // Опыт работы
                if (model.Experiences.Any()) {
                    col.Item().Text("Опыт работы").Bold().FontSize(13).FontColor(Colors.Blue.Darken3);

                    foreach (var exp in model.Experiences) {
                        col.Item().PaddingTop(6).Column(item => {
                            item.Item().Row(r => {
                                r.RelativeItem().Text($"{exp.Position} — {exp.Company}").Bold().FontSize(10.5f);
                                r.ConstantItem(150).AlignRight().Text($"{exp.Period} ({exp.DurationText})").FontSize(9).FontColor(Colors.Grey.Darken1);
                            });

                            item.Item().Text($"{exp.EmploymentType} | {exp.Location}").FontSize(8.5f).Italic().FontColor(Colors.Grey.Darken1);

                            if (!string.IsNullOrEmpty(exp.Description)) {
                                item.Item().PaddingTop(2).Text(exp.Description);
                            }

                            foreach (var highlight in exp.Highlights) {
                                item.Item().Text($"• {highlight}").FontSize(9);
                            }

                            if (!string.IsNullOrEmpty(exp.TechStack)) {
                                item.Item().PaddingTop(2).Text($"Стек: {exp.TechStack}").FontSize(8.5f).FontColor(Colors.Grey.Darken2);
                            }
                        });
                    }

                    col.Item().PaddingVertical(8).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                }

                // Навыки
                if (model.Skills.Any()) {
                    col.Item().Text("Навыки и технологии").Bold().FontSize(13).FontColor(Colors.Blue.Darken3);

                    var groupedSkills = model.Skills.GroupBy(s => s.Category);
                    foreach (var group in groupedSkills) {
                        var skillLine = string.Join(", ", group.Select(s => $"{s.Name} ({s.Level}/5)"));
                        col.Item().PaddingTop(3).Text(t => {
                            t.Span($"{group.Key}: ").Bold();
                            t.Span(skillLine);
                        });
                    }
                }
            });
        }

        private void ComposeFooter(IContainer container) {
            container.AlignCenter().Text(x => {
                x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Darken1);
                x.Span(" / ").FontSize(8).FontColor(Colors.Grey.Darken1);
                x.TotalPages().FontSize(8).FontColor(Colors.Grey.Darken1);
            });
        }

        private ResumeViewModel GetMockResumeData() {
            return new ResumeViewModel {
                FullName = "Иван Иванов",
                Title = "Middle Unity / Game Developer",
                Email = "developer@example.com",
                Phone = "+7 (999) 000-00-00",
                Location = "Москва, Россия (Remote)",
                GitHubUrl = "https://github.com/example",
                TelegramUrl = "https://t.me/example",
                Summary = "Геймдевелопер с опытом коммерческой разработки мобильных и ПК проектов на Unity/C#. Специализируюсь на игровой архитектуре, анимационных системах и оптимизации.",
                Experiences = new List<Experience>
                {
                    new Experience
                    {
                        Company = "Game Studio Alpha",
                        Position = "Middle Unity Developer",
                        EmploymentType = "Full-time",
                        Location = "Remote",
                        StartDate = DateTime.UtcNow.AddYears(-2),
                        EndDate = null,
                        Description = "Разработка боевой системы, инвентаря и сетевого взаимодействия.",
                        Highlights = new List<string>
                        {
                            "Оптимизировал производительность графического пайплайна (Draw Calls снижены на 30%).",
                            "Спроектировал гибкую систему абилок и заклинаний на базе ScriptableObjects."
                        },
                        TechStack = "Unity, C#, UniTask, Zenject, Git"
                    }
                },
                Skills = new List<Skill>
                {
                    new Skill { Name = "C#", Category = "Programming", Level = 5 },
                    new Skill { Name = "Unity", Category = "Tools", Level = 5 },
                    new Skill { Name = "Shader Graph", Category = "Graphics", Level = 3 },
                    new Skill { Name = "Git / Git LFS", Category = "Tools", Level = 4 }
                }
            };
        }
    }
}