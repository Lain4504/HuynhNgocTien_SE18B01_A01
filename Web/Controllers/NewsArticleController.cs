using BLL.Interfaces;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers;

public class NewsArticleController : Controller
{
    private readonly INewsArticleService _newsArticleService;
    private readonly ICategoryService _categoryService;

    public NewsArticleController(INewsArticleService newsArticleService, ICategoryService categoryService)
    {
        _newsArticleService = newsArticleService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string searchString, int? categoryId, int? page)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (!accountId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetInt32("AccountRole");
        var pageSize = 10;
        var pageNumber = page ?? 1;

        var articles = await _newsArticleService.GetAllAsync();
        
        // Filter by role
        if (role == 1) // Staff
        {
            articles = articles.Where(a => a.CreatedById == accountId);
        }
        else if (role == 2) // Lecturer
        {
            articles = articles.Where(a => a.NewsStatus == true); // Only published articles
        }

        // Apply search filter
        if (!string.IsNullOrEmpty(searchString))
        {
            articles = articles.Where(a => 
                a.NewsTitle.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                a.Headline.Contains(searchString, StringComparison.OrdinalIgnoreCase));
        }

        // Apply category filter
        if (categoryId.HasValue)
        {
            articles = articles.Where(a => a.CategoryId == categoryId);
        }

        // Get categories for filter dropdown
        ViewBag.Categories = await _categoryService.GetAllAsync();
        ViewBag.CurrentSearch = searchString;
        ViewBag.CurrentCategory = categoryId;

        return View(articles.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (!accountId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var article = await _newsArticleService.GetByIdAsync(id.ToString());
        if (article == null)
        {
            return NotFound();
        }

        var role = HttpContext.Session.GetInt32("AccountRole");
        if (role == 1 && article.CreatedById != accountId) // Staff can only view their own articles
        {
            return Forbid();
        }
        else if (role == 2 && article.NewsStatus != true) // Lecturer can only view published articles
        {
            return Forbid();
        }

        return View(article);
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
        if (role != 1 && role != 3) // Only Staff and Admin can create articles
        {
            return Forbid();
        }

        var categories = await _categoryService.GetAllAsync();
        var model = new NewsArticleViewModel
        {
            AvailableCategories = categories.ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewsArticleViewModel model)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (!accountId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetInt32("AccountRole");
        if (role != 1 && role != 3) // Only Staff and Admin can create articles
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            model.AvailableCategories = (await _categoryService.GetAllAsync()).ToList();
            return View(model);
        }

        try
        {
            var article = new NewsArticle
            {
                NewsTitle = model.NewsTitle,
                Headline = model.Headline,
                NewsContent = model.NewsContent,
                NewsSource = model.NewsSource,
                CategoryId = model.CategoryId,
                NewsStatus = model.NewsStatus,
                CreatedById = (short?)accountId.Value,
                CreatedDate = DateTime.Now
            };

            await _newsArticleService.CreateAsync(article, model.SelectedTagIds);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.AvailableCategories = (await _categoryService.GetAllAsync()).ToList();
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
        if (role != 1 && role != 3) // Only Staff and Admin can edit articles
        {
            return Forbid();
        }

        var article = await _newsArticleService.GetByIdAsync(id.ToString());
        if (article == null)
        {
            return NotFound();
        }

        if (role == 1 && article.CreatedById != accountId) // Staff can only edit their own articles
        {
            return Forbid();
        }

        var model = new NewsArticleViewModel
        {
            NewsArticleId = article.NewsArticleId,
            NewsTitle = article.NewsTitle,
            Headline = article.Headline,
            NewsContent = article.NewsContent,
            NewsSource = article.NewsSource,
            CategoryId = (short)article.CategoryId!,
            NewsStatus = (bool)article.NewsStatus!,
            AvailableCategories = (await _categoryService.GetAllAsync()).ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(NewsArticleViewModel model)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (!accountId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var role = HttpContext.Session.GetInt32("AccountRole");
        if (role != 1 && role != 3) // Only Staff and Admin can edit articles
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            model.AvailableCategories = (await _categoryService.GetAllAsync()).ToList();
            return View(model);
        }

        try
        {
            var article = await _newsArticleService.GetByIdAsync(model.NewsArticleId);
            if (article == null)
            {
                return NotFound();
            }

            if (role == 1 && article.CreatedById != accountId) // Staff can only edit their own articles
            {
                return Forbid();
            }

            article.NewsTitle = model.NewsTitle;
            article.Headline = model.Headline;
            article.NewsContent = model.NewsContent;
            article.NewsSource = model.NewsSource;
            article.CategoryId = model.CategoryId;
            article.NewsStatus = model.NewsStatus;
            article.UpdatedById = (short?)accountId;
            article.ModifiedDate = DateTime.Now;

            await _newsArticleService.UpdateAsync(article, model.SelectedTagIds);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.AvailableCategories = (await _categoryService.GetAllAsync()).ToList();
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
        if (role != 1 && role != 3) // Only Staff and Admin can delete articles
        {
            return Forbid();
        }

        try
        {
            var article = await _newsArticleService.GetByIdAsync(id.ToString());
            if (article == null)
            {
                return NotFound();
            }

            if (role == 1 && article.CreatedById != accountId) // Staff can only delete their own articles
            {
                return Forbid();
            }

            await _newsArticleService.DeleteAsync(id.ToString());
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
} 