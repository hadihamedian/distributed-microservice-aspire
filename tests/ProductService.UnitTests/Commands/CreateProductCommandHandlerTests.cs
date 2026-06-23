using FluentAssertions;
using MassTransit;
using Moq;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using Shared.Contracts.Events;

namespace ProductService.UnitTests.Commands;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new CreateProductCommandHandler(_repositoryMock.Object, _publishEndpointMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateProduct_WhenValidCommand()
    {
        var command = new CreateProductCommand("Laptop", "Gaming Laptop", 1500, 10, "Electronics");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPublishIntegrationEvent_AfterProductCreated()
    {
        var command = new CreateProductCommand("Mouse", "Wireless Mouse", 50, 100, "Accessories");

        await _handler.Handle(command, CancellationToken.None);

        _publishEndpointMock.Verify(p => p.Publish(It.IsAny<ProductCreatedIntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenNameIsEmpty()
    {
        var command = new CreateProductCommand("", "Description", 10, 5, "Category");

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Name cannot be empty");
    }
}