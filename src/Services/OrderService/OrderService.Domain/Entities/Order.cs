using OrderService.Domain.Enums;
using OrderService.Domain.Events;
using Shared.BuildingBlocks.Domain;

namespace OrderService.Domain.Entities;

public class Order : AggregateRoot
{
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Sum(i => i.Quantity * i.UnitPrice);

    private Order() { }

    public static Order Create(Guid userId, List<(Guid ProductId, int Quantity, decimal UnitPrice)> items)
    {
        if (items is null || items.Count == 0)
            throw new ArgumentException("Cannot place order if OrderItems is empty.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in items)
        {
            order._items.Add(new OrderItem(item.ProductId, item.Quantity, item.UnitPrice));
        }

        order.AddDomainEvent(new OrderPlacedEvent(order.Id, order.UserId, order.TotalAmount));
        return order;
    }
}