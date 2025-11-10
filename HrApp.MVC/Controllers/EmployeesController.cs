using HrApp.MVC.Models;
using HrApp.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var positions = await _apiService.GetPositionsHierarchyAsync();

            var viewModel = new EmployeesIndexViewModel
            {
                Employees = employees,
                Positions = positions,
                Query = q
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Session.GetString("jwt")))
                return RedirectToAction("Login", "Account");

            await PopulatePositionOptions();
            var model = new EmployeeViewModel { Status = 0 }; // Inactive by default
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            if (string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Session.GetString("jwt")))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                await PopulatePositionOptions();
                return View(model);
            }
            // Ensure status is always Inactive (0) for new employees
            model.Status = 0;
            var result = await _apiService.CreateEmployeeAsync(model);
            if (result.Success)
            {
                TempData["Message"] = "თანამშრომელი წარმატებით დაემატა";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Message ?? "დაფიქსირდა შეცდომა");
            await PopulatePositionOptions();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
        if (string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Session.GetString("jwt")))
            return RedirectToAction("Login", "Account");
            await PopulatePositionOptions();
            var employee = await _apiService.GetEmployeeAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel model)
        {
        if (string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Session.GetString("jwt")))
            return RedirectToAction("Login", "Account");
            await PopulatePositionOptions();
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

        private async Task PopulatePositionOptions()
        {
            var hierarchy = await _apiService.GetPositionsHierarchyAsync();
            var items = new List<SelectListItem>();

            void Flatten(PositionTreeViewModel node, int depth)
            {
                var prefix = depth > 0 ? new string('·', depth) + " " : string.Empty;
                items.Add(new SelectListItem
                {
                    Value = node.Id.ToString(),
                    Text = prefix + node.Title
                });

                if (node.Children == null || node.Children.Count == 0)
                {
                    return;
                }

                foreach (var child in node.Children)
                {
                    Flatten(child, depth + 1);
                }
            }

            foreach (var node in hierarchy)
            {
                Flatten(node, 0);
            }

            ViewBag.PositionOptions = items;
        }
    }
}
