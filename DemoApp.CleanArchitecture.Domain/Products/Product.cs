namespace DemoApp.CleanArchitecture.Domain.Products;

public sealed class Product
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}
