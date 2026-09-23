using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portfolio_game_dev.Areas.Admin.ViewModels;

namespace Portfolio_game_dev.Areas.Admin.Controllers;

/// <summary>
/// Вход/выход админа через Identity.
/// </summary>
[Area("Admin")]
[AllowAnonymous]   // доступ к логину — для всех (в т.ч. незалогиненных)
public class AccountController : Controller {
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<IdentityUser> signInManager,
        ILogger<AccountController> logger) {
        _signInManager = signInManager;
        _logger = logger;
    }

    // GET: /Admin/Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null) {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    // POST: /Admin/Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null) {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(vm);

        var result = await _signInManager.PasswordSignInAsync(
            userName: vm.Email,
            password: vm.Password,
            isPersistent: vm.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded) {
            _logger.LogInformation("Админ вошёл: {Email}", vm.Email);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        ModelState.AddModelError(string.Empty, "Неверный логин/email или пароль");
        return View(vm);
    }

    // POST: /Admin/Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Logout() {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("Админ вышел");
        return RedirectToAction(nameof(Login));
    }

    // GET: /Admin/Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied() => View();
}