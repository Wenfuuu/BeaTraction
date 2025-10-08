using BeaTraction.Application.DTOs;
using MediatR;

namespace BeaTraction.Application.Commands;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResponseDto>;
