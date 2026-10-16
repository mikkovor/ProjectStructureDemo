using DemoApp.Layered.DTOs;
using FluentValidation;

namespace DemoApp.Layered.Validators;

public sealed class ChangePriceRequestValidator : AbstractValidator<ChangePriceRequest>
{
    public ChangePriceRequestValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
