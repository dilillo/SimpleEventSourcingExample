# Copilot Instructions

## Project Overview

This repository demonstrates a simple Event Sourcing implementation in C# (.NET Core 3.1). The domain model is a car maintenance tracker built around the following concepts:

- **`CarAggregate`** – enforces business rules and accumulates domain events into an event stream.
- **`CarEvent`** (and subtypes `OilChanged`, `BrakesServiced`, `BatteryTested`) – immutable records of things that happened.
- **`CarEventFactory`** – factory for constructing strongly-typed `CarEvent` instances.
- **`CarViewProjector`** / **`CarView`** – reads an event stream and projects a read-model snapshot.
- **`CarException`** – domain exception thrown when business rules are violated.

## Repository Layout

```
src/
  SimpleEventSourcing/           # Core library (domain logic)
  SimpleEventSourcing.UnitTests/ # xUnit tests (UnitTests.cs)
SimpleEventSourcingExample.sln
```

## Coding Conventions

- Target framework: **netcoreapp3.1**.
- Use **C#** with standard .NET naming conventions (PascalCase for types and public members, camelCase for locals).
- Keep domain objects immutable where possible; state changes must be driven by `CarEvent` mutations inside `CarAggregate`.
- All business-rule violations must throw `CarException`.
- New event types must extend `CarEvent` and be handled in `CarAggregate` and `CarViewProjector`.

## Building & Testing

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run all unit tests
dotnet test
```

Tests live in `src/SimpleEventSourcing.UnitTests/UnitTests.cs` and use **xUnit**.

## Contribution Guidelines

- Add a unit test in `UnitTests.cs` for every new behaviour or bug fix.
- Do not modify existing tests unless the production behaviour they cover has intentionally changed.
- Keep each commit focused on a single logical change.

<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan at
`specs/001-dotnet8-migration/plan.md`.
<!-- SPECKIT END -->
