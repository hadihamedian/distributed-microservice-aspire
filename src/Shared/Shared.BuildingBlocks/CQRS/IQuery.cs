using MediatR;

namespace Shared.BuildingBlocks.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}