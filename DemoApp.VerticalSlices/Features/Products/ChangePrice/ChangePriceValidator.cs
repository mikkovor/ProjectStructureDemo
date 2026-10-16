using FluentValidation;

namespace DemoApp.VerticalSlices.Features.Products.ChangePrice;

public sealed class ChangePriceValidator : AbstractValidator<ChangePriceRequest>
{
    public ChangePriceValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
