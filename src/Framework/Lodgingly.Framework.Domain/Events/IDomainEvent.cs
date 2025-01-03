namespace Lodgingly.Framework.Domain.Events;

public interface IDomainEvent
{
    Guid Id { get; }

    DateTime HappenedAtUtc { get; }
}