using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Models.ViewModels;

public class ContactViewModel
{
    [StringLength(256), EmailAddress]
    public string? Email { get; set; }

    [StringLength(30), DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }
}
