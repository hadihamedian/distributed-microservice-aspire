using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Repositories;

namespace ProductService.Application.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IProductRepository _repository;

    public GetAllProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllAsync(cancellationToken);
        return products.Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.Category)).ToList();
    }
}