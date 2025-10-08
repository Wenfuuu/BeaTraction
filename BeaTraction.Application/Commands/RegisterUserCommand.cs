using BeaTraction.Application.DTOs;
using MediatR;

namespace BeaTraction.Application.Commands;

public record RegisterUserCommand(
    string Name,
    string Email,
    string Password,
    string Role = "user"
) : IRequest<UserDto>;
