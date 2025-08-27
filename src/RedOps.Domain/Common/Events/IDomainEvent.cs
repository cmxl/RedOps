using Mediator;

namespace RedOps.Domain.Common.Events;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}