using MediatR;

namespace Shared.BuildingBlocks.CQRS;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : INotification
{
}