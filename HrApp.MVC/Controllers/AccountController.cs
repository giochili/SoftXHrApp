using HrApp.MVC.Models;
using HrApp.MVC.Services;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.RegisterUserAsync(model);
            if (result.Success)
                return RedirectToAction("Index", "Employees"); // Main page

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.LoginUserAsync(model);
            if (result.Success)
                return RedirectToAction("Index", "Employees");

            ModelState.AddModelError("", result.Message ?? "მომხმარებელი ან პაროლი არასწორია");
            return View(model);
        }
    }
}
