using OrderService.Application.DTOs;
using Shared.BuildingBlocks.CQRS;

namespace OrderService.Application.Commands.PlaceOrder;

public record PlaceOrderCommand(Guid UserId, List<OrderItemDto> Items) : ICommand<Guid>;