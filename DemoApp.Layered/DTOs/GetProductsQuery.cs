namespace DemoApp.Layered.DTOs;

public sealed record GetProductsQuery(string? Search, int? PageSize);