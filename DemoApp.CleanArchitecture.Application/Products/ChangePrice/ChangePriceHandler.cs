using DemoApp.CleanArchitecture.Application.Abstractions;
using DemoApp.CleanArchitecture.Domain.Products;

namespace DemoApp.CleanArchitecture.Application.Products.ChangePrice;

public abstract record ChangePriceResult
{
    public sealed record Changed(Product Product) : ChangePriceResult;
    public sealed record NotFound : ChangePriceResult;
    public sealed record IncreaseTooLarge : ChangePriceResult;
}

public sealed class ChangePriceHandler(IProductRepository repository)
{
    public async Task<ChangePriceResult> HandleAsync(ChangePriceCommand command, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (product is null)
        {
            return new ChangePriceResult.NotFound();
        }

        // The rule itself is owned by the domain entity.
        if (!product.TryChangePrice(command.Price))
        {
            return new ChangePriceResult.IncreaseTooLarge();
        }

        await repository.UpdateAsync(product, cancellationToken);
        return new ChangePriceResult.Changed(product);
    }
}
