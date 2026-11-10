namespace MyPortfolio.Services;

public class ImageUploadService
{
    private readonly IWebHostEnvironment _environment;

    public ImageUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveAsync(IFormFile file)
    {
        if (file.Length <= 0 || file.Length > 2 * 1024 * 1024)
        {
            throw new ArgumentException("Choose a photo smaller than or equal to 2 MB.");
        }
        await using var input = file.OpenReadStream();
        var header = new byte[12];
        var count = await input.ReadAtLeastAsync(header, 12, throwOnEndOfStream: false);
        string? extension = null;
        if (count >= 8 && header.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })) extension = ".png";
        if (count >= 3 && header[0] == 255 && header[1] == 216 && header[2] == 255) extension = ".jpg";
        if (count == 12 && System.Text.Encoding.ASCII.GetString(header, 0, 4) == "RIFF" &&
            System.Text.Encoding.ASCII.GetString(header, 8, 4) == "WEBP") extension = ".webp";
        if (extension is null) throw new ArgumentException("Choose a PNG, JPEG, or WebP image.");

        var directory = Path.Combine(_environment.WebRootPath, "uploads", "images");
        Directory.CreateDirectory(directory);
        var filename = Guid.NewGuid().ToString("N") + extension;
        var path = Path.Combine(directory, filename);
        try
        {
            await using var output = File.Create(path);
            await output.WriteAsync(header.AsMemory(0, count));
            await input.CopyToAsync(output);
        }
        catch
        {
            File.Delete(path);
            throw;
        }
        return "/uploads/images/" + filename;
    }

    public void DeleteGenerated(string path)
    {
        var filename = Path.GetFileName(path);
        var folder = path == "/uploads/images/" + filename ? "images"
            : path == "/uploads/profiles/" + filename ? "profiles" : null;
        if (folder is not null &&
            Guid.TryParseExact(Path.GetFileNameWithoutExtension(filename), "N", out _))
        {
            File.Delete(Path.Combine(_environment.WebRootPath, "uploads", folder, filename));
        }
    }
}
