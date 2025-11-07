using System.Collections.Generic;
using System.Linq;
using HrApp.MVC.Models;
using HrApp.MVC.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrApp.MVC.Controllers
{
    public class PositionsController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PositionsController(IApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");

            var positions = await _apiService.GetPositionsAsync();
            var tree = await _apiService.GetPositionsHierarchyAsync();

            var parentLookup = positions.ToDictionary(p => p.Id, p => p.Title);
            foreach (var position in positions)
            {
                if (position.ParentPositionId.HasValue && parentLookup.TryGetValue(position.ParentPositionId.Value, out var parentTitle))
                {
                    position.ParentTitle = parentTitle;
                }
            }

            var viewModel = new PositionsPageViewModel
            {
                Positions = positions,
                Tree = tree
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");

            await PopulateParentOptions();
            return View(new PositionViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(PositionViewModel model)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                await PopulateParentOptions();
                return View(model);
            }

            var result = await _apiService.CreatePositionAsync(model);
            if (result.Success)
            {
                TempData["Message"] = "პოზიცია წარმატებით დაემატა";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message ?? "დაფიქსირდა შეცდომა");
            await PopulateParentOptions();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");

            var position = await _apiService.GetPositionAsync(id);
            if (position == null) return NotFound();

            await PopulateParentOptions(id);
            await FillParentTitle(position);
            return View(position);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, PositionViewModel model)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                await PopulateParentOptions(id);
                return View(model);
            }

            if (model.ParentPositionId == id)
            {
                ModelState.AddModelError(nameof(model.ParentPositionId), "პოზიცია ვერ იქნება საკუთარი თავის შვილობილი");
                await PopulateParentOptions(id);
                return View(model);
            }

            var result = await _apiService.UpdatePositionAsync(id, model);
            if (result.Success)
            {
                TempData["Message"] = "პოზიცია განახლდა";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message ?? "დაფიქსირდა შეცდომა");
            await PopulateParentOptions(id);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");

            var position = await _apiService.GetPositionAsync(id);
            if (position == null) return NotFound();

            await FillParentTitle(position);
            return View(position);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login", "Account");

            var ok = await _apiService.DeletePositionAsync(id);
            if (ok)
            {
                TempData["Message"] = "პოზიცია წაიშალა";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "დაფიქსირდა შეცდომა");
            var position = await _apiService.GetPositionAsync(id);
            if (position != null)
            {
                await FillParentTitle(position);
            }
            return View("Delete", position);
        }

        private bool IsAuthenticated() => !string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext?.Session.GetString("jwt"));

        private async Task PopulateParentOptions(int? excludeId = null)
        {
            var hierarchy = await _apiService.GetPositionsHierarchyAsync();
            var items = new List<SelectListItem>
            {
                new SelectListItem { Value = string.Empty, Text = "(ფესვი)" }
            };

            void Flatten(PositionTreeViewModel node, int depth)
            {
                if (excludeId.HasValue && node.Id == excludeId.Value)
                {
                    foreach (var child in node.Children)
                    {
                        Flatten(child, depth + 1);
                    }
                    return;
                }

                var prefix = depth > 0 ? new string('·', depth) + " " : string.Empty;
                items.Add(new SelectListItem
                {
                    Value = node.Id.ToString(),
                    Text = prefix + node.Title
                });

                foreach (var child in node.Children)
                {
                    Flatten(child, depth + 1);
                }
            }

            foreach (var node in hierarchy)
            {
                Flatten(node, 0);
            }

            ViewBag.ParentOptions = items;
        }

        private async Task FillParentTitle(PositionViewModel position)
        {
            if (position.ParentPositionId == null) return;

            var allPositions = await _apiService.GetPositionsAsync();
            var parent = allPositions.FirstOrDefault(p => p.Id == position.ParentPositionId);
            if (parent != null)
            {
                position.ParentTitle = parent.Title;
            }
        }
    }
}

