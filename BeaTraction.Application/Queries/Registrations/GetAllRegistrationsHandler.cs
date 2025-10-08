using BeaTraction.Application.DTOs.Registrations.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.Registrations;

public class GetAllRegistrationsHandler : IRequestHandler<GetAllRegistrationsQuery, List<RegistrationDto>>
{
    private readonly IRegistrationRepository _registrationRepository;
    
    public GetAllRegistrationsHandler(IRegistrationRepository registrationRepository)
    {
        _registrationRepository = registrationRepository;
    }

    public async Task<List<RegistrationDto>> Handle(GetAllRegistrationsQuery request, CancellationToken cancellationToken)
    {
        var registrations = await _registrationRepository.GetAllAsync(cancellationToken);

        return registrations.Select(r => new RegistrationDto
        {
            Id = r.Id,
            UserId = r.UserId,
            ScheduleId = r.ScheduleId,
            RegisteredAt = r.RegisteredAt
        }).ToList();
    }
}