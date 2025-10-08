using BeaTraction.Application.Commands;
using BeaTraction.Application.DTOs;
using BeaTraction.Application.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeaTraction.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        var query = new GetAllUsersQuery();
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> RegisterUser([FromBody] RegisterUserDto dto)
    {
        try
        {
            var command = new RegisterUserCommand(
                dto.Name,
                dto.Email,
                dto.Password,
                dto.Role
            );

            var validator = new RegisterUserValidator();
            var validationResult = await validator.ValidateAsync(command);
        
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors });
            }

            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAllUsers), new { id = response.Id }, response);
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

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
    {
        try
        {
            var command = new LoginCommand(dto.Email, dto.Password);

            var validator = new LoginValidator();
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors });
            }

            var response = await _mediator.Send(command);
            
            var expirationMinutes = int.TryParse(
                Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES"), 
                out var minutes) 
                ? minutes 
                : 60;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes)
            };
            
            Response.Cookies.Append("authToken", response.Token, cookieOptions);
            
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Logout()
    {
        Response.Cookies.Delete("authToken");
        
        return Ok(new { message = "Logged out successfully" });
    }
}
