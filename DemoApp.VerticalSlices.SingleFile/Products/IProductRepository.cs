namespace DemoApp.VerticalSlices.SingleFile.Products;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(string? search, int? pageSize, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Product> AddAsync(string name, decimal price, CancellationToken cancellationToken);
}