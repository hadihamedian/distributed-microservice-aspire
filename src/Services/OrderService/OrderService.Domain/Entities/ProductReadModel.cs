namespace OrderService.Domain.Entities;

// Anti-Corruption Layer: Local copy of Product data
public class ProductReadModel
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    private ProductReadModel() { }

    public ProductReadModel(Guid id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Price = price;
    }
}