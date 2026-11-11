using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Models.ViewModels;

public class ContactViewModel
{
    [StringLength(256), EmailAddress]
    public string? Email { get; set; }

    [StringLength(30), DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }

    public string? BackgroundImagePath { get; set; }

    [Display(Name = "Background image")]
    public IFormFile? BackgroundImageFile { get; set; }

    [Display(Name = "Use the default collage")]
    public bool RemoveBackgroundImage { get; set; }
}
