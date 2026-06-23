using Shared.BuildingBlocks.CQRS;

namespace ProductService.Application.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string Category
) : ICommand<Guid>;