using BeaTraction.Application.DTOs.Users.Response;
using MediatR;

namespace BeaTraction.Application.Queries.Users;

public record GetUserProfileQuery(string Email) : IRequest<UserDto>;
