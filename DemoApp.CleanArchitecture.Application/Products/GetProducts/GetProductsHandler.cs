using DemoApp.CleanArchitecture.Application.Abstractions;
using DemoApp.CleanArchitecture.Domain.Products;

namespace DemoApp.CleanArchitecture.Application.Products.GetProducts;

public sealed class GetProductsHandler(IProductRepository repository)
{
    public Task<IReadOnlyList<Product>> HandleAsync(GetProductsQuery query, CancellationToken cancellationToken) =>
        repository.GetAllAsync(query.Search, query.PageSize, cancellationToken);
}
