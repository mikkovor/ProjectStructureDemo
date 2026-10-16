using System.Net;
using System.Net.Http.Json;

namespace ArchitectureDemo.Tests;

public sealed record ProductResponse(Guid Id, string Name, decimal Price);

public static class ProductApiContract
{
    public static async Task AssertAsync(HttpClient client)
    {
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/products")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/products?search=demo&pageSize=1")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/products?pageSize=101")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/products/ffffffff-ffff-ffff-ffff-ffffffffffff")).StatusCode);

        var created = await client.PostAsJsonAsync("/products", new { name = "New product", price = 12.50m });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var invalid = await client.PostAsJsonAsync("/products", new { name = "", price = 0m });
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }
}