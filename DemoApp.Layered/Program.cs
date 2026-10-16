using DemoApp.Layered.DTOs;
using DemoApp.Layered.Repositories;
using DemoApp.Layered.Services;
using DemoApp.Layered.Validators;
using FluentValidation;

namespace DemoApp.Layered
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddSingleton<IValidator<GetProductsQuery>, GetProductsQueryValidator>();
            builder.Services.AddSingleton<IValidator<CreateProductRequest>, CreateProductRequestValidator>();
            builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}