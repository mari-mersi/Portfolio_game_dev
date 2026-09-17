using Microsoft.AspNetCore.Mvc;

namespace Portfolio_game_dev.Controllers {
    /// <summary>
    /// Обработка ошибок: 500 (исключения) и 404 (не найдено).
    /// </summary>
    [Route("error")]
    public class ErrorController : Controller {
        // GET: /error
        [Route("")]
        [Route("500")]
        public IActionResult Error() {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            ViewData["Title"] = "Ошибка";
            ViewData["Code"] = 500;
            return View();
        }

        // GET: /error/404
        [Route("404")]
        public IActionResult NotFound() {
            Response.StatusCode = StatusCodes.Status404NotFound;
            ViewData["Title"] = "Страница не найдена";
            ViewData["Code"] = 404;
            return View();
        }

        // GET: /error/{code} — fallback для любых других статусов
        [Route("{code:int}")]
        public IActionResult StatusCodePage(int code) {
            Response.StatusCode = code;
            ViewData["Title"] = $"Ошибка {code}";
            ViewData["Code"] = code;
            return View("Error");
        }
    }
}