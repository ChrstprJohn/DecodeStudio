using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using MyPortfolio.Validation;

namespace MyPortfolio.Models.ViewModels;

public class TeamMemberViewModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = "";

    [StringLength(100)]
    [Display(Name = "Middle name (optional)")]
    public string? MiddleName { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = "";

    [Required, StringLength(150)]
    [Display(Name = "Role title")]
    public string RoleTitle { get; set; } = "";

    [Required, StringLength(2000)]
    [Display(Name = "Biography")]
    public string Bio { get; set; } = "";

    [BindNever]
    public string? PhotoPath { get; set; }

    [Display(Name = "Choose image (PNG, JPEG, or WebP; up to 2 MB)")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Remove current image")]
    public bool RemoveImage { get; set; }

    [Display(Name = "Owner")]
    public bool IsOwner { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Display order")]
    public int SortOrder { get; set; }

    [Display(Name = "Published")]
    public bool IsPublished { get; set; }
}
