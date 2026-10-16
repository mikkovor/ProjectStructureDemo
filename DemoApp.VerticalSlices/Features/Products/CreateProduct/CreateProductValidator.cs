using DemoApp.VerticalSlices.Domain;
using FluentValidation;

namespace DemoApp.VerticalSlices.Features.Products.CreateProduct;

public sealed class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Product.MaxNameLength);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}