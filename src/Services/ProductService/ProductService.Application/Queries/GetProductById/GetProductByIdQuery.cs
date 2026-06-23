using Shared.BuildingBlocks.CQRS;
using ProductService.Application.DTOs;

namespace ProductService.Application.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<ProductDto?>;