using System.Collections.Concurrent;
using DemoApp.CleanArchitecture.Application.Abstractions;
using DemoApp.CleanArchitecture.Domain.Products;

namespace DemoApp.CleanArchitecture.Infrastructure.Products;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _products = new();
    private readonly ConcurrentDictionary<Guid, long> _order = new();
    private long _sequence;

    public InMemoryProductRepository()
    {
        Store(new Product { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Demo product", Price = 9.99m });
        Store(new Product { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Office chair", Price = 149.00m });
    }

    public Task<IReadOnlyList<Product>> GetAllAsync(string? search, int? pageSize, CancellationToken cancellationToken)
    {
        IEnumerable<Product> query = _products.Values.OrderBy(p => _order[p.Id]);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (pageSize.HasValue)
        {
            query = query.Take(pageSize.Value);
        }

        return Task.FromResult<IReadOnlyList<Product>>(query.ToList());
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_products.TryGetValue(id, out var product) ? product : null);

    public Task<Product> AddAsync(Product product, CancellationToken cancellationToken)
    {
        var stored = new Product { Id = Guid.NewGuid(), Name = product.Name, Price = product.Price };
        Store(stored);
        return Task.FromResult(stored);
    }

    private void Store(Product product)
    {
        _order[product.Id] = Interlocked.Increment(ref _sequence);
        _products[product.Id] = product;
    }
}
