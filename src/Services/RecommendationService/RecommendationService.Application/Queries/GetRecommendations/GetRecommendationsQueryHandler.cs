using MediatR;
using RecommendationService.Application.Services;

namespace RecommendationService.Application.Queries.GetRecommendations;

public class GetRecommendationsQueryHandler : IRequestHandler<GetRecommendationsQuery, List<RecommendationDto>>
{
    private readonly IAIRecommendationService _aiService;

    public GetRecommendationsQueryHandler(IAIRecommendationService aiService)
    {
        _aiService = aiService;
    }

    public async Task<List<RecommendationDto>> Handle(GetRecommendationsQuery request, CancellationToken cancellationToken)
    {
        return await _aiService.GetRecommendationsAsync(request.UserId, request.ProductCategories, cancellationToken);
    }
}