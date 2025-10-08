using BeaTraction.Application.Commands.Attractions;
using BeaTraction.Application.DTOs.Attractions.Request;
using BeaTraction.Application.DTOs.Attractions.Response;
using BeaTraction.Application.Queries.Attractions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeaTraction.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttractionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttractionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<AttractionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AttractionDto>>> GetAllAttractions()
    {
        var query = new GetAllAttractionsQuery();
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AttractionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult<AttractionDto>> GetAttractionById(Guid id)
    {
        try
        {
            var query = new GetAttractionByIdQuery(id);
            var response = await _mediator.Send(query);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(AttractionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize]
    public async Task<ActionResult<AttractionDto>> CreateAttraction([FromBody] CreateAttractionDto dto)
    {
        try
        {
            var command = new CreateAttractionCommand(
                dto.Name,
                dto.Description,
                dto.ImageUrl,
                dto.Capacity
            );

            var validator = new CreateAttractionValidator();
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors });
            }

            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAttractionById), new { id = response.Id }, response);
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
    [ProducesResponseType(typeof(AttractionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult<AttractionDto>> UpdateAttraction(Guid id, [FromBody] UpdateAttractionDto dto)
    {
        try
        {
            var command = new UpdateAttractionCommand(
                id,
                dto.Name,
                dto.Description,
                dto.ImageUrl,
                dto.Capacity
            );

            var validator = new UpdateAttractionValidator();
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult> DeleteAttraction(Guid id)
    {
        try
        {
            var command = new DeleteAttractionCommand(id);
            await _mediator.Send(command);
            return Ok(new { message = "Attraction deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
