using BeaTraction.Application.DTOs.Users.Response;
using MediatR;

namespace BeaTraction.Application.Commands.Users;

public record RegisterUserCommand(
    string Name,
    string Email,
    string Password,
    string Role = "user"
) : IRequest<UserDto>;
