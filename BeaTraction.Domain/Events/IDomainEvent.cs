using MediatR;

namespace BeaTraction.Domain.Events;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
