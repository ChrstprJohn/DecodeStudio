namespace MyPortfolio.Helpers;

// Local defaults for the sample content; an uploaded image always takes precedence.
public static class DemoImagePaths
{
    private static readonly HashSet<string> ProjectKeys =
        ["architecture", "ecommerce", "hospitality", "fitness", "food", "education", "beauty", "creative"];

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

    public static string Project(string? imagePath, bool wall = false)
    {
        const string prefix = "/uploads/images/demo-project-";
        if (imagePath is not null && imagePath.StartsWith(prefix) && imagePath.EndsWith(".svg"))
        {
            var key = imagePath[prefix.Length..^4];
            if (ProjectKeys.Contains(key))
                return $"/images/projects/{key}{(wall ? "-wall" : "")}.webp";
        }
        return imagePath ?? "";
    }

    public static string? Service(string title, string? uploadedImage)
    {
        if (!string.IsNullOrWhiteSpace(uploadedImage)) return uploadedImage;
        return ServiceKeys.TryGetValue(title, out var key) ? $"/images/services/{key}.webp" : null;
    }
}
