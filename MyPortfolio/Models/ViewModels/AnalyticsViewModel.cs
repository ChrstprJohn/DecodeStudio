namespace MyPortfolio.Models.ViewModels;

public class AnalyticsViewModel
{
    public required DateOnly Today { get; init; }
    public required AnalyticsTotals Totals { get; init; }
    public required IReadOnlyList<RegistrationDay> Registrations { get; init; }
}

public class AnalyticsTotals
{
    public int ActiveAccounts { get; set; }
    public int InactiveAccounts { get; set; }
    public int PublishedProjects { get; set; }
    public int DraftProjects { get; set; }
    public int PublishedServices { get; set; }
    public int DraftServices { get; set; }
    public int PublishedTeamMembers { get; set; }
    public int DraftTeamMembers { get; set; }
}

public class RegistrationDay
{
    public DateOnly Date { get; set; }
    public int Registrations { get; set; }
}
