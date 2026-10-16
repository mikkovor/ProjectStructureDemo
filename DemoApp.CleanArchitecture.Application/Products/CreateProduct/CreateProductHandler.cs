using DemoApp.CleanArchitecture.Application.Abstractions;
using DemoApp.CleanArchitecture.Domain.Products;

namespace DemoApp.CleanArchitecture.Application.Products.CreateProduct;

public sealed class CreateProductHandler(IProductRepository repository)
{
    public Task<Product> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken) =>
        repository.AddAsync(new Product { Name = command.Name, Price = command.Price }, cancellationToken);
}
