using DemoApp.Layered.Models;

namespace DemoApp.Layered.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(string? search, int? pageSize, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken);
}