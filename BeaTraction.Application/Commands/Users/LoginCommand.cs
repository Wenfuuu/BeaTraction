using BeaTraction.Application.DTOs.Users.Response;
using MediatR;

namespace BeaTraction.Application.Commands.Users;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResponseDto>;
