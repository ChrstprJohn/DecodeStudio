using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPortfolio.Models.Entities;

[Table("SiteContent")]
public class SiteContent
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [MaxLength(150)]
    public string StudioName { get; set; } = "";

    [MaxLength(250)]
    public string? Tagline { get; set; }

    [MaxLength(200)]
    public string HeroTitle { get; set; } = "";

    [MaxLength(2000)]
    public string HeroDescription { get; set; } = "";

    [MaxLength(200)]
    public string AboutTitle { get; set; } = "";

    public string AboutDescription { get; set; } = "";

    [MaxLength(256)]
    public string ContactEmail { get; set; } = "";

    [MaxLength(30)]
    public string? ContactPhone { get; set; }

    [MaxLength(500)]
    public string? ContactBackgroundImagePath { get; set; }

}
