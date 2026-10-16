namespace DemoApp.CleanArchitecture.Api.Requests;

public sealed record GetProductsRequest(string? Search, int? PageSize);
