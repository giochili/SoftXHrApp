using HrApp.MVC.Models;
using HrApp.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace HrApp.MVC.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IApiService _apiService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmployeesController(IApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
        _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(string? q)
        {
        if (string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Session.GetString("jwt")))
            return RedirectToAction("Login", "Account");
            var employees = await _apiService.GetEmployeesAsync(q);
            ViewData["q"] = q;
            return View(employees);
        }

        [HttpGet]
    public IActionResult Create()
    {
        if (string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Session.GetString("jwt")))
            return RedirectToAction("Login", "Account");
        return View(new EmployeeViewModel());
    }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
        if (string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Session.GetString("jwt")))
            return RedirectToAction("Login", "Account");
            if (!ModelState.IsValid) return View(model);
            var result = await _apiService.CreateEmployeeAsync(model);
            if (result.Success)
            {
                TempData["Message"] = "თანამშრომელი წარმატებით დაემატა";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Message ?? "დაფიქსირდა შეცდომა");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
        if (string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Session.GetString("jwt")))
            return RedirectToAction("Login", "Account");
            var employee = await _apiService.GetEmployeeAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel model)
        {
        if (string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Session.GetString("jwt")))
            return RedirectToAction("Login", "Account");
            if (!ModelState.IsValid) return View(model);
            var result = await _apiService.UpdateEmployeeAsync(id, model);
            if (result.Success)
            {
                TempData["Message"] = "ცვლილებები შენახულია";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Message ?? "დაფიქსირდა შეცდომა");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
        if (string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Session.GetString("jwt")))
            return RedirectToAction("Login", "Account");
            var employee = await _apiService.GetEmployeeAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
        if (string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Session.GetString("jwt")))
            return RedirectToAction("Login", "Account");
            var ok = await _apiService.DeleteEmployeeAsync(id);
            if (ok)
            {
                TempData["Message"] = "ჩანაწერი წაიშალა";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "დაფიქსირდა შეცდომა");
            var employee = await _apiService.GetEmployeeAsync(id);
            return View("Delete", employee);
        }
    }
}
