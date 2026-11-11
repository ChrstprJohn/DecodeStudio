using System.ComponentModel.DataAnnotations;

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

    [Required, StringLength(200)]
    [Display(Name = "About heading")]
    public string AboutTitle { get; set; } = "";

    [Required]
    [Display(Name = "About description")]
    public string AboutDescription { get; set; } = "";

}
