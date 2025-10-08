using BeaTraction.Application.DTOs;
using MediatR;

namespace BeaTraction.Application.Queries;

public record GetUserProfileQuery(string Email) : IRequest<UserDto>;
