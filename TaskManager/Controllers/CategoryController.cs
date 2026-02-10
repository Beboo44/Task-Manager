using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Business.Services;
using TaskManager.Business.DTOs;
using TaskManager.ViewModels;

namespace TaskManager.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            ICategoryService categoryService,
            ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var categories = await _categoryService.GetAllCategoriesAsync(userId);

            var viewModel = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                TaskCount = c.TaskCount
            }).ToList();

            return View(viewModel);
        }

        // GET: /Category/Create
        public IActionResult Create()
        {
            return View(new CategoryViewModel());
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var categoryDto = new CategoryDto
            {
                Name = model.Name,
                Description = model.Description,
                UserId = GetUserId()
            };

            await _categoryService.CreateCategoryAsync(categoryDto);
            TempData["SuccessMessage"] = "Category created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();
            var category = await _categoryService.GetCategoryByIdAsync(id, userId);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                TaskCount = category.TaskCount
            };

            return View(viewModel);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var categoryDto = new CategoryDto
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                UserId = GetUserId()
            };

            var success = await _categoryService.UpdateCategoryAsync(categoryDto);

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to update category.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Category updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var category = await _categoryService.GetCategoryByIdAsync(id, userId);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                TaskCount = category.TaskCount
            };

            return View(viewModel);
        }

        // POST: /Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            
            // Check if category can be deleted
            var canDelete = await _categoryService.CanDeleteCategoryAsync(id, userId);
            
            if (!canDelete)
            {
                TempData["ErrorMessage"] = "Cannot delete category that has tasks. Please delete or reassign the tasks first.";
                return RedirectToAction(nameof(Index));
            }

            var success = await _categoryService.DeleteCategoryAsync(id, userId);

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to delete category.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
