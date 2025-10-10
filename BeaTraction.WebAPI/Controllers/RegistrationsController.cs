using BeaTraction.Application.Commands.Registrations;
using BeaTraction.Application.DTOs.Registrations.Request;
using BeaTraction.Application.DTOs.Registrations.Response;
using BeaTraction.Application.Queries.Registrations;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeaTraction.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public RegistrationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RegistrationDto>), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<ActionResult<List<RegistrationDto>>> GetAllRegistrations()
    {
        var query = new GetAllRegistrationsQuery();
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RegistrationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult<RegistrationDto>> GetRegistrationById(Guid id)
    {
        var query = new GetRegistrationByIdQuery(id);
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<RegistrationDto>), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<ActionResult<List<RegistrationDto>>> GetRegistrationsByUserId(Guid userId)
    {
        var query = new GetRegistrationsByUserIdQuery(userId);
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RegistrationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize]
    public async Task<ActionResult<RegistrationDto>> CreateRegistration([FromBody] CreateRegistrationDto dto)
    {
        try
        {
            var command = new CreateRegistrationCommand(
                dto.UserId,
                dto.ScheduleAttractionId,
                dto.RegisteredAt
            );
            
            var validator = new CreateRegistrationValidator();
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors });
            }
            
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetRegistrationById), new { id = response.Id }, response);
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

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult> DeleteRegistration(Guid id)
    {
        try
        {
            var command = new DeleteRegistrationCommand(id);

            var validator = new DeleteRegistrationValidator();
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors });
            }

            await _mediator.Send(command);
            return NoContent();
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
}