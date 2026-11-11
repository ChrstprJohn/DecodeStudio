using MyPortfolio.Models.Entities;

namespace MyPortfolio.Models.ViewModels;

public class HomeViewModel
{
    public string StudioName { get; set; } = "Decode Studio";
    public string HeroTitle { get; set; } = "Decode Studio";
    public string HeroDescription { get; set; } = "";
    public SiteContent? Content { get; set; }
    public List<Project> Projects { get; set; } = [];
    public List<(string ImagePath, string Title)> HeroPreviews { get; set; } = [];
    public List<ServiceOffering> Services { get; set; } = [];
    public List<TeamMember> TeamMembers { get; set; } = [];
    public List<SocialLink> SocialLinks { get; set; } = [];
}
