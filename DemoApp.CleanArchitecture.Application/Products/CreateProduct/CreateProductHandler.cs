using DemoApp.CleanArchitecture.Application.Abstractions;
using DemoApp.CleanArchitecture.Domain.Products;

namespace DemoApp.CleanArchitecture.Application.Products.CreateProduct;

public sealed class CreateProductHandler(IProductRepository repository)
{
    public async Task<Product> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = Product.Create(command.Name, command.Price);
        await repository.AddAsync(product, cancellationToken);
        return product;
    }
}
