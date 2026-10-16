# Architecture Demo: One Product API, Four Structures

This solution implements the same small Product API four times on .NET 10 so you can compare how different code organizations feel in practice. The examples illustrate tradeoffs; none of them is a universal ranking, and the right choice depends on team size, domain complexity, and how the code will change.

Each project has its **own** in-memory implementation. There is no database and no implementation code is shared between projects.

## Projects

| Style | Project | Main code |
| --- | --- | --- |
| Layered | `DemoApp.Layered` | `Controllers`, `Services`, `Repositories`, `Models`, `DTOs`, `Validators` |
| Vertical slices | `DemoApp.VerticalSlices` | `Features\Products\<Operation>`, shared `Domain` and `Data` |
| Single-file vertical slices | `DemoApp.VerticalSlices.SingleFile` | `Features\Products\<Operation>.cs`, shared `Domain` and `Data` |
| Clean Architecture | `DemoApp.CleanArchitecture.Api` (+ `Application`, `Domain`, `Infrastructure`) | one project per layer |

Cross-project contract tests live in `ArchitectureDemo.Tests`.

### Layered

Controller -> service -> repository, grouped by technical role.

```text
DemoApp.Layered
  Controllers\ProductsController.cs
  Services\IProductService.cs, ProductService.cs, ChangePriceResult.cs
  Repositories\IProductRepository.cs, InMemoryProductRepository.cs
  Models\Product.cs
  DTOs\CreateProductRequest.cs, ChangePriceRequest.cs, GetProductsQuery.cs, ProductResponse.cs
  Validators\CreateProductRequestValidator.cs, ChangePriceRequestValidator.cs, GetProductsQueryValidator.cs
  Program.cs
```

- Familiar and easy to navigate by technical concern; a good fit for small CRUD-style services.
- A single feature is spread across many folders, and services can grow into large catch-all classes as the app expands.

### Vertical slices

Each operation owns a folder; endpoint, request/response contracts and validator are separate focused files.

```text
DemoApp.VerticalSlices
  Features\Products\
    ChangePrice\     ChangePrice.cs, ChangePriceRequest.cs, ChangePriceResponse.cs, ChangePriceValidator.cs
    CreateProduct\   CreateProduct.cs, CreateProductRequest.cs, CreateProductResponse.cs, CreateProductValidator.cs
    GetProductById\  GetProductById.cs, GetProductByIdResponse.cs
    GetProducts\     GetProducts.cs, GetProductsRequest.cs, GetProductsResponse.cs, GetProductsValidator.cs
    ProductEndpoints.cs   route group that wires the slices together
  Common\            ValidationFilter.cs
  Domain\            Product.cs (invariants + price-change rule)
  Data\              ProductStore.cs
  Program.cs
```

- Changes to one operation stay in one folder, and slices can evolve independently.
- Slices use the concrete `ProductStore` directly; there is no repository interface or service layer between the endpoint and the data. `Domain` and `Data` hold what slices genuinely share.
- Vertical slices are about how features are organized, not about how rich the domain model is: `Product` is a DDD-style entity that guards its own invariants, and slices orchestrate it.
- Small files mean more files to open, and slices can drift apart in style without conventions.
- As the app grows, `Domain` and `Data` can move into their own projects when another host (for example a background worker) needs them. Projects are then split by what is shared, not by layer; each host keeps its own slices.

### Single-file vertical slices

Same slice idea, but the endpoint and its request, response and validator types are colocated in one file per operation.

```text
DemoApp.VerticalSlices.SingleFile
  Features\Products\ChangePrice.cs, CreateProduct.cs, GetProductById.cs, GetProducts.cs, ProductEndpoints.cs
  Common\ValidationFilter.cs
  Domain\Product.cs
  Data\ProductStore.cs
  Program.cs
```

- Everything about an operation is readable top to bottom in one place.
- Files can grow long as an operation becomes complex, and several types sharing a file may surprise people used to one type per file.

### Clean Architecture

Separate projects enforce dependency direction: `Api` references `Application` and `Infrastructure`; `Infrastructure` references `Application` and `Domain`; `Application` references `Domain`.

