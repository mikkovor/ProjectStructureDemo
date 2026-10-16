using Microsoft.AspNetCore.Mvc.Testing;

namespace ArchitectureDemo.Tests;

public sealed class CleanArchitectureApiTests : IClassFixture<WebApplicationFactory<DemoApp.CleanArchitecture.Api.Program>>
{
    private readonly WebApplicationFactory<DemoApp.CleanArchitecture.Api.Program> _factory;

    public CleanArchitectureApiTests(WebApplicationFactory<DemoApp.CleanArchitecture.Api.Program> factory) => _factory = factory;

    [Fact]
    public async Task SatisfiesProductApiContract()
    {
        using var client = _factory.CreateClient();

        await ProductApiContract.AssertAsync(client);
    }
}
