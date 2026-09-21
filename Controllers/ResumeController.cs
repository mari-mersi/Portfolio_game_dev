using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Services.Abstractions;

namespace Portfolio_game_dev.Controllers;

/// <summary>
/// Просмотр и скачивание PDF-резюме.
/// </summary>
public class ResumeController : Controller {
    private readonly IPdfResumeService _resume;

    public ResumeController(IPdfResumeService resume) {
        _resume = resume;
    }

    /// <summary>
    /// GET: /resume — веб-превью резюме с кнопкой «Скачать PDF».
    /// </summary>
    [HttpGet("resume")]
    public async Task<IActionResult> Index(CancellationToken ct = default) {
        var vm = await _resume.BuildViewModelAsync(ct);

        ViewData["Title"] = "Резюме";
        ViewData["Description"] = "Интерактивное резюме: опыт, навыки, избранные проекты.";
        return View(vm);
    }

    /// <summary>
    /// GET: /resume/download — отдаёт готовый PDF.
    /// </summary>
    [HttpGet("resume/download")]
    public async Task<IActionResult> Download(CancellationToken ct = default) {
        var bytes = await _resume.GeneratePdfAsync(ct);
        return File(bytes, "application/pdf", "Resume.pdf");
    }
}