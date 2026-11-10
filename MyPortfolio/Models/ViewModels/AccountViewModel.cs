namespace MyPortfolio.Models.ViewModels;

public class AccountViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? ProfileImagePath { get; set; }
    public string Role { get; set; } = "";
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
