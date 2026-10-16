using DemoApp.Layered.DTOs;
using DemoApp.Layered.Models;
using DemoApp.Layered.Repositories;

namespace DemoApp.Layered.Services;

public sealed class ProductService(IProductRepository repository) : IProductService
{
    // Business rules live in the service layer.
    private const decimal MaxIncreaseFactor = 1.5m;

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

    public async Task<ChangePriceResult> ChangePriceAsync(Guid id, ChangePriceRequest request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return new ChangePriceResult(ChangePriceStatus.NotFound);
        }

        if (request.Price > product.Price * MaxIncreaseFactor)
        {
            return new ChangePriceResult(ChangePriceStatus.IncreaseTooLarge);
        }

        product.Price = request.Price;
        await repository.UpdateAsync(product, cancellationToken);
        return new ChangePriceResult(ChangePriceStatus.Changed, ToResponse(product));
    }

    private static ProductResponse ToResponse(Product product) => new(product.Id, product.Name, product.Price);
}