namespace DemoApp.CleanArchitecture.Application.Products.GetProducts;

public sealed record GetProductsQuery(string? Search, int? PageSize);
