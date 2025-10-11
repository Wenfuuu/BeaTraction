using BeaTraction.Application.Common;
using BeaTraction.Application.DTOs.Registrations.Response;
using BeaTraction.Application.Interfaces;
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
    private readonly ICacheService _cacheService;
    private readonly IPublisher _publisher;
    private const int MaxRetryAttempts = 3;

    public CreateRegistrationHandler(
        IRegistrationRepository registrationRepository,
        IUserRepository userRepository,
        IScheduleAttractionRepository scheduleAttractionRepository,
        ICacheService cacheService,
        IPublisher publisher)
    {
        _registrationRepository = registrationRepository;
        _userRepository = userRepository;
        _scheduleAttractionRepository = scheduleAttractionRepository;
        _cacheService = cacheService;
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

        var attractionCapacity = scheduleAttraction.Attraction?.Capacity ?? 0;
        if (attractionCapacity == 0)
        {
            throw new InvalidOperationException("Attraction capacity is not set");
        }

        var existingRegistration = scheduleAttraction.Registrations?
            .FirstOrDefault(r => r.UserId == request.UserId);
        
        if (existingRegistration != null)
        {
            throw new InvalidOperationException("Registration failed: You have already registered for this schedule.");
        }

        // redis optimistic locking with retry
        var capacityKey = CacheKeys.GetCapacity(request.ScheduleAttractionId);
        var lockKey = CacheKeys.GetRegistrationLock(request.ScheduleAttractionId);
        
        for (int attempt = 0; attempt < MaxRetryAttempts; attempt++)
        {
            try
            {
                // try to acquire lock (5 sec expiration)
                var lockAcquired = await _cacheService.SetIfNotExistsAsync(
                    lockKey, 
                    request.UserId.ToString(), 
                    TimeSpan.FromSeconds(5)
                );

                if (!lockAcquired)
                {
                    // lock is held by another request, wait and retry
                    await Task.Delay(100 * (attempt + 1), cancellationToken);
                    continue;
                }

                try
                {
                    var currentCountStr = await _cacheService.GetStringAsync(capacityKey);
                    long currentCount;

                    if (string.IsNullOrEmpty(currentCountStr))
                    {
                        currentCount = scheduleAttraction.Registrations?.Count ?? 0;
                        await _cacheService.SetStringAsync(capacityKey, currentCount.ToString(), TimeSpan.FromHours(24));
                    }
                    else
                    {
                        currentCount = long.Parse(currentCountStr);
                    }

                    if (currentCount >= attractionCapacity)
                    {
                        throw new InvalidOperationException(
                            $"Registration failed: This attraction has reached its maximum capacity of {attractionCapacity}. " +
                            $"Currently {currentCount} registrations exist.");
                    }

                    var registration = new Registration
                    {
                        Id = Guid.NewGuid(),
                        UserId = request.UserId,
                        ScheduleAttractionId = request.ScheduleAttractionId,
                        RegisteredAt = request.RegisteredAt
                    };

                    await _registrationRepository.AddAsync(registration, cancellationToken);

                    // increment capacity counter in redis
                    await _cacheService.IncrementAsync(capacityKey);

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
                finally
                {
                    await _cacheService.RemoveAsync(lockKey);
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (attempt == MaxRetryAttempts - 1)
                {
                    throw new InvalidOperationException(
                        "Registration failed after multiple attempts. Please try again.", ex);
                }
                
                await Task.Delay(100 * (attempt + 1), cancellationToken);
            }
        }

        throw new InvalidOperationException("Registration failed: Unable to acquire lock after multiple attempts.");
    }
}