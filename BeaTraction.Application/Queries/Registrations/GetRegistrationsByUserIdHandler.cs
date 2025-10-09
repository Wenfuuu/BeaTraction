using BeaTraction.Application.DTOs.Registrations.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.Registrations;

public class GetRegistrationsByUserIdHandler : IRequestHandler<GetRegistrationsByUserIdQuery, List<RegistrationDto>>
{
    private readonly IRegistrationRepository _registrationRepository;
    
    public GetRegistrationsByUserIdHandler(IRegistrationRepository registrationRepository)
    {
        _registrationRepository = registrationRepository;
    }

    public async Task<List<RegistrationDto>> Handle(GetRegistrationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var registrations = await _registrationRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return registrations.Select(r => new RegistrationDto
        {
            Id = r.Id,
            UserId = r.UserId,
            ScheduleAttractionId = r.ScheduleAttractionId,
            RegisteredAt = r.RegisteredAt
        }).ToList();
    }
}