```text
DemoApp.CleanArchitecture.Api             Endpoints\, Filters\ValidationFilter.cs, Requests\ (with validators), Responses\, Program.cs
DemoApp.CleanArchitecture.Application     Abstractions\IProductRepository.cs, Products\<UseCase>\ (query/command + handler)
DemoApp.CleanArchitecture.Domain          Products\Product.cs (invariants + price-change rule)
DemoApp.CleanArchitecture.Infrastructure  Products\InMemoryProductRepository.cs
```

- The compiler enforces inward dependencies, keeping domain and use-case logic independent of web and storage details.
- `Product` protects its own invariants (valid name, positive price) and owns the price-change rule; the API validators only reject malformed input early.
- More projects, mapping and indirection than a simple CRUD API needs; the benefit grows with domain complexity and longevity.

## Where does the business rule live?

`PUT /products/{id}/price` may not raise a price by more than 50% at once.

| Style | Rule | Orchestration |
| --- | --- | --- |
| Layered | `ProductService.ChangePriceAsync` (anemic `Product`, logic in the service layer) | `ProductsController` |
| Vertical slices | `Domain\Product.TryChangePrice` | `Features\Products\ChangePrice\ChangePrice.cs` |
| Single-file vertical slices | `Domain\Product.TryChangePrice` | `Features\Products\ChangePrice.cs` |
| Clean Architecture | `Product.TryChangePrice` in the Domain project | `ChangePriceHandler` in Application |

Vertical slices and Clean Architecture share the same rich domain model. The difference is everything around it: slices keep each use case in one place and talk to the store directly, while Clean Architecture splits the same flow across projects with an abstraction (`IProductRepository`) between use case and storage.

## Route groups and validation

The three Minimal API projects map all product endpoints through one route group:

```csharp
var group = app.MapGroup("/products")
    .WithTags("Products")
    .AddEndpointFilter<ValidationFilter>()
    .ProducesValidationProblem();
```

Endpoints use relative routes (`"/"`, `"/{id:guid}"`, `"/{id:guid}/price"`). `ValidationFilter` runs every registered FluentValidation validator for the endpoint's arguments and returns `400` before the handler runs, so handlers contain no validation code. Layered gets the same prefix from `[Route("products")]` on the controller and validates explicitly in each action.

## Common Product behavior

All four APIs expose the same operations:

| Operation | Success | Failure |
| --- | --- | --- |
| `GET /products?search=&pageSize=` | `200` list of products | `400` for invalid query |
| `GET /products/{id}` | `200` product | `404` if unknown |
| `POST /products` with `{ "name": "...", "price": 0.00 }` | `201` with product body and `Location` header of the new product | `400` for invalid or malformed body; nothing is created |
| `PUT /products/{id}/price` with `{ "price": 0.00 }` | `200` updated product | `400` invalid or malformed body, `404` if unknown, `422` if the price would rise more than 50%; nothing is changed |

A product is `{ id, name, price }`. Search is a case-insensitive substring match on name; results keep insertion order.

Validation rules (FluentValidation; invoked by the route-group filter in the Minimal API projects and explicitly in the Layered controller):

- `search`: at most 100 characters.
- `pageSize`: when supplied, between 1 and 100 inclusive.
- `name`: required, at most 100 characters.
- `price`: greater than 0 (both when creating and when changing the price).

Business rule (not input validation, so it returns `422` instead of `400`):

- A price change may raise the price by at most 50% (exactly +50% is allowed). Lowering is always allowed.

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

Each API uses its own in-memory data, and its console prints the listening URL. In the Development environment, append `/openapi/v1.json` to that URL to inspect the generated OpenAPI document. Every project also has a `.http` file with ready-made requests for all operations, including the `422` business-rule case.

## Build and test

```powershell
dotnet build DemoApp.slnx
dotnet test DemoApp.slnx
```

`ArchitectureDemo.Tests` runs one shared contract (`ProductApiContract`) against all four APIs: exact bodies, filtering, page-size bounds, malformed or missing fields, status codes, the `Location` header, that failed POSTs create nothing, and the price-change rule including its +50% boundary.
