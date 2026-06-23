using OrderService.Application.DTOs;
using Shared.BuildingBlocks.CQRS;

namespace OrderService.Application.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IQuery<OrderDto?>;