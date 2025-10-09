using BeaTraction.Application.Commands.Schedules;
using BeaTraction.Application.DTOs.Schedules.Request;
using BeaTraction.Application.DTOs.Schedules.Response;
using BeaTraction.Application.Queries.Schedules;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeaTraction.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SchedulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ScheduleDto>), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<ActionResult<List<ScheduleDto>>> GetAllSchedules()
    {
        var query = new GetAllSchedulesQuery();
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult<ScheduleDto>> GetScheduleById(Guid id)
    {
        try
        {
            var query = new GetScheduleByIdQuery(id);
            var response = await _mediator.Send(query);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ScheduleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize]
    public async Task<ActionResult<ScheduleDto>> CreateSchedule([FromBody] CreateScheduleDto dto)
    {
        try
        {
            var command = new CreateScheduleCommand(
                dto.Name,
                dto.StartTime,
                dto.EndTime
            );

            var validator = new CreateScheduleValidator();
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors });
            }

            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetScheduleById), new { id = response.Id }, response);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            return BadRequest(new { errors });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult<ScheduleDto>> UpdateSchedule(Guid id, [FromBody] UpdateScheduleDto dto)
    {
        try
        {
            var command = new UpdateScheduleCommand(
                id,
                dto.Name,
                dto.StartTime,
                dto.EndTime
            );

            var validator = new UpdateScheduleValidator();
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors });
            }

            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            return BadRequest(new { errors });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult> DeleteSchedule(Guid id)
    {
        try
        {
            var command = new DeleteScheduleCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
