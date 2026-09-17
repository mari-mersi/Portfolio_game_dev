using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Models;
using Portfolio_game_dev.Services;

namespace Portfolio_game_dev.Controllers {
    /// <summary>
    /// Просмотр и скачивание PDF-резюме
    /// </summary>
    public class ResumeController : Controller {
        private readonly IPdfResumeService _pdfResumeService;

        public ResumeController(IPdfResumeService pdfResumeService) {
            _pdfResumeService = pdfResumeService;
        }

        /// <summary>
        /// Главная страница резюме с веб-просмотром и кнопкой скачивания
        /// </summary>
        [HttpGet("resume")]
        public IActionResult Index() {
            // СОЗДАЕМ ИЛИ ПОЛУЧАЕМ МОДЕЛЬ (не передавайте null!)
            var viewModel = GetResumeData();

            // Передаем экземпляр модели в View
            return View(viewModel);
        }

        /// <summary>
        /// Скачивание PDF-файла резюме
        /// </summary>
        [HttpGet("resume/download")]
        public async Task<IActionResult> Download() {
            byte[] pdfBytes = await _pdfResumeService.GeneratePdfAsync();
            string fileName = "Resume_GameDev.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        // Временный метод для наполнения данных (или берите их из DB/Service)
        private ResumeViewModel GetResumeData() {
            return new ResumeViewModel {
                FullName = "Иван Иванов",
                Title = "Middle Unity / Game Developer",
                Email = "developer@example.com",
                Phone = "+7 (999) 000-00-00",
                Location = "Москва, Россия (Remote)",
                GitHubUrl = "https://github.com/example",
                TelegramUrl = "https://t.me/example",
                Summary = "Геймдевелопер с опытом коммерческой разработки проектов на Unity/C#.",
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
                        Description = "Разработка боевой системы и механик.",
                        Highlights = new List<string> { "Оптимизировал Draw Calls на 30%" },
                        TechStack = "Unity, C#, Zenject"
                    }
                },
                Skills = new List<Skill>
                {
                    new Skill { Name = "C#", Category = "Programming", Level = 5 },
                    new Skill { Name = "Unity", Category = "Tools", Level = 5 }
                }
            };
        }
    }
}