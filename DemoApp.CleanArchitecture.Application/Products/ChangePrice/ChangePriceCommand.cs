namespace DemoApp.CleanArchitecture.Application.Products.ChangePrice;

public sealed record ChangePriceCommand(Guid Id, decimal Price);
