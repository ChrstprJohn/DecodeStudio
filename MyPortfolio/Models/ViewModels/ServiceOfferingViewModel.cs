using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using MyPortfolio.Validation;
using MyPortfolio.Models.Entities;

namespace MyPortfolio.Models.ViewModels;

public class ServiceOfferingViewModel
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = "";

    [Required, StringLength(2000)]
    public string Description { get; set; } = "";

    [BindNever]
    public string? ImagePath { get; set; }

    [Display(Name = "Choose image (PNG, JPEG, or WebP; up to 2 MB)")]
    public IFormFile? ImageFile { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Display order")]
    public int SortOrder { get; set; }

    [Display(Name = "Published")]
    public bool IsPublished { get; set; }

    public void ApplyTo(ServiceOffering item)
    {
        item.Title = Title.Trim();
        item.Description = Description.Trim();
        item.SortOrder = SortOrder;
        item.IsPublished = IsPublished;
    }
}
