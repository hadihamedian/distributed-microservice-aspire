using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.PlaceOrder;
using OrderService.Application.Queries.GetOrderById;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, new { Id = orderId });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id));
        if (order is null) return NotFound();
        return Ok(order);
    }
}