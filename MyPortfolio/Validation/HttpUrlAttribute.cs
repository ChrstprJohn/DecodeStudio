using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Validation;

public class HttpUrlAttribute : ValidationAttribute
{
    public HttpUrlAttribute()
    {
        ErrorMessage = "Enter a complete http or https URL.";
    }

    public override bool IsValid(object? value)
    {
        return value is null || value is string text &&
            (string.IsNullOrEmpty(text) || Uri.TryCreate(text, UriKind.Absolute, out var uri) &&
            (uri.Scheme == "http" || uri.Scheme == "https"));
    }
}
