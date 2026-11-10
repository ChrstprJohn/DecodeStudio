using System.ComponentModel.DataAnnotations;
using MyPortfolio.Validation;

namespace MyPortfolio.Models.ViewModels;

public class SocialLinkViewModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Label { get; set; } = "";

    [Required, StringLength(2048), HttpUrl]
    public string Url { get; set; } = "";

    [Range(0, int.MaxValue)]
    [Display(Name = "Display order")]
    public int SortOrder { get; set; }
}
