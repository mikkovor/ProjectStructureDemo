using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace ArchitectureDemo.Tests;

public sealed record ProductResponse(Guid Id, string Name, decimal Price);

public static class ProductApiContract
{
    private static readonly Guid DemoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ChairId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static async Task AssertAsync(HttpClient client)
    {
        var unfiltered = await client.GetFromJsonAsync<List<ProductResponse>>("/products");
        Assert.Equal(2, unfiltered!.Count);
        Assert.Equal(new ProductResponse(DemoId, "Demo product", 9.99m), unfiltered[0]);
        Assert.Equal(new ProductResponse(ChairId, "Office chair", 149.00m), unfiltered[1]);

        var filtered = await client.GetFromJsonAsync<List<ProductResponse>>("/products?search=DEMO&pageSize=1");
        Assert.Equal("Demo product", Assert.Single(filtered!).Name);

        var noMatches = await client.GetFromJsonAsync<List<ProductResponse>>("/products?search=not-found");
        Assert.Empty(noMatches!);

        var oneItem = await client.GetFromJsonAsync<List<ProductResponse>>("/products?pageSize=1");
        Assert.Single(oneItem!);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/products?pageSize=100")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/products?pageSize=0")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/products?pageSize=101")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync($"/products?search={new string('x', 101)}")).StatusCode);

        var seeded = await client.GetAsync($"/products/{DemoId}");
        Assert.Equal(HttpStatusCode.OK, seeded.StatusCode);
        Assert.Equal(new ProductResponse(DemoId, "Demo product", 9.99m), await seeded.Content.ReadFromJsonAsync<ProductResponse>());
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/products/ffffffff-ffff-ffff-ffff-ffffffffffff")).StatusCode);

        await AssertInvalidPostsCreateNothingAsync(client);

        var created = await client.PostAsJsonAsync("/products", new { name = "New product", price = 12.50m });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        Assert.NotNull(created.Headers.Location);
        var createdProduct = await created.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotEqual(Guid.Empty, createdProduct!.Id);
        Assert.Equal("New product", createdProduct.Name);
        Assert.Equal(12.50m, createdProduct.Price);

        var located = await client.GetAsync(created.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, located.StatusCode);
        Assert.Equal(createdProduct, await located.Content.ReadFromJsonAsync<ProductResponse>());
        Assert.EndsWith($"/products/{createdProduct.Id}", created.Headers.Location!.AbsolutePath, StringComparison.OrdinalIgnoreCase);

        var afterCreate = await client.GetFromJsonAsync<List<ProductResponse>>("/products");
        Assert.Equal(3, afterCreate!.Count);
        Assert.Contains(createdProduct, afterCreate);
    }

    private static async Task AssertInvalidPostsCreateNothingAsync(HttpClient client)
    {
        var before = await client.GetFromJsonAsync<List<ProductResponse>>("/products");

        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/products", new { name = "", price = 0m })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/products", new { name = new string('x', 101), price = 12.50m })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/products", new { name = "Invalid price", price = 0m })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/products", new { price = 12.50m })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/products", new StringContent("{", Encoding.UTF8, "application/json"))).StatusCode);

        var after = await client.GetFromJsonAsync<List<ProductResponse>>("/products");
        Assert.Equal(before, after);
    }
}

