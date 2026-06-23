using Shared.BuildingBlocks.CQRS;
using RecommendationService.Application.Services;

namespace RecommendationService.Application.Queries.GetRecommendations;

public record GetRecommendationsQuery(Guid UserId, List<string> ProductCategories) : IQuery<List<RecommendationDto>>;