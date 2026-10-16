namespace DemoApp.Layered.Validators;

public sealed record GetProductsQuery(string? Search, int? PageSize);