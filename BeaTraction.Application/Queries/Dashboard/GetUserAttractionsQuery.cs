using BeaTraction.Application.DTOs.Dashboard.Response;
using MediatR;

namespace BeaTraction.Application.Queries.Dashboard;

public record GetUserAttractionsQuery(Guid UserId) : IRequest<List<UserAttractionDto>>;
