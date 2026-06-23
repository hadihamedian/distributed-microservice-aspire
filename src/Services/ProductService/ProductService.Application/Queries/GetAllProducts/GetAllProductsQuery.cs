using Shared.BuildingBlocks.CQRS;
using ProductService.Application.DTOs;

namespace ProductService.Application.Queries.GetAllProducts;

public record GetAllProductsQuery() : IQuery<IReadOnlyList<ProductDto>>;