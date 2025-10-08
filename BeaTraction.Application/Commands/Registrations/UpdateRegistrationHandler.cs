using BeaTraction.Application.DTOs.Registrations.Response;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Registrations;

public class UpdateRegistrationHandler : IRequestHandler<UpdateRegistrationCommand, RegistrationDto>
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public UpdateRegistrationHandler(
        IRegistrationRepository registrationRepository,
        IUserRepository userRepository,
        IScheduleRepository scheduleRepository)
    {
        _registrationRepository = registrationRepository;
        _userRepository = userRepository;
        _scheduleRepository = scheduleRepository;
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

        var scheduleExists = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (scheduleExists == null)
        {
            throw new InvalidOperationException("Schedule not found");
        }

        registration.UserId = request.UserId;
        registration.ScheduleId = request.ScheduleId;
        registration.RegisteredAt = request.RegisteredAt;

        await _registrationRepository.UpdateAsync(registration, cancellationToken);

        return new RegistrationDto
        {
            Id = registration.Id,
            UserId = registration.UserId,
            ScheduleId = registration.ScheduleId,
            RegisteredAt = registration.RegisteredAt
        };
    }
}
