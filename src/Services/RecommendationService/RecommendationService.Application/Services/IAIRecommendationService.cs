namespace RecommendationService.Application.Services;

public record RecommendationDto(string Name, string Category, string Reason);

public interface IAIRecommendationService
{
    Task<List<RecommendationDto>> GetRecommendationsAsync(Guid userId, List<string> productCategories, CancellationToken cancellationToken = default);
}