using FluentValidation;

namespace DemoApp.CleanArchitecture.Api.Requests;

public sealed class ChangePriceRequestValidator : AbstractValidator<ChangePriceRequest>
{
    public ChangePriceRequestValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
