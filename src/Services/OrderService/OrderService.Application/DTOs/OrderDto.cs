namespace OrderService.Application.DTOs;

public record OrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice);
public record OrderDto(Guid Id, Guid UserId, string Status, decimal TotalAmount, List<OrderItemDto> Items);