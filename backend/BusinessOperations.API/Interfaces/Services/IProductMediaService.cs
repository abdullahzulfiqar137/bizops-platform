using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IProductMediaService
{
    Task<ApiResponse<string>> UploadImageAsync(Guid productId, IFormFile file, CancellationToken token = default);
    Task<ApiResponse<bool>> RemoveImageAsync(Guid productId, CancellationToken token = default);
}
