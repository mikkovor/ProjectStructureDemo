using FluentValidation;

namespace DemoApp.VerticalSlices.Features.Products.GetProducts;

public sealed class GetProductsValidator : AbstractValidator<GetProductsRequest>
{
    public GetProductsValidator()
    {
        RuleFor(x => x.Search).MaximumLength(100);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).When(x => x.PageSize.HasValue);
    }
}