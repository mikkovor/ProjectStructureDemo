using DemoApp.Layered.DTOs;
using DemoApp.Layered.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DemoApp.Layered.Controllers;

[ApiController]
[Route("products")]
public sealed class ProductsController(
    IProductService service,
    IValidator<GetProductsQuery> queryValidator,
    IValidator<CreateProductRequest> createValidator,
    IValidator<ChangePriceRequest> changePriceValidator) : ControllerBase
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

    [HttpPut("{id:guid}/price")]
    public async Task<ActionResult<ProductResponse>> ChangePrice(Guid id, ChangePriceRequest request, CancellationToken cancellationToken)
    {
        var validation = await changePriceValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation);
        }

        var result = await service.ChangePriceAsync(id, request, cancellationToken);
        return result.Status switch
        {
            ChangePriceStatus.Changed => Ok(result.Product),
            ChangePriceStatus.NotFound => NotFound(),
            _ => Problem(
                title: "Price increase too large",
                detail: "A price may not be increased by more than 50% at once.",
                statusCode: StatusCodes.Status422UnprocessableEntity)
        };
    }

    private ActionResult ValidationProblem(FluentValidation.Results.ValidationResult validation)
    {
        var errors = validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        return ValidationProblem(new ValidationProblemDetails(errors));
    }
}