using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecommendationService.Application.Queries.GetRecommendations;

namespace RecommendationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecommendationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetRecommendations([FromQuery] Guid userId, [FromQuery] string categories)
    {
        var categoryList = categories?.Split(',').Select(c => c.Trim()).ToList() ?? new List<string>();
        var recommendations = await _mediator.Send(new GetRecommendationsQuery(userId, categoryList));
        return Ok(recommendations);
    }
}