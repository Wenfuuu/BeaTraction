using BeaTraction.Application.DTOs;
using MediatR;

namespace BeaTraction.Application.Queries;

public record GetAllUsersQuery : IRequest<List<UserDto>>;
