using MediatR;

namespace Shared.BuildingBlocks.Domain;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    private readonly List<INotification> _domainEvents = new();
    public IReadOnlyList<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}