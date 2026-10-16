using Microsoft.AspNetCore.Mvc.Testing;

namespace ArchitectureDemo.Tests;

public sealed class VerticalSlicesApiTests : IClassFixture<WebApplicationFactory<DemoApp.VerticalSlices.Program>>
{
    private readonly WebApplicationFactory<DemoApp.VerticalSlices.Program> _factory;

    public VerticalSlicesApiTests(WebApplicationFactory<DemoApp.VerticalSlices.Program> factory) => _factory = factory;

    [Fact]
    public async Task SatisfiesProductApiContract()
    {
        using var client = _factory.CreateClient();

        await ProductApiContract.AssertAsync(client);
    }
}