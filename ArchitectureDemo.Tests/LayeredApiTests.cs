using Microsoft.AspNetCore.Mvc.Testing;

namespace ArchitectureDemo.Tests;

public sealed class LayeredApiTests : IClassFixture<WebApplicationFactory<DemoApp.Layered.Program>>
{
    private readonly WebApplicationFactory<DemoApp.Layered.Program> _factory;

    public LayeredApiTests(WebApplicationFactory<DemoApp.Layered.Program> factory) => _factory = factory;

    [Fact]
    public async Task SatisfiesProductApiContract()
    {
        using var client = _factory.CreateClient();

        await ProductApiContract.AssertAsync(client);
    }
}