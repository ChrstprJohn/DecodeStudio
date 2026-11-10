using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using MyPortfolio.Validation;

namespace MyPortfolio.Models.ViewModels;

public class SiteContentViewModel
{
    [Required, StringLength(150)]
    [Display(Name = "Studio name")]
    public string StudioName { get; set; } = "";

    [StringLength(250)]
    public string? Tagline { get; set; }

    [Required, StringLength(200)]
    [Display(Name = "Hero title")]
    public string HeroTitle { get; set; } = "";

    [Required, StringLength(2000)]
    [Display(Name = "Hero description")]
    public string HeroDescription { get; set; } = "";

    [BindNever]
    public string? HeroImagePath { get; set; }

    [Display(Name = "Choose image (PNG, JPEG, or WebP; up to 2 MB)")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Remove current image")]
    public bool RemoveImage { get; set; }

    [StringLength(100)]
    [Display(Name = "Hero link text")]
    public string? HeroLinkText { get; set; }

    [StringLength(2048), LinkUrl]
    [Display(Name = "Hero link URL")]
    public string? HeroLinkUrl { get; set; }

    [Required, StringLength(200)]
    [Display(Name = "About heading")]
    public string AboutTitle { get; set; } = "";

    [Required]
    [Display(Name = "About description")]
    public string AboutDescription { get; set; } = "";

    [Required, StringLength(256), EmailAddress]
    [Display(Name = "Contact email")]
    public string ContactEmail { get; set; } = "";

    [StringLength(30)]
    [Display(Name = "Contact phone")]
    public string? ContactPhone { get; set; }

    [StringLength(500)]
    [Display(Name = "Footer text")]
    public string? FooterText { get; set; }
}
