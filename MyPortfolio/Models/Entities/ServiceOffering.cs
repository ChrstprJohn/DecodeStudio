using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPortfolio.Models.Entities;

[Table("Services")]
public class ServiceOffering
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = "";

    [MaxLength(2000)]
    public string Description { get; set; } = "";

    [MaxLength(2048)]
    public string? ImagePath { get; set; }

    public int SortOrder { get; set; }

    public bool IsPublished { get; set; }
}

