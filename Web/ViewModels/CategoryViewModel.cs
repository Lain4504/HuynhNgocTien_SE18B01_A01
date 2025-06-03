using System.ComponentModel.DataAnnotations;
using DAL.Entities;

namespace Web.ViewModels;

public class CategoryViewModel
{
    public short CategoryId { get; set; }

    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string CategoryName { get; set; } = null!;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
    public string CategoryDescription { get; set; } = null!;

    public short? ParentCategoryId { get; set; }
    public bool? IsActive { get; set; } = true;

    // Navigation properties
    public Category? ParentCategory { get; set; }
    public List<Category> AvailableParentCategories { get; set; } = new();
} 