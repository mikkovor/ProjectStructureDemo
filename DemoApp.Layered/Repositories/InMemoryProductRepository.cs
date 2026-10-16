using DemoApp.Layered.Models;

namespace DemoApp.Layered.Repositories;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products =
    [
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Demo product", Price = 9.99m },
        new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Office chair", Price = 149.00m }
    ];

    private readonly Lock _lock = new();

    public Task<IReadOnlyList<Product>> GetAllAsync(string? search, int? pageSize, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            IEnumerable<Product> query = _products;

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (pageSize.HasValue)
            {
                query = query.Take(pageSize.Value);
            }

            return Task.FromResult<IReadOnlyList<Product>>(query.Select(Copy).ToList());
        }
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product is null ? null : Copy(product));
        }
    }

    public Task<Product> AddAsync(Product product, CancellationToken cancellationToken)
    {
        var stored = new Product { Id = Guid.NewGuid(), Name = product.Name, Price = product.Price };
        lock (_lock)
        {
            _products.Add(stored);
        }

        return Task.FromResult(Copy(stored));
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            var index = _products.FindIndex(p => p.Id == product.Id);
            if (index < 0)
            {
                throw new InvalidOperationException($"Product {product.Id} does not exist.");
            }

            _products[index] = Copy(product);
        }

        return Task.CompletedTask;
    }

    private static Product Copy(Product p) => new() { Id = p.Id, Name = p.Name, Price = p.Price };
}
