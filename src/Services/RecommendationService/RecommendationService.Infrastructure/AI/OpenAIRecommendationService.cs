using System.Text.Json;
using Microsoft.SemanticKernel.ChatCompletion;
using RecommendationService.Application.Services;

namespace RecommendationService.Infrastructure.AI;

public class OpenAIRecommendationService : IAIRecommendationService
{
    private readonly IChatCompletionService _chatCompletionService;

    public OpenAIRecommendationService(IChatCompletionService chatCompletionService)
    {
        _chatCompletionService = chatCompletionService;
    }

    public async Task<List<RecommendationDto>> GetRecommendationsAsync(Guid userId, List<string> productCategories, CancellationToken cancellationToken = default)
    {
        var history = new ChatHistory("You are a product recommendation engine for an e-commerce platform. Given the user's browsing history by category, recommend 5 products they might like. Respond ONLY in JSON format: [{\"name\": \"...\", \"category\": \"...\", \"reason\": \"...\"}]");

        var categoriesText = string.Join(", ", productCategories);
        history.AddUserMessage($"User ID: {userId}. Recently viewed categories: {categoriesText}");

        var response = await _chatCompletionService.GetChatMessageContentAsync(history, cancellationToken: cancellationToken);
        var content = response.Content ?? "[]";

        if (content.StartsWith("```json"))
        {
            content = content.Replace("```json", "").Replace("```", "").Trim();
        }

        try
        {
            return JsonSerializer.Deserialize<List<RecommendationDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<RecommendationDto>();
        }
        catch
        {
            return new List<RecommendationDto>();
        }
    }
}