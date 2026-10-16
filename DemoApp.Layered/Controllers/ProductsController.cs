using DemoApp.Layered.DTOs;
using DemoApp.Layered.Services;
using DemoApp.Layered.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DemoApp.Layered.Controllers;

[ApiController]
[Route("products")]
public sealed class ProductsController(
    IProductService service,
    IValidator<GetProductsQuery> queryValidator,
    IValidator<CreateProductRequest> createValidator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(
        [FromQuery] string? search, [FromQuery] int? pageSize, CancellationToken cancellationToken)
    {
        var validation = await queryValidator.ValidateAsync(new GetProductsQuery(search, pageSize), cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation);
        }

        return Ok(await service.GetAllAsync(search, pageSize, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await service.GetByIdAsync(id, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var validation = await createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation);
        }

        var created = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    private ActionResult ValidationProblem(FluentValidation.Results.ValidationResult validation)
    {
        var errors = validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        return ValidationProblem(new ValidationProblemDetails(errors));
    }
}