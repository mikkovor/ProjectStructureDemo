namespace DemoApp.VerticalSlices.Products;

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

    public Task<Product> AddAsync(string name, decimal price, CancellationToken cancellationToken)
    {
        var product = new Product { Id = Guid.NewGuid(), Name = name, Price = price };
        lock (_lock)
        {
            _products.Add(product);
        }

        return Task.FromResult(Copy(product));
    }

    private static Product Copy(Product p) => new() { Id = p.Id, Name = p.Name, Price = p.Price };
}