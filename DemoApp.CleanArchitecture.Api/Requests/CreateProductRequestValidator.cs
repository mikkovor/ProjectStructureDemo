using DemoApp.CleanArchitecture.Domain.Products;
using FluentValidation;

namespace DemoApp.CleanArchitecture.Api.Requests;

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Product.MaxNameLength);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
