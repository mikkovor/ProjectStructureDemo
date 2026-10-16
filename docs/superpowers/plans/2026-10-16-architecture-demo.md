# Architecture Demo Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a .NET 10 solution that demonstrates four project-organization styles through equivalent in-memory Product APIs.

**Architecture:** Create four separately runnable APIs: a technically layered controller/service/repository project, a multi-file vertical-slice project, a one-file-per-operation vertical-slice project, and a four-project Clean Architecture example. Verify the same Product contract across all four with one integration-test project, and explain the differences and run commands in the solution README.

**Tech Stack:** .NET 10, ASP.NET Core Minimal APIs and MVC controllers, FluentValidation, xUnit, `Microsoft.AspNetCore.Mvc.Testing`, XML solution format (`.slnx`).

**Spec:** `docs\superpowers\specs\2026-10-16-architecture-demo-design.md`

## Global Constraints

- Use the repository's current .NET 10 target framework across projects.
- Each project uses its own in-memory implementation and starts with a small, deterministic sample dataset.
- No database or cross-project shared implementation is introduced.
- FluentValidation will be used for request validation in the examples, with registration and invocation following a consistent approach so validation behavior does not vary by architecture.
- The solution README will explain what each project demonstrates, where its main code lives, how to run each API and access its OpenAPI description, the common Product behavior and validation rules, and that the examples illustrate tradeoffs and are not a universal ranking.

## Review Focus

- GET `pageSize` is omitted, at its boundaries (1 and 100), or outside 1–100; test defaulting, acceptance, and validation response.
- GET `search` is omitted, empty, or longer than its maximum; test case-insensitive filtering and validation response.
- GET requests use an unknown product id; test that all four APIs return 404.
- POST requests have an empty/overlong name or non-positive price; test consistent validation errors and no successful creation.
- POST body is malformed JSON or has missing fields; test the API returns a client error rather than an unhandled server error.

---

## File Structure

