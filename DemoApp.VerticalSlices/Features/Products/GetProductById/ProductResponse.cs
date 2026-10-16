namespace DemoApp.VerticalSlices.Features.Products.GetProductById;

public sealed record ProductResponse(Guid Id, string Name, decimal Price);