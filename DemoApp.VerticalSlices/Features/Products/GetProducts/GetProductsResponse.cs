namespace DemoApp.VerticalSlices.Features.Products.GetProducts;

public sealed record GetProductsResponse(Guid Id, string Name, decimal Price);