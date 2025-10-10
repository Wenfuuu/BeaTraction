using BeaTraction.Application.DTOs.Dashboard.Response;
using BeaTraction.Application.Queries.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeaTraction.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("attraction-stats")]
    [ProducesResponseType(typeof(List<AttractionStatsDto>), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<ActionResult<List<AttractionStatsDto>>> GetAttractionStats()
    {
        var query = new GetAttractionStatsQuery();
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("user-attractions/{userId}")]
    [ProducesResponseType(typeof(List<UserAttractionDto>), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<ActionResult<List<UserAttractionDto>>> GetUserAttractions(Guid userId)
    {
        var query = new GetUserAttractionsQuery(userId);
        var response = await _mediator.Send(query);
        return Ok(response);
    }
}
