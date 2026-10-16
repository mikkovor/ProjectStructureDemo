using Microsoft.AspNetCore.Mvc.Testing;

namespace ArchitectureDemo.Tests;

public sealed class SingleFileVerticalSlicesApiTests : IClassFixture<WebApplicationFactory<DemoApp.VerticalSlices.SingleFile.Program>>
{
    private readonly WebApplicationFactory<DemoApp.VerticalSlices.SingleFile.Program> _factory;

    public SingleFileVerticalSlicesApiTests(WebApplicationFactory<DemoApp.VerticalSlices.SingleFile.Program> factory) => _factory = factory;

    [Fact]
    public async Task SatisfiesProductApiContract()
    {
        using var client = _factory.CreateClient();

        await ProductApiContract.AssertAsync(client);
    }
}