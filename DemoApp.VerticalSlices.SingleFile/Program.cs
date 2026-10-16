using DemoApp.VerticalSlices.SingleFile.Features.Products;
using DemoApp.VerticalSlices.SingleFile.Products;
using FluentValidation;

namespace DemoApp.VerticalSlices.SingleFile
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOpenApi();
            builder.Services.AddSingleton<IValidator<GetProducts.Query>, GetProducts.Validator>();
            builder.Services.AddSingleton<IValidator<CreateProduct.Request>, CreateProduct.Validator>();
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