- `DemoApp.slnx` — includes the four runnable API examples, the three supporting Clean Architecture projects, and the integration-test project.
- `DemoApp.Layered\` — the existing starter project adapted into the layered example.
  - `Controllers\ProductsController.cs` — HTTP routing and HTTP response mapping.
  - `Services\IProductService.cs`, `Services\ProductService.cs` — product use cases.
  - `Repositories\IProductRepository.cs`, `Repositories\InMemoryProductRepository.cs` — storage contract and in-memory implementation.
  - `Models\Product.cs` — internal product model.
  - `DTOs\ProductResponse.cs`, `DTOs\CreateProductRequest.cs` — HTTP contracts.
  - `Validators\CreateProductRequestValidator.cs`, `Validators\GetProductsQueryValidator.cs` — request rules.
  - `Program.cs` — dependency injection, controllers, OpenAPI, and middleware.
- `DemoApp.VerticalSlices\` — one API project; `Features\Products\GetProducts\` and `Features\Products\CreateProduct\` each contain focused endpoint, contract, and validator files; `Features\Products\GetProductById\` contains the read-by-id endpoint and response contract. A `Products\` folder holds only that example's in-memory product storage.
- `DemoApp.VerticalSlices.SingleFile\` — one API project; `Features\Products\GetProducts.cs`, `GetProductById.cs`, and `CreateProduct.cs` each colocate the endpoint, relevant request/response types, and validator. `Products\InMemoryProductRepository.cs` holds in-memory storage.
- `DemoApp.CleanArchitecture.Api\` — HTTP endpoints, HTTP contracts/validators, dependency composition, and OpenAPI.
- `DemoApp.CleanArchitecture.Application\` — use cases and `IProductRepository` abstraction.
- `DemoApp.CleanArchitecture.Domain\` — `Product` entity and domain-level rules; no dependency on outer layers.
- `DemoApp.CleanArchitecture.Infrastructure\` — in-memory repository implementation.
- `ArchitectureDemo.Tests\` — xUnit integration tests, a shared Product API contract assertion helper, and one test fixture per API using `WebApplicationFactory<TEntryPoint>`.
- `README.md` — architecture comparison, project map, shared contract, and run/OpenAPI instructions.

## Task 1: Create the Solution Project Layout

**Files:**
- Modify: `DemoApp.slnx`
- Rename/adapt: `DemoApp.API\DemoApp.API.csproj` to `DemoApp.Layered\DemoApp.Layered.csproj`
- Create: `DemoApp.VerticalSlices\DemoApp.VerticalSlices.csproj`
- Create: `DemoApp.VerticalSlices.SingleFile\DemoApp.VerticalSlices.SingleFile.csproj`
- Create: `DemoApp.CleanArchitecture.Api\DemoApp.CleanArchitecture.Api.csproj`
- Create: `DemoApp.CleanArchitecture.Application\DemoApp.CleanArchitecture.Application.csproj`
- Create: `DemoApp.CleanArchitecture.Domain\DemoApp.CleanArchitecture.Domain.csproj`
- Create: `DemoApp.CleanArchitecture.Infrastructure\DemoApp.CleanArchitecture.Infrastructure.csproj`
- Create: `ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj`

**Interfaces:**
- Produces: seven application projects (four runnable APIs and three Clean Architecture class libraries) plus one test project; no Product behavior is implemented in this task.
- Clean Architecture project references: Application → Domain; Infrastructure → Application and Domain; API → Application and Infrastructure; Domain has no project references.
- Every project targets `net10.0` and enables nullable reference types and implicit usings.

- [ ] **Step 1: Rename the starter API project to the layered example**

Use PowerShell to rename the starter project directory and project file, preserving its existing `Microsoft.AspNetCore.OpenApi` reference:

```powershell
Move-Item DemoApp.API DemoApp.Layered
Move-Item DemoApp.Layered\DemoApp.API.csproj DemoApp.Layered\DemoApp.Layered.csproj
```

- [ ] **Step 2: Create the remaining API and Clean Architecture project skeletons**

```powershell
dotnet new webapi -f net10.0 -n DemoApp.VerticalSlices -o DemoApp.VerticalSlices
dotnet new webapi -f net10.0 -n DemoApp.VerticalSlices.SingleFile -o DemoApp.VerticalSlices.SingleFile
dotnet new webapi -f net10.0 -n DemoApp.CleanArchitecture.Api -o DemoApp.CleanArchitecture.Api
dotnet new classlib -f net10.0 -n DemoApp.CleanArchitecture.Application -o DemoApp.CleanArchitecture.Application
dotnet new classlib -f net10.0 -n DemoApp.CleanArchitecture.Domain -o DemoApp.CleanArchitecture.Domain
dotnet new classlib -f net10.0 -n DemoApp.CleanArchitecture.Infrastructure -o DemoApp.CleanArchitecture.Infrastructure
dotnet new xunit -f net10.0 -n ArchitectureDemo.Tests -o ArchitectureDemo.Tests
```

- [ ] **Step 3: Add projects and Clean Architecture references**

```powershell
dotnet sln DemoApp.slnx add DemoApp.Layered\DemoApp.Layered.csproj DemoApp.VerticalSlices\DemoApp.VerticalSlices.csproj DemoApp.VerticalSlices.SingleFile\DemoApp.VerticalSlices.SingleFile.csproj DemoApp.CleanArchitecture.Api\DemoApp.CleanArchitecture.Api.csproj DemoApp.CleanArchitecture.Application\DemoApp.CleanArchitecture.Application.csproj DemoApp.CleanArchitecture.Domain\DemoApp.CleanArchitecture.Domain.csproj DemoApp.CleanArchitecture.Infrastructure\DemoApp.CleanArchitecture.Infrastructure.csproj ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj
dotnet add DemoApp.CleanArchitecture.Application\DemoApp.CleanArchitecture.Application.csproj reference DemoApp.CleanArchitecture.Domain\DemoApp.CleanArchitecture.Domain.csproj
dotnet add DemoApp.CleanArchitecture.Infrastructure\DemoApp.CleanArchitecture.Infrastructure.csproj reference DemoApp.CleanArchitecture.Application\DemoApp.CleanArchitecture.Application.csproj DemoApp.CleanArchitecture.Domain\DemoApp.CleanArchitecture.Domain.csproj
dotnet add DemoApp.CleanArchitecture.Api\DemoApp.CleanArchitecture.Api.csproj reference DemoApp.CleanArchitecture.Application\DemoApp.CleanArchitecture.Application.csproj DemoApp.CleanArchitecture.Infrastructure\DemoApp.CleanArchitecture.Infrastructure.csproj
```

- [ ] **Step 4: Add OpenAPI and FluentValidation packages to the API projects**

Keep the existing OpenAPI package in the renamed layered API. For each new Web API project, retain the template's OpenAPI package. Add FluentValidation to each API project:

```powershell
dotnet add DemoApp.Layered\DemoApp.Layered.csproj package FluentValidation
dotnet add DemoApp.VerticalSlices\DemoApp.VerticalSlices.csproj package FluentValidation
dotnet add DemoApp.VerticalSlices.SingleFile\DemoApp.VerticalSlices.SingleFile.csproj package FluentValidation
dotnet add DemoApp.CleanArchitecture.Api\DemoApp.CleanArchitecture.Api.csproj package FluentValidation
```

- [ ] **Step 5: Restore and build the project skeletons**

Run: `dotnet restore DemoApp.slnx`
Expected: all solution projects restore successfully.

Run: `dotnet build DemoApp.slnx --no-restore`
Expected: all projects build; resolve template/namespace issues before moving on.

## Task 2: Implement and Verify the Layered API

**Files:**
- Modify: `DemoApp.Layered\Program.cs`, `DemoApp.Layered\DemoApp.Layered.csproj`
- Create: `DemoApp.Layered\Controllers\ProductsController.cs`
- Create: `DemoApp.Layered\Services\IProductService.cs`, `DemoApp.Layered\Services\ProductService.cs`
- Create: `DemoApp.Layered\Repositories\IProductRepository.cs`, `DemoApp.Layered\Repositories\InMemoryProductRepository.cs`
- Create: `DemoApp.Layered\Models\Product.cs`
- Create: `DemoApp.Layered\DTOs\ProductResponse.cs`, `DemoApp.Layered\DTOs\CreateProductRequest.cs`
- Create: `DemoApp.Layered\Validators\CreateProductRequestValidator.cs`, `DemoApp.Layered\Validators\GetProductsQuery.cs`, `DemoApp.Layered\Validators\GetProductsQueryValidator.cs`
- Create: `ArchitectureDemo.Tests\ProductApiContract.cs`, `ArchitectureDemo.Tests\LayeredApiTests.cs`
- Modify: `ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj`

**Interfaces:**
- Produces: `GET /products?search=&pageSize=`, `GET /products/{id:guid}`, and `POST /products`.
- `IProductRepository`: `Task<IReadOnlyList<Product>> GetAllAsync(string? search, int? pageSize, CancellationToken)`, `Task<Product?> GetByIdAsync(Guid, CancellationToken)`, and `Task<Product> AddAsync(Product, CancellationToken)`.
- `IProductService`: asynchronous equivalents returning response DTOs and accepting validated request data.
- Validators are registered as `IValidator<T>` and explicitly called with `ValidateAsync`; validation failures map to `Results.ValidationProblem`/an equivalent MVC 400 validation response.

- [ ] **Step 1: Add contract tests for the expected HTTP behavior**

Create `ProductApiContract.AssertAsync(HttpClient)` using `System.Net.Http.Json`. Assert that:

```csharp
var list = await client.GetAsync("/products");
Assert.Equal(HttpStatusCode.OK, list.StatusCode);

