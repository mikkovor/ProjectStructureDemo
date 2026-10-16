using DemoApp.Layered.DTOs;
using DemoApp.Layered.Models;
using DemoApp.Layered.Repositories;

namespace DemoApp.Layered.Services;

public sealed class ProductService(IProductRepository repository) : IProductService
{
    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(string? search, int? pageSize, CancellationToken cancellationToken)
    {
        var products = await repository.GetAllAsync(search, pageSize, cancellationToken);
        return products.Select(ToResponse).ToList();
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : ToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await repository.AddAsync(new Product { Name = request.Name, Price = request.Price }, cancellationToken);
        return ToResponse(product);
    }

    private static ProductResponse ToResponse(Product product) => new(product.Id, product.Name, product.Price);
}