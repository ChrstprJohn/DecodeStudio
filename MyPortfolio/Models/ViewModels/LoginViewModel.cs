using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Models.ViewModels;

public class LoginViewModel
{
    [Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = "";

    [Required, StringLength(128), DataType(DataType.Password)]
    public string Password { get; set; } = "";
}