var search = await client.GetAsync("/products?search=demo&pageSize=1");
Assert.Equal(HttpStatusCode.OK, search.StatusCode);

var badPageSize = await client.GetAsync("/products?pageSize=101");
Assert.Equal(HttpStatusCode.BadRequest, badPageSize.StatusCode);

var missing = await client.GetAsync("/products/ffffffff-ffff-ffff-ffff-ffffffffffff");
Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);

var created = await client.PostAsJsonAsync("/products", new { name = "New product", price = 12.50m });
Assert.Equal(HttpStatusCode.Created, created.StatusCode);

var invalid = await client.PostAsJsonAsync("/products", new { name = "", price = 0m });
Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
```

Add an `IClassFixture<WebApplicationFactory<DemoApp.Layered.Program>>` test class that calls this contract helper. Make the entry point a public partial `DemoApp.Layered.Program` type so the test host can discover it. Add `Microsoft.AspNetCore.Mvc.Testing` 10.0.12 to the test project and reference the layered API project.

Define the test-only response record used by the helper:

```csharp
internal sealed record ProductResponse(Guid Id, string Name, decimal Price);
```

- [ ] **Step 2: Run the layered contract test and confirm it fails before implementation**

Run: `dotnet test ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj --filter FullyQualifiedName~LayeredApiTests`
Expected: failure because the starter app does not yet expose the Product contract.

- [ ] **Step 3: Add model, DTOs, and validators**

Create a `Product` model with `Guid Id`, `string Name`, and `decimal Price`; response DTO with the same serialized fields; create request with `Name` and `Price`; and GET query with nullable `Search` and `PageSize`. The create validator requires a non-empty name of at most 100 characters and `Price > 0`. The GET validator allows omitted `pageSize`, otherwise requires 1–100, and limits `search` to 100 characters. A null page size means no limit; an empty search means no filtering. Match these rules in all following examples.

- [ ] **Step 4: Add the in-memory repository and service layer**

Seed the repository deterministically with exactly two products: `Demo product` and `Office chair`, using fixed Guid identifiers. Store products in a private in-memory collection. `GetAllAsync(string? search, int? pageSize, CancellationToken)` applies case-insensitive substring search, then page-size limiting when supplied; `GetByIdAsync` returns null for an unknown id; `AddAsync` assigns a new Guid and stores the product. The service maps models to response DTOs and does not depend on HTTP types.

- [ ] **Step 5: Implement the controller and registrations**

Map the three routes in `ProductsController`. Inject `IProductService` and `IValidator<T>`; call `ValidateAsync` for GET query and POST request, return 400 validation details for invalid requests, 404 for an absent id, 201 plus `Location` for successful creation, and 200 for successful reads. In `Program.cs`, register MVC controllers, OpenAPI, validators, repository, and service. Remove the weather endpoint.

- [ ] **Step 6: Run the layered test and build**

Run: `dotnet test ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj --filter FullyQualifiedName~LayeredApiTests`
Expected: PASS, including list/search, invalid `pageSize`, not-found, valid creation, and invalid creation.

Run: `dotnet build DemoApp.Layered\DemoApp.Layered.csproj`
Expected: PASS with no compilation errors.

## Task 3: Implement the Multi-File Vertical-Slice API

**Files:**
- Modify: `DemoApp.VerticalSlices\Program.cs`, `DemoApp.VerticalSlices\DemoApp.VerticalSlices.csproj`
- Create: `DemoApp.VerticalSlices\Features\Products\GetProducts\GetProducts.cs`, `GetProductsRequest.cs`, `GetProductsResponse.cs`, `GetProductsValidator.cs`
- Create: `DemoApp.VerticalSlices\Features\Products\GetProductById\GetProductById.cs`, `ProductResponse.cs`
- Create: `DemoApp.VerticalSlices\Features\Products\CreateProduct\CreateProduct.cs`, `CreateProductRequest.cs`, `CreateProductResponse.cs`, `CreateProductValidator.cs`
- Create: `DemoApp.VerticalSlices\Products\Product.cs`, `IProductRepository.cs`, `InMemoryProductRepository.cs`
- Create: `ArchitectureDemo.Tests\VerticalSlicesApiTests.cs`
- Modify: `ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj`

**Interfaces:**
- Consumes: Product API contract helper from Task 2.
- Produces: the same three routes, JSON shapes, status codes, seed behavior, query and body validation, and filtering semantics as `DemoApp.Layered`.
- Each feature's endpoint uses only that feature's contracts/validator and the product storage abstraction; do not introduce controllers, a global service layer, or shared implementation code.

- [ ] **Step 1: Add the vertical-slice integration-test fixture**

Add the API project reference and a `WebApplicationFactory<DemoApp.VerticalSlices.Program>` fixture whose test calls `ProductApiContract.AssertAsync`. Make its entry point public and partial. Run the test before implementing endpoints.

Run: `dotnet test ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj --filter FullyQualifiedName~VerticalSlicesApiTests`
Expected: FAIL because Product endpoints are not implemented yet.

- [ ] **Step 2: Add product storage and the GET products slice**

Create the independent in-memory repository and deterministic seed data. Implement GET list filtering, `pageSize`, and its request validator in the `GetProducts` feature files. Map validation errors to the same 400 response as the layered API.

- [ ] **Step 3: Add read-by-id and create-product slices**

Implement GET-by-id with the matching 200/404 behavior. Implement POST with a feature-local request validator and response type; return the same 201 `Location` behavior and use the exact same validation rules as the layered API.

- [ ] **Step 4: Wire feature endpoints and validators**

In `Program.cs`, register the repository and each feature validator, map the three feature endpoints, and retain OpenAPI. Use explicit `ValidateAsync` calls and do not use automatic MVC validation.

- [ ] **Step 5: Run the vertical-slice contract test and build**

Run: `dotnet test ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj --filter FullyQualifiedName~VerticalSlicesApiTests`
Expected: PASS for all shared contract assertions.

Run: `dotnet build DemoApp.VerticalSlices\DemoApp.VerticalSlices.csproj`
Expected: PASS.

## Task 4: Implement the Single-File Vertical-Slice API

**Files:**
- Modify: `DemoApp.VerticalSlices.SingleFile\Program.cs`, `DemoApp.VerticalSlices.SingleFile\DemoApp.VerticalSlices.SingleFile.csproj`
- Create: `DemoApp.VerticalSlices.SingleFile\Features\Products\GetProducts.cs`
- Create: `DemoApp.VerticalSlices.SingleFile\Features\Products\GetProductById.cs`
- Create: `DemoApp.VerticalSlices.SingleFile\Features\Products\CreateProduct.cs`
- Create: `DemoApp.VerticalSlices.SingleFile\Products\Product.cs`, `IProductRepository.cs`, `InMemoryProductRepository.cs`
- Create: `ArchitectureDemo.Tests\SingleFileVerticalSlicesApiTests.cs`
- Modify: `ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj`

**Interfaces:**
- Consumes: Product API contract helper from Task 2.
- Produces: the same behavior and JSON contract as the other APIs.
- `GetProducts.cs` contains its GET endpoint mapping, query DTO, response DTO, and FluentValidation validator together. The other two operation files follow the same one-file-per-operation pattern.

- [ ] **Step 1: Add the single-file integration-test fixture**

Add the API project reference and a `WebApplicationFactory<DemoApp.VerticalSlices.SingleFile.Program>` fixture calling the shared contract helper. Make the entry point public and partial. Run the test to confirm the unimplemented API fails the behavior test.

Run: `dotnet test ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj --filter FullyQualifiedName~SingleFileVerticalSlicesApiTests`
Expected: FAIL until the endpoints are implemented.

- [ ] **Step 2: Implement the repository and `GetProducts.cs`**

Add deterministic in-memory products. In `GetProducts.cs`, define `GetProductsRequest`, `GetProductsResponse`, and `GetProductsValidator` together with a static endpoint mapper. Validate search length and page size (1–100), then return the filtered, limited response.

- [ ] **Step 3: Implement the remaining operation files**

In `GetProductById.cs`, colocate its response contract and endpoint. In `CreateProduct.cs`, colocate its request, response, validator, and endpoint. Preserve shared validation rules and HTTP response behavior exactly.

- [ ] **Step 4: Wire the feature files**

Register the repository and validators in `Program.cs`, map all endpoint methods, retain OpenAPI, and remove template endpoints. Keep operation-specific code in its feature file; do not move validators or DTOs to global folders.

- [ ] **Step 5: Run the single-file contract test and build**

Run: `dotnet test ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj --filter FullyQualifiedName~SingleFileVerticalSlicesApiTests`
Expected: PASS for the shared behavior checks.

Run: `dotnet build DemoApp.VerticalSlices.SingleFile\DemoApp.VerticalSlices.SingleFile.csproj`
Expected: PASS.

## Task 5: Implement Clean Architecture

**Files:**
- Modify: `DemoApp.CleanArchitecture.Api\Program.cs`, `DemoApp.CleanArchitecture.Api\DemoApp.CleanArchitecture.Api.csproj`
- Create: `DemoApp.CleanArchitecture.Api\Endpoints\ProductsEndpoints.cs`, `Requests\CreateProductRequest.cs`, `Requests\GetProductsRequest.cs`, request validators, and `Responses\ProductResponse.cs`
- Create: `DemoApp.CleanArchitecture.Application\Abstractions\IProductRepository.cs`
- Create: `DemoApp.CleanArchitecture.Application\Products\GetProducts\GetProductsQuery.cs`, `GetProductsHandler.cs`
- Create: `DemoApp.CleanArchitecture.Application\Products\GetProductById\GetProductByIdQuery.cs`, `GetProductByIdHandler.cs`
- Create: `DemoApp.CleanArchitecture.Application\Products\CreateProduct\CreateProductCommand.cs`, `CreateProductHandler.cs`
- Create: `DemoApp.CleanArchitecture.Domain\Products\Product.cs`
- Create: `DemoApp.CleanArchitecture.Infrastructure\Products\InMemoryProductRepository.cs`
- Create: `ArchitectureDemo.Tests\CleanArchitectureApiTests.cs`
- Modify: `ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj`

**Interfaces:**
- Consumes: the shared Product contract helper from Task 2.
- `IProductRepository` belongs to Application and exposes asynchronous list, get-by-id, and add methods with `CancellationToken`.
- Application handlers depend on Domain and the Application abstraction only; Infrastructure implements that abstraction; API depends on Application and Infrastructure only for composition and endpoint binding.
- The API project exposes exactly the same Product routes, JSON contracts, status codes, and validation behavior as the other examples.

- [ ] **Step 1: Add the Clean Architecture integration-test fixture**

Add a reference to `DemoApp.CleanArchitecture.Api` and a `WebApplicationFactory<DemoApp.CleanArchitecture.Api.Program>` test fixture calling the shared contract helper. Make the entry point public and partial. Run the test before implementing the behavior.

Run: `dotnet test ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj --filter FullyQualifiedName~CleanArchitectureApiTests`
Expected: FAIL until Product API behavior is added.

- [ ] **Step 2: Add the Domain entity and Application abstraction**

Implement the Product entity with Guid identifier, name, and decimal price. Add the Application-owned repository abstraction with `GetAllAsync(string? search, int? pageSize, CancellationToken)`, `GetByIdAsync(Guid, CancellationToken)`, and `AddAsync(Product, CancellationToken)`. Keep Domain independent of Application, ASP.NET Core, and Infrastructure.

- [ ] **Step 3: Add Application use-case handlers**

Create list, get-by-id, and create request/handler pairs in Application. Handlers call `IProductRepository` and return explicit results that distinguish missing products from successful reads; request validation stays at the API boundary so the same FluentValidation behavior is visible in all four examples. Do not add MediatR or another mediator package.

- [ ] **Step 4: Implement the Infrastructure repository and registration**

Implement a deterministic, thread-safe in-memory repository seeded with the same two fixed-id products, case-insensitive search, page-size limiting, unknown-id behavior, and created product IDs. Register `IProductRepository` to `InMemoryProductRepository` from the API composition root; do not add ASP.NET Core or dependency-injection packages to the Infrastructure class library.

- [ ] **Step 5: Implement and wire the API**

Add Minimal API endpoints, API request validators, and `ProductResponse` mapping. In `Program.cs`, register the repository implementation, Application handlers, and FluentValidation validators; explicitly validate requests asynchronously and map validation, not-found, and created results to the same HTTP behavior as other projects. Remove generated weather endpoints.

- [ ] **Step 6: Run the Clean Architecture contract test and build**

Run: `dotnet test ArchitectureDemo.Tests\ArchitectureDemo.Tests.csproj --filter FullyQualifiedName~CleanArchitectureApiTests`
Expected: PASS for all shared contract assertions.

Run: `dotnet build DemoApp.CleanArchitecture.Api\DemoApp.CleanArchitecture.Api.csproj`
Expected: PASS and no inward dependency from Domain to any other project.

## Task 6: Complete Cross-Project Contract Tests and README

**Files:**
- Modify: `ArchitectureDemo.Tests\ProductApiContract.cs`, all four `*ApiTests.cs` fixtures if needed
- Create: `README.md`
- Modify: `DemoApp.slnx` if final project entries are missing

**Interfaces:**
- Consumes: all four APIs and the shared contract helper.
- Produces: one solution-level test run that exercises the same behavior against every API and learner documentation for running/comparing the examples.

- [ ] **Step 1: Complete the shared contract assertions**

Ensure the shared contract runs against each API and covers:

```csharp
var unfiltered = await client.GetFromJsonAsync<List<ProductResponse>>("/products");
Assert.Equal(2, unfiltered!.Count);

