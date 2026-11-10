using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Models.ViewModels;

public class ChangePasswordViewModel
{
    [Required, StringLength(128), DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string CurrentPassword { get; set; } = "";

    [Required, StringLength(128, MinimumLength = 8), DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; } = "";

    [Required, Compare(nameof(NewPassword)), DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    public string ConfirmPassword { get; set; } = "";
}
