using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyPortfolio.Validation;
using MyPortfolio.Models.Entities;

namespace MyPortfolio.Models.ViewModels;

public class SocialLinkViewModel
{
    public int Id { get; set; }

    [Display(Name = "Custom SVG icon")]
    public IFormFile? CustomIconFile { get; set; }

    [BindNever]
    public string? CurrentIconSource { get; set; }

    [Display(Name = "Remove custom icon")]
    public bool RemoveCustomIcon { get; set; }

    [Required, StringLength(100)]
    public string Label { get; set; } = "";

    [Required, StringLength(2048), HttpUrl]
    public string Url { get; set; } = "";

    [Range(0, int.MaxValue)]
    [Display(Name = "Display order")]
    public int SortOrder { get; set; }

    public void ApplyTo(SocialLink item)
    {
        item.Label = Label.Trim();
        item.Url = Url.Trim();
        item.SortOrder = SortOrder;
        item.CustomIconSvg = CustomIconFile is { } file ? SocialIcon.Read(file)
            : RemoveCustomIcon ? null : item.CustomIconSvg;
    }
}
