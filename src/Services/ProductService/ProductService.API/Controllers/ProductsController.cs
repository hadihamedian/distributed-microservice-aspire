using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Application.Queries.GetAllProducts;
using ProductService.Application.Queries.GetProductById;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProductById), new { id = productId }, new { Id = productId });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _mediator.Send(new GetAllProductsQuery());
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        if (product is null) return NotFound();
        return Ok(product);
    }
}