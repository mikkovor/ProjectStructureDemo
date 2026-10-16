using DemoApp.CleanArchitecture.Api.Endpoints;
using DemoApp.CleanArchitecture.Api.Requests;
using DemoApp.CleanArchitecture.Application.Abstractions;
using DemoApp.CleanArchitecture.Application.Products.CreateProduct;
using DemoApp.CleanArchitecture.Application.Products.GetProductById;
using DemoApp.CleanArchitecture.Application.Products.GetProducts;
using DemoApp.CleanArchitecture.Infrastructure.Products;
using FluentValidation;

namespace DemoApp.CleanArchitecture.Api
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOpenApi();
            builder.Services.AddSingleton<IValidator<GetProductsRequest>, GetProductsRequestValidator>();
            builder.Services.AddSingleton<IValidator<CreateProductRequest>, CreateProductRequestValidator>();
            builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
            builder.Services.AddScoped<GetProductsHandler>();
            builder.Services.AddScoped<GetProductByIdHandler>();
            builder.Services.AddScoped<CreateProductHandler>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.MapProducts();

            app.Run();
        }
    }
}
