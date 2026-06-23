using MassTransit;
using MediatR;
using Shared.BuildingBlocks.CQRS;
using Shared.Contracts.Events;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;

namespace ProductService.Application.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateProductCommandHandler(IProductRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Description, request.Price, request.StockQuantity, request.Category);

        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        var integrationEvent = new ProductCreatedIntegrationEvent(
            product.Id,
            product.Name,
            product.Category,
            product.Price,
            DateTime.UtcNow
        );

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        return product.Id;
    }
}