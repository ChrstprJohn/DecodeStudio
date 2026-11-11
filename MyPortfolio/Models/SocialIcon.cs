using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MyPortfolio.Models;

public static class SocialIcon
{
    private const int MaxBytes = 64 * 1024;
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private static readonly HashSet<string> Elements = new(StringComparer.Ordinal)
    {
        "svg", "g", "path", "rect", "circle", "ellipse", "line", "polyline", "polygon", "title", "desc"
    };
    private static readonly HashSet<string> Attributes = new(StringComparer.Ordinal)
    {
        "viewBox", "width", "height", "preserveAspectRatio", "version", "id",
        "d", "x", "y", "x1", "x2", "y1", "y2", "cx", "cy", "r", "rx", "ry", "points",
        "fill", "fill-rule", "fill-opacity", "opacity", "transform",
        "stroke", "stroke-width", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit",
        "stroke-opacity", "stroke-dasharray", "stroke-dashoffset", "clip-rule", "vector-effect"
    };

    public static string Read(IFormFile file)
    {
        const string error = "Choose a plain SVG icon up to 64 KB using paths or shapes.";
        if (file.Length is <= 0 or > MaxBytes || !string.Equals(Path.GetExtension(file.FileName), ".svg", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException(error);

        try
        {
            using var stream = file.OpenReadStream();
            using var reader = XmlReader.Create(stream, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                MaxCharactersInDocument = MaxBytes,
                IgnoreProcessingInstructions = true,
                IgnoreComments = true
            });
            var root = XDocument.Load(reader).Root;
            if (root?.Name != Svg + "svg" || root.DescendantsAndSelf().Any(element =>
                element.Name.Namespace != Svg || !Elements.Contains(element.Name.LocalName) ||
                element.Attributes().Any(attribute => !attribute.IsNamespaceDeclaration &&
                    (attribute.Name.Namespace != XNamespace.None || !Attributes.Contains(attribute.Name.LocalName) ||
                     attribute.Value.Contains("url", StringComparison.OrdinalIgnoreCase)))))
                throw new ArgumentException(error);

            return root.ToString(SaveOptions.DisableFormatting);
        }
        catch (XmlException exception)
        {
            throw new ArgumentException(error, exception);
        }
    }

    public static string? Source(string? svg) => svg is null
        ? null
        : "data:image/svg+xml;base64," + Convert.ToBase64String(Encoding.UTF8.GetBytes(svg));
}
