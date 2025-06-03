using BLL.Interfaces;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers;

public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (!accountId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetInt32("AccountRole");
        if (role != 3) // Only Admin can manage categories
        {
            return Forbid();
        }

        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (!accountId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetInt32("AccountRole");
        if (role != 3) // Only Admin can create categories
        {
            return Forbid();
        }

        var categories = await _categoryService.GetAllAsync();
        var model = new CategoryViewModel
        {
            AvailableParentCategories = categories.ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryViewModel model)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (!accountId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetInt32("AccountRole");
        if (role != 3) // Only Admin can create categories
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            model.AvailableParentCategories = (await _categoryService.GetAllAsync()).ToList();
            return View(model);
        }

        try
        {
            var category = new Category
            {
                CategoryName = model.CategoryName,
                CategoryDescription = model.CategoryDescription,
                ParentCategoryId = model.ParentCategoryId,
                IsActive = model.IsActive
            };

            await _categoryService.CreateAsync(category);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.AvailableParentCategories = (await _categoryService.GetAllAsync()).ToList();
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (!accountId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetInt32("AccountRole");
        if (role != 3) // Only Admin can edit categories
        {
            return Forbid();
        }

        var category = await _categoryService.GetByIdAsync((short)id);
        if (category == null)
        {
            return NotFound();
        }

        var categories = await _categoryService.GetAllAsync();
        var model = new CategoryViewModel
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            CategoryDescription = category.CategoryDescription,
            ParentCategoryId = category.ParentCategoryId,
            IsActive = category.IsActive,
            AvailableParentCategories = categories.Where(c => c.CategoryId != id).ToList() // Exclude current category from parent options
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CategoryViewModel model)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (!accountId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetInt32("AccountRole");
        if (role != 3) // Only Admin can edit categories
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            model.AvailableParentCategories = (await _categoryService.GetAllAsync()).Where(c => c.CategoryId != model.CategoryId).ToList();
            return View(model);
        }

        try
        {
            var category = await _categoryService.GetByIdAsync(model.CategoryId);
            if (category == null)
            {
                return NotFound();
            }

            category.CategoryName = model.CategoryName;
            category.CategoryDescription = model.CategoryDescription;
            category.ParentCategoryId = model.ParentCategoryId;
            category.IsActive = model.IsActive;

            await _categoryService.UpdateAsync(category);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.AvailableParentCategories = (await _categoryService.GetAllAsync()).Where(c => c.CategoryId != model.CategoryId).ToList();
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (!accountId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetInt32("AccountRole");
        if (role != 3) // Only Admin can delete categories
        {
            return Forbid();
        }

        try
        {
            await _categoryService.DeleteAsync((short)id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
} 