using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Data;

namespace Portfolio_game_dev.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ContactMessagesController : Controller {
    private readonly AppDbContext _db;

    public ContactMessagesController(AppDbContext db) {
        _db = db;
    }

    // ── Index ────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Index(string? status, CancellationToken ct) {
        var query = _db.ContactMessages.AsNoTracking().AsQueryable();

        if (status == "unread")
            query = query.Where(m => !m.IsRead);
        else if (status == "read")
            query = query.Where(m => m.IsRead);

        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(ct);

        ViewData["Title"] = "Сообщения";
        ViewData["StatusFilter"] = status;
        return View(messages);
    }

    // ── Details ──────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct) {
        var message = await _db.ContactMessages.FindAsync(new object[] { id }, ct);
        if (message is null)
            return NotFound();

        // Автоматически помечаем как прочитанное
        if (!message.IsRead) {
            message.IsRead = true;
            await _db.SaveChangesAsync(ct);
        }

        ViewData["Title"] = $"Сообщение от {message.Name}";
        return View(message);
    }

    // ── MarkAsRead / MarkAsUnread ────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleRead(int id, string? returnUrl, CancellationToken ct) {
        var message = await _db.ContactMessages.FindAsync(new object[] { id }, ct);
        if (message is null)
            return NotFound();

        message.IsRead = !message.IsRead;
        await _db.SaveChangesAsync(ct);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    // ── Delete ───────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) {
        var message = await _db.ContactMessages.FindAsync(new object[] { id }, ct);
        if (message is null)
            return NotFound();

        _db.ContactMessages.Remove(message);
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"Сообщение от {message.Name} удалено.";
        return RedirectToAction(nameof(Index));
    }
}