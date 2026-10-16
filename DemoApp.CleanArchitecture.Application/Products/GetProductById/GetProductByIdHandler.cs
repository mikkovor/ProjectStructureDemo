using DemoApp.CleanArchitecture.Application.Abstractions;
using DemoApp.CleanArchitecture.Domain.Products;

namespace DemoApp.CleanArchitecture.Application.Products.GetProductById;

public abstract record GetProductByIdResult
{
    public sealed record Found(Product Product) : GetProductByIdResult;
    public sealed record NotFound : GetProductByIdResult;
}

public sealed class GetProductByIdHandler(IProductRepository repository)
{
    public async Task<GetProductByIdResult> HandleAsync(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(query.Id, cancellationToken);
        return product is null ? new GetProductByIdResult.NotFound() : new GetProductByIdResult.Found(product);
    }
}
