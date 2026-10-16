# Architecture Demo: One Product API, Four Structures

This solution implements the same small Product API four times on .NET 10 so you can compare how different code organizations feel in practice. The examples illustrate tradeoffs; none of them is a universal ranking, and the right choice depends on team size, domain complexity, and how the code will change.

Each project has its **own** in-memory implementation. There is no database and no implementation code is shared between projects.

## Projects

| Style | Project | Main code |
| --- | --- | --- |
| Layered | `DemoApp.Layered` | `Controllers`, `Services`, `Repositories`, `Models`, `DTOs`, `Validators` |
| Vertical slices | `DemoApp.VerticalSlices` | `Features\Products\<Operation>`, shared `Products` |
| Single-file vertical slices | `DemoApp.VerticalSlices.SingleFile` | `Features\Products\<Operation>.cs`, shared `Products` |
| Clean Architecture | `DemoApp.CleanArchitecture.Api` (+ `Application`, `Domain`, `Infrastructure`) | one project per layer |

Cross-project contract tests live in `ArchitectureDemo.Tests`.

### Layered

Controller -> service -> repository, grouped by technical role.

```text
DemoApp.Layered
  Controllers\ProductsController.cs
  Services\IProductService.cs, ProductService.cs
  Repositories\IProductRepository.cs, InMemoryProductRepository.cs
  Models\Product.cs
  DTOs\CreateProductRequest.cs, ProductResponse.cs
  Validators\GetProductsQuery.cs, GetProductsQueryValidator.cs, CreateProductRequestValidator.cs
  Program.cs
```

- Familiar and easy to navigate by technical concern; a good fit for small CRUD-style services.
- A single feature is spread across many folders, and services can grow into large catch-all classes as the app expands.

### Vertical slices

Each operation owns a folder; endpoint, request/response contracts and validator are separate focused files.

```text
DemoApp.VerticalSlices
  Features\Products\
    CreateProduct\   CreateProduct.cs, CreateProductRequest.cs, CreateProductResponse.cs, CreateProductValidator.cs
    GetProductById\  GetProductById.cs, ProductResponse.cs
    GetProducts\     GetProducts.cs, GetProductsRequest.cs, GetProductsResponse.cs, GetProductsValidator.cs
  Products\          Product.cs, IProductRepository.cs, InMemoryProductRepository.cs
  Program.cs
```

- Changes to one operation stay in one folder, and slices can evolve independently.
- Small files mean more files to open; shared concepts (here `Products`) still need a deliberate home, and slices can drift apart in style without conventions.

### Single-file vertical slices

Same slice idea, but the endpoint and its request, response and validator types are colocated in one file per operation.

```text
DemoApp.VerticalSlices.SingleFile
  Features\Products\CreateProduct.cs, GetProductById.cs, GetProducts.cs
  Products\Product.cs, IProductRepository.cs, InMemoryProductRepository.cs
  Program.cs
```

- Everything about an operation is readable top to bottom in one place.
- Files can grow long as an operation becomes complex, and several types sharing a file may surprise people used to one type per file.

### Clean Architecture

Separate projects enforce dependency direction: `Api` references `Application` and `Infrastructure`; `Infrastructure` references `Application` and `Domain`; `Application` references `Domain`.

```text
DemoApp.CleanArchitecture.Api             Endpoints\, Requests\ (with validators), Responses\, Program.cs
DemoApp.CleanArchitecture.Application     Abstractions\IProductRepository.cs, Products\<UseCase>\ (query/command + handler)
DemoApp.CleanArchitecture.Domain          Products\Product.cs
DemoApp.CleanArchitecture.Infrastructure  Products\InMemoryProductRepository.cs
```

- The compiler enforces inward dependencies, keeping domain and use-case logic independent of web and storage details.
- More projects, mapping and indirection than a simple CRUD API needs; the benefit grows with domain complexity and longevity.

## Common Product behavior

All four APIs expose the same operations:

| Operation | Success | Failure |
| --- | --- | --- |
| `GET /products?search=&pageSize=` | `200` list of products | `400` for invalid query |
| `GET /products/{id}` | `200` product | `404` if unknown |
| `POST /products` with `{ "name": "...", "price": 0.00 }` | `201` with product body and `Location` header of the new product | `400` for invalid or malformed body; nothing is created |

A product is `{ id, name, price }`. Search is a case-insensitive substring match on name; results keep insertion order.

Validation rules (FluentValidation, registered and invoked the same way in every project):

- `search`: at most 100 characters.
- `pageSize`: when supplied, between 1 and 100 inclusive.
- `name`: required, at most 100 characters.
- `price`: greater than 0.

Each API starts with the same deterministic data, held in memory and independent per project (restarting resets it):

- `11111111-1111-1111-1111-111111111111` - Demo product - 9.99
- `22222222-2222-2222-2222-222222222222` - Office chair - 149.00

## Run the APIs

From the repository root in PowerShell:

```powershell
dotnet run --project DemoApp.Layered
dotnet run --project DemoApp.VerticalSlices
dotnet run --project DemoApp.VerticalSlices.SingleFile
dotnet run --project DemoApp.CleanArchitecture.Api
```

Each API uses its own in-memory data, and its console prints the listening URL. In the Development environment, append `/openapi/v1.json` to that URL to inspect the generated OpenAPI document.

## Build and test

```powershell
dotnet build DemoApp.slnx
dotnet test DemoApp.slnx
```

`ArchitectureDemo.Tests` runs one shared contract (`ProductApiContract`) against all four APIs: exact bodies, filtering, page-size bounds, malformed or missing fields, status codes, the `Location` header, and that failed POSTs create nothing.
