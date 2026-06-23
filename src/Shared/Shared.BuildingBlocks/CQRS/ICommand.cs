using MediatR;

namespace Shared.BuildingBlocks.CQRS;

public interface ICommand : IRequest
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}