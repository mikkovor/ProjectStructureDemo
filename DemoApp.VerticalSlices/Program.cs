using DemoApp.VerticalSlices.Features.Products.CreateProduct;
using DemoApp.VerticalSlices.Features.Products.GetProductById;
using DemoApp.VerticalSlices.Features.Products.GetProducts;
using DemoApp.VerticalSlices.Products;
using FluentValidation;

namespace DemoApp.VerticalSlices
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOpenApi();
            builder.Services.AddSingleton<IValidator<GetProductsRequest>, GetProductsValidator>();
            builder.Services.AddSingleton<IValidator<CreateProductRequest>, CreateProductValidator>();
            builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.MapGetProducts();
            app.MapGetProductById();
            app.MapCreateProduct();

            app.Run();
        }
    }
}