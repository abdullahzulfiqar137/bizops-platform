using BusinessOperations.API.DTOs.Insights;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperations.API.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public sealed class AiInsightsController : ControllerBase
{
    private readonly IAiInsightService _aiInsightService;

    public AiInsightsController(IAiInsightService aiInsightService)
    {
        _aiInsightService = aiInsightService;
    }

    /// <summary>
    /// Gets real-time AI-powered actionable insights and recommendations.
    /// </summary>
    [HttpGet("insights")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BusinessInsightResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInsights(CancellationToken cancellationToken)
    {
        var response = await _aiInsightService.GetActionableInsightsAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets executive AI business digest.
    /// </summary>
    [HttpGet("digest")]
    [ProducesResponseType(typeof(ApiResponse<AiSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDigest(CancellationToken cancellationToken)
    {
        var response = await _aiInsightService.GetExecutiveAiDigestAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Interactive ChatGPT-style AI Business Assistant conversational endpoint.
    /// </summary>
    [HttpPost("chat")]
    [ProducesResponseType(typeof(ApiResponse<AiChatMessageResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Chat([FromBody] AiChatMessageRequest request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Message) || request.Message.Length > 8_000)
        {
            return BadRequest(ApiResponse<AiChatMessageResponse>.FailureResponse("Message must contain between 1 and 8,000 characters."));
        }

        if ((request.SystemPrompt?.Length ?? 0) > 4_000 ||
            request.History is null || request.History.Count > 50 || request.History.Any(item =>
                item is null ||
                item.Content.Length > 8_000 ||
                (item.Role is not "user" and not "assistant")))
        {
            return BadRequest(ApiResponse<AiChatMessageResponse>.FailureResponse("Conversation history is invalid."));
        }

        var response = await _aiInsightService.AskAiAssistantAsync(request, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(StatusCodes.Status502BadGateway, response);
    }

    /// <summary>
    /// Gets persistent chat history stored in database.
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AiChatMessageResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
    {
        var response = await _aiInsightService.GetChatHistoryAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Clears persistent chat history from database.
    /// </summary>
    [HttpDelete("history")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ClearHistory(CancellationToken cancellationToken)
    {
        var response = await _aiInsightService.ClearChatHistoryAsync(cancellationToken);
        return Ok(response);
    }
}
