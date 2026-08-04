using BusinessOperations.API.DTOs.Businesses;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IBusinessProfileService
{
    Task<ApiResponse<BusinessProfileResponse>> GetProfileAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<BusinessProfileResponse>> UpdateProfileAsync(UpdateBusinessProfileRequest request, CancellationToken cancellationToken = default);
}
