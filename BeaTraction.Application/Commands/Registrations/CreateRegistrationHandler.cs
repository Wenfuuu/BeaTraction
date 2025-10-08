using BeaTraction.Application.DTOs.Registrations.Response;
using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Registrations;

public class CreateRegistrationHandler : IRequestHandler<CreateRegistrationCommand, RegistrationDto>
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public CreateRegistrationHandler(
        IRegistrationRepository registrationRepository,
        IUserRepository userRepository,
        IScheduleRepository scheduleRepository)
    {
        _registrationRepository = registrationRepository;
        _userRepository = userRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<RegistrationDto> Handle(CreateRegistrationCommand request, CancellationToken cancellationToken)
    {
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

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ScheduleId = request.ScheduleId,
            RegisteredAt = request.RegisteredAt
        };

        await _registrationRepository.AddAsync(registration, cancellationToken);

        return new RegistrationDto
        {
            Id = registration.Id,
            UserId = registration.UserId,
            ScheduleId = registration.ScheduleId,
            RegisteredAt = registration.RegisteredAt
        };
    }
}