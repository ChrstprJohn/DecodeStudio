namespace MyPortfolio.Helpers;

// Local defaults for the sample content; an uploaded image always takes precedence.
public static class DemoImagePaths
{
    private static readonly Dictionary<string, string> ServiceKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Web Design"] = "web-design",
        ["Web Development"] = "web-development",
        ["Landing Pages"] = "landing-pages",
        ["Ecommerce Websites"] = "ecommerce",
        ["Website Redesign"] = "website-redesign",
        ["Deployment and Hosting Setup"] = "deployment",
        ["Performance Optimization"] = "performance",
        ["Website Maintenance"] = "maintenance"
    };

    public static string? Service(string title, string? uploadedImage)
    {
        if (!string.IsNullOrWhiteSpace(uploadedImage)) return uploadedImage;
        return ServiceKeys.TryGetValue(title, out var key) ? $"/images/services/{key}.webp" : null;
    }
}
