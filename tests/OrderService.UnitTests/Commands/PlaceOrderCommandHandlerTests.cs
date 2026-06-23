using FluentAssertions;
using MassTransit;
using Moq;
using OrderService.Application.Commands.PlaceOrder;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;

namespace OrderService.UnitTests.Commands;

public class PlaceOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly PlaceOrderCommandHandler _handler;

    public PlaceOrderCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new PlaceOrderCommandHandler(_repositoryMock.Object, _publishEndpointMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldPlaceOrder_WhenValidItems()
    {
        var items = new List<OrderService.Application.DTOs.OrderItemDto> { new(Guid.NewGuid(), 2, 100) };
        var command = new PlaceOrderCommand(Guid.NewGuid(), items);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenOrderItemsIsEmpty()
    {
        var command = new PlaceOrderCommand(Guid.NewGuid(), new List<OrderService.Application.DTOs.OrderItemDto>());

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Cannot place order if OrderItems is empty.");
    }
}