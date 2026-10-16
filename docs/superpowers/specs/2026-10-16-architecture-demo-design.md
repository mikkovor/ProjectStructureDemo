# Architecture Demo Design

## Purpose

Create an educational .NET solution that demonstrates and compares several
ways to organize the same small Product API. The examples should make
architectural differences visible without introducing unrelated infrastructure
or feature differences.

## Current state

The solution currently contains one `DemoApp.API` project targeting .NET 10.
It is the default Minimal API template with a weather forecast endpoint and no
existing product domain or persistence layer.

## Goals

- Demonstrate four distinct organization styles in one solution.
- Keep each style in a separately runnable API project.
- Implement equivalent Product behavior across all four examples so learners
  can compare code organization rather than feature scope.
- Keep the sample self-contained by using in-memory data.
- Document the purpose, structure, and run instructions for each project.

## Non-goals

- Production-grade persistence, authentication, authorization, or deployment.
- Benchmarking the architectures or declaring one universally best.
- Building a complete e-commerce domain or broad CRUD application.
- Sharing implementation code across the demo projects; the examples should
  remain independently understandable.

## Project layout

The solution will contain the following API projects:

1. **Layered** — one API project organized by technical responsibility:
   controllers, services, repositories, models, and DTOs. The request flow is
   Controller → Service → Repository.
2. **VerticalSlices** — one API project organized by feature/use case. Each
   operation has a feature folder and its endpoint, validation, and contracts
   are split into focused files.
3. **VerticalSlices.SingleFile** — one API project organized by feature, with
   each operation's implementation grouped in one file. For example,
   `GetProducts.cs` contains the endpoint, validator, DTO/request, and response
   types needed by that operation.
4. **CleanArchitecture** — separate API, Application, Domain, and
   Infrastructure projects. Dependencies point inward: API and Infrastructure
   depend on Application/Domain as required, while Domain has no dependency
   on outer layers. Application defines persistence abstractions; Infrastructure
   implements them.

Each project will use the same target framework and equivalent API behavior.
The existing template project will be adapted as one of the examples rather
than kept as an extra, unrelated weather API.

## Shared Product demonstration

Each example will expose the same small API:

- `GET /products` returns the products.
- `GET /products/{id}` returns a product or a not-found response.
- `POST /products` validates and creates a product, returning the created
  resource.

`GET /products` accepts optional `search` and `pageSize` query parameters.
The validator checks that `search` does not exceed a fixed maximum length and
that `pageSize`, when supplied, is within the range 1–100. The returned list
uses those values consistently in every project.

The sample Product has an identifier, name, and price. Each project uses its
own in-memory implementation and starts with a small, deterministic sample
dataset. No database or cross-project shared implementation is introduced.

The POST request validator will apply the same rules in every example
(non-empty name with a reasonable maximum length and a positive price). In the
single-file slice project, `GetProducts.cs` contains the GET query DTO,
response, validator, and Minimal API endpoint; the POST operation has its own
co-located feature file. Other projects place those responsibilities according
to their demonstrated organization.

## Runtime and dependencies

Use the repository's current .NET 10 target framework across projects. Add only
the packages needed for the demonstrated behavior. FluentValidation will be
used for request validation in the examples, with registration and invocation
following a consistent approach so validation behavior does not vary by
architecture.

## Documentation

The solution README will explain:

- what each project demonstrates and where its main code lives;
- how to run each API and access its OpenAPI description;
- the common Product behavior and validation rules;
- that the examples illustrate tradeoffs and are not a universal ranking.

## Verification criteria

- The solution restores and builds with all four API projects.
- Each project can run independently and exposes the same three Product
  operations.
- Equivalent requests produce equivalent success, validation, and not-found
  behavior across projects.
- The single-file `GetProducts.cs` demonstrates co-location of the endpoint,
  query DTO, validator, and response relevant to that operation.
- README instructions are sufficient to identify and run each example.
