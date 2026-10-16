namespace DemoApp.VerticalSlices.Features.Products.GetProducts;

public sealed record GetProductsRequest(string? Search, int? PageSize);