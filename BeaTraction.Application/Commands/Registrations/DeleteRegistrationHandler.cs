using BeaTraction.Application.Common;
using BeaTraction.Application.Interfaces;
using BeaTraction.Domain.Events.Registrations;
using BeaTraction.Domain.Interfaces;
using MediatR;

namespace BeaTraction.Application.Commands.Registrations;

public class DeleteRegistrationHandler : IRequestHandler<DeleteRegistrationCommand, bool>
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ICacheService _cacheService;
    private readonly IPublisher _publisher;

    public DeleteRegistrationHandler(
        IRegistrationRepository registrationRepository,
        ICacheService cacheService,
        IPublisher publisher)
    {
        _registrationRepository = registrationRepository;
        _cacheService = cacheService;
        _publisher = publisher;
    }

    public async Task<bool> Handle(DeleteRegistrationCommand request, CancellationToken cancellationToken)
    {
        var registration = await _registrationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (registration == null)
        {
            throw new InvalidOperationException("Registration not found");
        }

        var userId = registration.UserId;
        var scheduleAttractionId = registration.ScheduleAttractionId;
        var registrationId = registration.Id;

        await _registrationRepository.DeleteAsync(registration, cancellationToken);

        var capacityKey = CacheKeys.GetCapacity(scheduleAttractionId);
        await _cacheService.DecrementAsync(capacityKey);

        var domainEvent = new RegistrationDeletedEvent(
            registrationId,
            userId,
            scheduleAttractionId);
        
        await _publisher.Publish(domainEvent, cancellationToken);

        return true;
    }
}