var filtered = await client.GetFromJsonAsync<List<ProductResponse>>("/products?search=DEMO&pageSize=1");
Assert.Single(filtered!);
Assert.Equal("Demo product", filtered[0].Name);

var noMatches = await client.GetFromJsonAsync<List<ProductResponse>>("/products?search=not-found");
Assert.Empty(noMatches!);

var oneItem = await client.GetFromJsonAsync<List<ProductResponse>>("/products?pageSize=1");
Assert.Single(oneItem!);
var maxPageSize = await client.GetAsync("/products?pageSize=100");
Assert.Equal(HttpStatusCode.OK, maxPageSize.StatusCode);
Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/products?pageSize=0")).StatusCode);
Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/products?pageSize=101")).StatusCode);
Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync($"/products?search={new string('x', 101)}")).StatusCode);
Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/products/ffffffff-ffff-ffff-ffff-ffffffffffff")).StatusCode);
var created = await client.PostAsJsonAsync("/products", new { name = "New product", price = 12.50m });
Assert.Equal(HttpStatusCode.Created, created.StatusCode);
Assert.NotNull(created.Headers.Location);
var createdProduct = await created.Content.ReadFromJsonAsync<ProductResponse>();
Assert.NotEqual(Guid.Empty, createdProduct!.Id);
Assert.Equal("New product", createdProduct.Name);
Assert.Equal(12.50m, createdProduct.Price);
Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/products", new { name = "", price = 0m })).StatusCode);
Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/products", new { name = new string('x', 101), price = 12.50m })).StatusCode);
Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/products", new { name = "Invalid price", price = 0m })).StatusCode);
Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/products", new StringContent("{", Encoding.UTF8, "application/json"))).StatusCode);
```

Also send a POST body with a missing `name` and assert 400. Check that failed POSTs do not create products by comparing the list before and after invalid requests.

- [ ] **Step 2: Run the full test suite**

Run: `dotnet test DemoApp.slnx`
Expected: all shared contract tests pass for the four API projects.

- [ ] **Step 3: Write the README architecture map**

Describe the layered, multi-file vertical-slice, single-file vertical-slice, and Clean Architecture projects. Include concise directory trees and explain their boundaries and tradeoffs; do not claim one approach is universally best.

- [ ] **Step 4: Add exact run and OpenAPI commands**

Document the commands below and note that each API uses its own local in-memory data:

```powershell
dotnet run --project DemoApp.Layered
dotnet run --project DemoApp.VerticalSlices
dotnet run --project DemoApp.VerticalSlices.SingleFile
dotnet run --project DemoApp.CleanArchitecture.Api
```

Explain how to use the listening URL printed by `dotnet run` and append `/openapi/v1.json` to inspect the OpenAPI document in Development.

- [ ] **Step 5: Verify the documented commands and final solution build**

Run: `dotnet build DemoApp.slnx`
Expected: PASS for the full solution.

Run: `dotnet test DemoApp.slnx`
Expected: PASS for all architecture contract tests.

Compare the README project paths and run commands with the actual solution entries; correct any mismatch.
