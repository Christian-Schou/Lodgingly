using Lodgingly.Framework.Domain.Events;

namespace Lodgingly.Framework.Application.Messaging.Events;

public abstract class DomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    public abstract Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);

    public Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Handle((TDomainEvent)domainEvent, cancellationToken);
    }
}