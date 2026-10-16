using DemoApp.VerticalSlices.Data;
using DemoApp.VerticalSlices.Features.Products;
using DemoApp.VerticalSlices.Features.Products.ChangePrice;
using DemoApp.VerticalSlices.Features.Products.CreateProduct;
using DemoApp.VerticalSlices.Features.Products.GetProducts;
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
            builder.Services.AddSingleton<IValidator<ChangePriceRequest>, ChangePriceValidator>();
            builder.Services.AddSingleton<ProductStore>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.MapProductEndpoints();

            app.Run();
        }
    }
}