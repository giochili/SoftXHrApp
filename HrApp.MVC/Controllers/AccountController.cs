using HrApp.MVC.Models;
using HrApp.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace HrApp.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApiService _apiService;

        public AccountController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("jwt")))
                return RedirectToAction("Index", "Employees");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.RegisterUserAsync(model);
            if (result.Success && !string.IsNullOrWhiteSpace(result.Token))
            {
                HttpContext.Session.SetString("jwt", result.Token);
                TempData["t"] = "set";
                return RedirectToAction("Index", "Employees");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("jwt")))
                return RedirectToAction("Index", "Employees");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.LoginUserAsync(model);
            if (result.Success && !string.IsNullOrWhiteSpace(result.Token))
            {
                HttpContext.Session.SetString("jwt", result.Token);
                TempData["t"] = $"ok:1 token:len:{result.Token.Length}";
                return RedirectToAction("Index", "Employees");
            }
            if (result.Success && !string.IsNullOrWhiteSpace(result.Token))
            {
                HttpContext.Session.SetString("jwt", result.Token);
                TempData["t"] = $"ok:1 token:len:{result.Token.Length}";
                return RedirectToAction("Index", "Employees");
            }

            ModelState.AddModelError("", result.Message ?? "მომხმარებელი ან პაროლი არასწორია");
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("jwt");
            return RedirectToAction("Login");
        }
    }
}
