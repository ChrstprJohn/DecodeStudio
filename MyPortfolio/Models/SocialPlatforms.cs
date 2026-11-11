namespace MyPortfolio.Models;

public static class SocialPlatforms
{
    public static IReadOnlyList<string> Names { get; } = Array.AsReadOnly(new[]
    {
        "GitHub", "LinkedIn", "Instagram", "Facebook", "Behance", "Dribbble",
        "YouTube", "X", "TikTok", "Discord", "WhatsApp", "Threads"
    });

    public static string? IconPath(string label)
    {
        var name = Names.FirstOrDefault(name =>
            string.Equals(label.Trim(), name, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(label.Trim(), $"{name} (Demo)", StringComparison.OrdinalIgnoreCase));
        return name is null ? null : $"~/icons/social/{name.ToLowerInvariant()}.svg";
    }
}
