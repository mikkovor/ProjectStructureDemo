using DemoApp.VerticalSlices.Domain;

namespace DemoApp.VerticalSlices.Data;

public sealed class ProductStore
{
    private readonly List<Product> _products =
    [
        new(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Demo product", 9.99m),
        new(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Office chair", 149.00m)
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

    public Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            _products.Add(Copy(product));
        }

        return Task.CompletedTask;
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

    private static Product Copy(Product p) => new(p.Id, p.Name, p.Price);
}