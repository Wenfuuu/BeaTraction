using BeaTraction.Application.DTOs.Registrations.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Queries.Registrations;

public class GetRegistrationByIdHandler : IRequestHandler<GetRegistrationByIdQuery, RegistrationDto>
{
    private readonly IRegistrationRepository _registrationRepository;

    public GetRegistrationByIdHandler(IRegistrationRepository registrationRepository)
    {
        _registrationRepository = registrationRepository;
    }

    public async Task<RegistrationDto> Handle(GetRegistrationByIdQuery request, CancellationToken cancellationToken)
    {
        var registration = await _registrationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (registration == null)
        {
            throw new InvalidOperationException("Registration not found");
        }

        return new RegistrationDto
        {
            Id = registration.Id,
            UserId = registration.UserId,
            ScheduleAttractionId = registration.ScheduleAttractionId,
            RegisteredAt = registration.RegisteredAt
        };
    }
}