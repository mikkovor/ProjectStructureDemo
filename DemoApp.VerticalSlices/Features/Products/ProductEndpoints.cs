using DemoApp.VerticalSlices.Common;
using DemoApp.VerticalSlices.Features.Products.ChangePrice;
using DemoApp.VerticalSlices.Features.Products.CreateProduct;
using DemoApp.VerticalSlices.Features.Products.GetProductById;
using DemoApp.VerticalSlices.Features.Products.GetProducts;

namespace DemoApp.VerticalSlices.Features.Products;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products")
            .WithTags("Products")
            .AddEndpointFilter<ValidationFilter>()
            .ProducesValidationProblem();

        group.MapGetProducts();
        group.MapGetProductById();
        group.MapCreateProduct();
        group.MapChangePrice();
    }
}
