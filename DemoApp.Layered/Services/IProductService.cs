using DemoApp.Layered.DTOs;

namespace DemoApp.Layered.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponse>> GetAllAsync(string? search, int? pageSize, CancellationToken cancellationToken);
    Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
}