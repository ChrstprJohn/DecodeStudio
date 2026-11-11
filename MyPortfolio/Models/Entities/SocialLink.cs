using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPortfolio.Models.Entities;

[Table("SocialLinks")]
public class SocialLink
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Label { get; set; } = "";

    [MaxLength(2048)]
    public string Url { get; set; } = "";

    public int SortOrder { get; set; }

    public string? CustomIconSvg { get; set; }
}
