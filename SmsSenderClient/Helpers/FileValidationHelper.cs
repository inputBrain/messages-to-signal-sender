namespace SmsSenderClient.Helpers;

public static class FileValidationHelper
{
    private static readonly Dictionary<string, byte[][]> ImageSignatures = new()
    {
        { "image/jpeg", new[] { new byte[] { 0xFF, 0xD8, 0xFF } } },

        { "image/png", new[] { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },

        { "image/gif", new[] {
            new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 },
            new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }
        }},

        { "image/webp", new[] { new byte[] { 0x52, 0x49, 0x46, 0x46 } } },

        { "image/bmp", new[] { new byte[] { 0x42, 0x4D } } }
    };
    
    private static readonly HashSet<string> HeicBrands = new(StringComparer.OrdinalIgnoreCase)
    {
        "heic", "heix", "hevc", "hevx",
        "mif1", "msf1",
        "avif"
    };

    private static readonly HashSet<string> BlockedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".svg",
        ".svgz",
        ".php",
        ".phtml",
        ".asp",
        ".aspx",
        ".jsp",
        ".jspx",
        ".exe",
        ".dll",
        ".bat",
        ".cmd",
        ".ps1",
        ".sh",
        ".html",
        ".htm",
        ".xhtml",
        ".js",
        ".mjs"
    };

    public static (bool IsValid, string? Error) ValidateImageFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return (true, null);
        }

        var extension = Path.GetExtension(file.FileName);
        if (BlockedExtensions.Contains(extension))
        {
            return (false, $"Тип файлу '{extension}' заборонений.");
        }

        using var stream = file.OpenReadStream();
        var headerBytes = new byte[32];
        var bytesRead = stream.Read(headerBytes, 0, headerBytes.Length);

        if (bytesRead < 4)
        {
            return (false, "Файл пошкоджений або порожній.");
        }
        
        if (bytesRead >= 12 &&
            headerBytes[4] == 0x66 && // f
            headerBytes[5] == 0x74 && // t
            headerBytes[6] == 0x79 && // y
            headerBytes[7] == 0x70)   // p
        {
            var brand = System.Text.Encoding.ASCII.GetString(headerBytes, 8, 4).TrimEnd('\0');
            if (HeicBrands.Contains(brand))
            {
                return (true, null);
            }
        }

        foreach (var (mimeType, signatures) in ImageSignatures)
        {
            foreach (var signature in signatures)
            {
                if (bytesRead >= signature.Length && 
                    headerBytes.Take(signature.Length).SequenceEqual(signature))
                {
                    if (mimeType == "image/webp")
                    {
                        if (bytesRead >= 12 &&
                            headerBytes[8] == 0x57 && // W
                            headerBytes[9] == 0x45 && // E
                            headerBytes[10] == 0x42 && // B
                            headerBytes[11] == 0x50)   // P
                        {
                            return (true, null);
                        }
                        continue;
                    }
                    return (true, null);
                }
            }
        }

        return (false, "Файл не є дійсним зображенням. Дозволені формати: JPEG, PNG, GIF, WebP, BMP, HEIC/HEIF.");
    }
}