using BeaTraction.Application.DTOs.Registrations.Response;
using BeaTraction.Domain.Entities;
using BeaTraction.Domain.Events.Registrations;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Registrations;

public class CreateRegistrationHandler : IRequestHandler<CreateRegistrationCommand, RegistrationDto>
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IScheduleAttractionRepository _scheduleAttractionRepository;
    private readonly IPublisher _publisher;

    public CreateRegistrationHandler(
        IRegistrationRepository registrationRepository,
        IUserRepository userRepository,
        IScheduleAttractionRepository scheduleAttractionRepository,
        IPublisher publisher)
    {
        _registrationRepository = registrationRepository;
        _userRepository = userRepository;
        _scheduleAttractionRepository = scheduleAttractionRepository;
        _publisher = publisher;
    }

    public async Task<RegistrationDto> Handle(CreateRegistrationCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (userExists == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var scheduleAttraction = await _scheduleAttractionRepository.GetByIdAsync(request.ScheduleAttractionId, cancellationToken);
        if (scheduleAttraction == null)
        {
            throw new InvalidOperationException("ScheduleAttraction not found");
        }

        var currentRegistrationCount = scheduleAttraction.Registrations?.Count ?? 0;
        var attractionCapacity = scheduleAttraction.Attraction?.Capacity ?? 0;

        if (currentRegistrationCount >= attractionCapacity)
        {
            throw new InvalidOperationException(
                $"Registration failed: This attraction has reached its maximum capacity of {attractionCapacity}. " +
                $"Currently {currentRegistrationCount} registrations exist.");
        }

        var existingRegistration = scheduleAttraction.Registrations?
            .FirstOrDefault(r => r.UserId == request.UserId);
        
        if (existingRegistration != null)
        {
            throw new InvalidOperationException("Registration failed: You have already registered for this schedule.");
        }

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ScheduleAttractionId = request.ScheduleAttractionId,
            RegisteredAt = request.RegisteredAt
        };

        await _registrationRepository.AddAsync(registration, cancellationToken);

        var domainEvent = new RegistrationCreatedEvent(
            registration.Id,
            registration.UserId,
            registration.ScheduleAttractionId,
            registration.RegisteredAt);
        
        await _publisher.Publish(domainEvent, cancellationToken);

        return new RegistrationDto
        {
            Id = registration.Id,
            UserId = registration.UserId,
            ScheduleAttractionId = registration.ScheduleAttractionId,
            RegisteredAt = registration.RegisteredAt
        };
    }
}