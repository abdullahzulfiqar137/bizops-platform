using BusinessOperations.API.Data;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Products;

public sealed class ProductMediaService : IProductMediaService
{
    private const long MaxBytes = 5 * 1024 * 1024;
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _environment;
    private readonly ICurrentUserService _user;
    public ProductMediaService(ApplicationDbContext db, IWebHostEnvironment environment, ICurrentUserService user)
    { _db = db; _environment = environment; _user = user; }

    public async Task<ApiResponse<string>> UploadImageAsync(Guid productId, IFormFile file, CancellationToken token = default)
    {
        var product = await _db.Products.SingleOrDefaultAsync(x => x.Id == productId, token);
        if (product == null) return ApiResponse<string>.FailureResponse("Product not found.");
        if (file.Length == 0 || file.Length > MaxBytes) return ApiResponse<string>.FailureResponse("Image must be between 1 byte and 5 MB.");
        var format = await DetectFormatAsync(file, token);
        if (format == null) return ApiResponse<string>.FailureResponse("Only valid JPEG, PNG, or WebP images are accepted.");
        var root = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var directory = Path.GetFullPath(Path.Combine(root, "uploads", "products"));
        Directory.CreateDirectory(directory);
        var fileName = $"{product.Id:N}-{Guid.NewGuid():N}.{format}";
        var path = Path.GetFullPath(Path.Combine(directory, fileName));
        if (!path.StartsWith(directory + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid product image path.");
        await using (var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true))
            await file.CopyToAsync(output, token);
        RemovePhysicalFile(product.ImageUrl, root);
        product.ImageUrl = $"/uploads/products/{fileName}";
        product.UpdatedBy = _user.Username;
        await _db.SaveChangesAsync(token);
        return ApiResponse<string>.SuccessResponse(product.ImageUrl, "Product image uploaded successfully.");
    }

    public async Task<ApiResponse<bool>> RemoveImageAsync(Guid productId, CancellationToken token = default)
    {
        var product = await _db.Products.SingleOrDefaultAsync(x => x.Id == productId, token);
        if (product == null) return ApiResponse<bool>.FailureResponse("Product not found.");
        var root = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        RemovePhysicalFile(product.ImageUrl, root);
        product.ImageUrl = null; product.UpdatedBy = _user.Username;
        await _db.SaveChangesAsync(token);
        return ApiResponse<bool>.SuccessResponse(true, "Product image removed successfully.");
    }

    private static async Task<string?> DetectFormatAsync(IFormFile file, CancellationToken token)
    {
        var header = new byte[12];
        await using var stream = file.OpenReadStream();
        var read = await stream.ReadAsync(header, token);
        if (read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF) return "jpg";
        if (read >= 8 && header.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })) return "png";
        if (read >= 12 && System.Text.Encoding.ASCII.GetString(header, 0, 4) == "RIFF" && System.Text.Encoding.ASCII.GetString(header, 8, 4) == "WEBP") return "webp";
        return null;
    }

    private static void RemovePhysicalFile(string? url, string root)
    {
        if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("/uploads/products/", StringComparison.Ordinal)) return;
        var directory = Path.GetFullPath(Path.Combine(root, "uploads", "products"));
        var path = Path.GetFullPath(Path.Combine(directory, Path.GetFileName(url)));
        if (path.StartsWith(directory + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) && File.Exists(path)) File.Delete(path);
    }
}
