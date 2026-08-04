using BusinessOperations.API.DTOs.Insights;
using BusinessOperations.API.Models;

namespace BusinessOperations.API.Interfaces.Services;

public interface IAiInsightService
{
    Task<ApiResponse<IEnumerable<BusinessInsightResponse>>> GetActionableInsightsAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<AiSummaryResponse>> GetExecutiveAiDigestAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<AiChatMessageResponse>> AskAiAssistantAsync(AiChatMessageRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<AiChatMessageResponse>>> GetChatHistoryAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> ClearChatHistoryAsync(CancellationToken cancellationToken = default);
}
