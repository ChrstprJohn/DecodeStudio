using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPortfolio.Models.Entities;

[Table("TeamMembers")]
public class TeamMember
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string FirstName { get; set; } = "";

    [MaxLength(100)]
    public string? MiddleName { get; set; }

    [MaxLength(100)]
    public string LastName { get; set; } = "";

    [MaxLength(150)]
    public string RoleTitle { get; set; } = "";

    [MaxLength(2000)]
    public string Bio { get; set; } = "";

    [MaxLength(500)]
    public string? PhotoPath { get; set; }

    public bool IsOwner { get; set; }

    public int SortOrder { get; set; }

    public bool IsPublished { get; set; }
}
