using BeaTraction.Application.DTOs.Registrations.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Registrations;

public class UpdateRegistrationHandler : IRequestHandler<UpdateRegistrationCommand, RegistrationDto>
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IScheduleAttractionRepository _scheduleAttractionRepository;

    public UpdateRegistrationHandler(
        IRegistrationRepository registrationRepository,
        IUserRepository userRepository,
        IScheduleAttractionRepository scheduleAttractionRepository)
    {
        _registrationRepository = registrationRepository;
        _userRepository = userRepository;
        _scheduleAttractionRepository = scheduleAttractionRepository;
    }

    public async Task<RegistrationDto> Handle(UpdateRegistrationCommand request, CancellationToken cancellationToken)
    {
        var registration = await _registrationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (registration == null)
        {
            throw new InvalidOperationException("Registration not found");
        }

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

        registration.UserId = request.UserId;
        registration.ScheduleAttractionId = request.ScheduleAttractionId;
        registration.RegisteredAt = request.RegisteredAt;

        await _registrationRepository.UpdateAsync(registration, cancellationToken);

        return new RegistrationDto
        {
            Id = registration.Id,
            UserId = registration.UserId,
            ScheduleAttractionId = registration.ScheduleAttractionId,
            RegisteredAt = registration.RegisteredAt
        };
    }
}
