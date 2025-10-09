using BeaTraction.Application.Commands.ScheduleAttractions;
using BeaTraction.Application.DTOs.ScheduleAttractions.Request;
using BeaTraction.Application.DTOs.ScheduleAttractions.Response;
using BeaTraction.Application.Queries.ScheduleAttractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeaTraction.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleAttractionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ScheduleAttractionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ScheduleAttractionDto>), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> GetAllScheduleAttractions()
    {
        var query = new GetAllScheduleAttractionsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ScheduleAttractionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> GetScheduleAttractionById(Guid id)
    {
        try
        {
            var query = new GetScheduleAttractionByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("schedule/{scheduleId}")]
    [ProducesResponseType(typeof(List<ScheduleAttractionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> GetScheduleAttractionByScheduleId(Guid scheduleId)
    {
        var query = new GetScheduleAttractionsByScheduleIdQuery { ScheduleId = scheduleId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("attraction/{attractionId}")]
    [ProducesResponseType(typeof(List<ScheduleAttractionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> GetByAttractionId(Guid attractionId)
    {
        var query = new GetScheduleAttractionsByAttractionIdQuery { AttractionId = attractionId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ScheduleAttractionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateScheduleAttraction([FromBody] CreateScheduleAttractionDto dto)
    {
        try
        {
            var command = new CreateScheduleAttractionCommand { Data = dto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetScheduleAttractionById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteScheduleAttraction(Guid id)
    {
        try
        {
            var command = new DeleteScheduleAttractionCommand { Id = id };
            var result = await _mediator.Send(command);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
