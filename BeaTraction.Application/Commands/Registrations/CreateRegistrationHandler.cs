using BeaTraction.Application.DTOs.Registrations.Response;
using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Registrations;

public class CreateRegistrationHandler : IRequestHandler<CreateRegistrationCommand, RegistrationDto>
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IScheduleAttractionRepository _scheduleAttractionRepository;

    public CreateRegistrationHandler(
        IRegistrationRepository registrationRepository,
        IUserRepository userRepository,
        IScheduleAttractionRepository scheduleAttractionRepository)
    {
        _registrationRepository = registrationRepository;
        _userRepository = userRepository;
        _scheduleAttractionRepository = scheduleAttractionRepository;
    }

    public async Task<RegistrationDto> Handle(CreateRegistrationCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (userExists == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var scheduleAttractionExists = await _scheduleAttractionRepository.GetByIdAsync(request.ScheduleAttractionId, cancellationToken);
        if (scheduleAttractionExists == null)
        {
            throw new InvalidOperationException("ScheduleAttraction not found");
        }

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ScheduleAttractionId = request.ScheduleAttractionId,
            RegisteredAt = request.RegisteredAt
        };

        await _registrationRepository.AddAsync(registration, cancellationToken);

        return new RegistrationDto
        {
            Id = registration.Id,
            UserId = registration.UserId,
            ScheduleAttractionId = registration.ScheduleAttractionId,
            RegisteredAt = registration.RegisteredAt
        };
    }
}