using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Validation;

public class LinkUrlAttribute : ValidationAttribute
{
    public LinkUrlAttribute()
    {
        ErrorMessage = "Use http/https, a local /path, or #section.";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string text || string.IsNullOrEmpty(text))
        {
            return true;
        }
        return (text.StartsWith('/') && !text.StartsWith("//") || text.StartsWith('#')) &&
            !text.Contains('\\') && !text.Any(char.IsControl)
            || new HttpUrlAttribute().IsValid(text);
    }
}
