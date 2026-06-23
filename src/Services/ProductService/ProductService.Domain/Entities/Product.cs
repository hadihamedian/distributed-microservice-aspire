using Shared.BuildingBlocks.Domain;
using ProductService.Domain.Events;

namespace ProductService.Domain.Entities;

public class Product : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public string Category { get; private set; } = string.Empty;

    private Product() { }

    public static Product Create(string name, string description, decimal price, int stockQuantity, string category)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            StockQuantity = stockQuantity,
            Category = category
        };

        product.AddDomainEvent(new ProductCreatedEvent(product.Id, product.Name, product.Category, product.Price));
        return product;
    }

    public void UpdateStock(int quantity)
    {
        StockQuantity = quantity;
    }
}