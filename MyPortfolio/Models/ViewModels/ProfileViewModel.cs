using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MyPortfolio.Models.ViewModels;

public class ProfileViewModel
{
    [Required, StringLength(100)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = "";

    [StringLength(100)]
    [Display(Name = "Middle name")]
    public string? MiddleName { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = "";

    [Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = "";

    [BindNever]
    public string? ProfileImagePath { get; set; }

    [Display(Name = "Profile photo (PNG, JPEG, or WebP; up to 2 MB)")]
    public IFormFile? Photo { get; set; }

    [Display(Name = "Remove current photo")]
    public bool RemovePhoto { get; set; }
}
