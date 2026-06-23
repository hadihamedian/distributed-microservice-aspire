using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Repositories;

namespace ProductService.Application.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _repository;

    public GetProductByIdQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null) return null;

        return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.StockQuantity, product.Category);
    }
}