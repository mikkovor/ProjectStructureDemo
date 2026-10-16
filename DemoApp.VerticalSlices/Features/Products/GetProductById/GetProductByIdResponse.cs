namespace DemoApp.VerticalSlices.Features.Products.GetProductById;

public sealed record GetProductByIdResponse(Guid Id, string Name, decimal Price);