using DemoApp.CleanArchitecture.Domain.Products;

namespace DemoApp.CleanArchitecture.Api.Responses;

public sealed record ProductResponse(Guid Id, string Name, decimal Price)
{
    public static ProductResponse From(Product product) => new(product.Id, product.Name, product.Price);
}
