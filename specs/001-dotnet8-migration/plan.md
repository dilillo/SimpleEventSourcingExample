# Implementation Plan: Migrate to .NET 8

**Branch**: `001-dotnet8-migration` | **Date**: 2025-07-14 | **Spec**: [spec.md](./spec.md)  
**Input**: Feature specification from `specs/001-dotnet8-migration/spec.md`

## Summary

Retarget both `SimpleEventSourcing` (core library) and `SimpleEventSourcing.UnitTests` from
`netcoreapp3.1` to `net8.0`, and upgrade all test NuGet packages to versions that declare
`net8.0` support. No domain logic, event model, or test logic changes are required — the
migration is confined entirely to project file metadata and package version numbers. A clean
`dotnet build` / `dotnet test` pass validates completion.

## Technical Context

**Language/Version**: C# 12 / .NET 8.0 (LTS, SDK 8.0.x available in environment)  
**Primary Dependencies**: None at runtime — the core library has zero third-party runtime
dependencies. Test tooling: `Microsoft.NET.Test.Sdk` 17.12.0, `xunit` 2.9.3,
`xunit.runner.visualstudio` 2.8.2, `coverlet.collector` 6.0.4.  
**Storage**: N/A (in-memory event stream only)  
**Testing**: `dotnet test` via xUnit; 4 existing facts in `UnitTests.cs`  
**Target Platform**: Any OS supported by .NET 8 (Linux, macOS, Windows)  
**Project Type**: Library + companion unit-test project  
**Performance Goals**: `dotnet build` completes in < 60 s; `dotnet test` reports 0 regressions  
**Constraints**: Zero breaking changes to public API surface or test assertions; no new
third-party runtime dependencies introduced  
**Scale/Scope**: 2 `.csproj` files, ~7 source files, 4 unit tests

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. Event-First Design | ✅ PASS | Migration touches no domain event logic |
| II. Aggregate Integrity | ✅ PASS | No changes to `CarAggregate` or `CarException` |
| III. Test-Driven Development | ✅ PASS | All 4 existing tests serve as the acceptance gate; no test logic is modified |
| IV. Immutable Event Stream | ✅ PASS | No changes to event construction or stream storage |
| V. Read/Write Separation | ✅ PASS | `CarViewProjector`/`CarView` untouched |
| Technology Constraints | ⚠️ AUTHORISED DEVIATION | Constitution states "no framework upgrades without an explicit decision recorded in a PR description." This feature spec **is** that explicit decision. The deviation is fully justified — see Complexity Tracking below. |

**Post-design re-check**: All principles still pass. No new deviations introduced in Phase 1.

## Project Structure

### Documentation (this feature)

```text
specs/001-dotnet8-migration/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
└── tasks.md             # Phase 2 output (/speckit.tasks — NOT created by /speckit.plan)
```

*(No `contracts/` directory — library exposes a public C# API only; documented in `data-model.md`.)*

### Source Code (repository root)

```text
src/
├── SimpleEventSourcing/                    # Core library (domain logic)
│   ├── SimpleEventSourcing.csproj          # ← TargetFramework: netcoreapp3.1 → net8.0
│   ├── CarAggregate.cs
│   ├── CarEventFactory.cs
│   ├── CarEvents.cs
│   ├── CarException.cs
│   ├── CarView.cs
│   └── CarViewProjector.cs
└── SimpleEventSourcing.UnitTests/          # Test project
    ├── SimpleEventSourcing.UnitTests.csproj # ← TFM + 4 package versions updated
    └── UnitTests.cs
SimpleEventSourcingExample.sln
```

**Structure Decision**: Single-project library (Option 1). The repository already follows this
layout; no structural changes are needed. Only project file metadata is modified.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Framework upgrade (Technology Constraints) | .NET Core 3.1 reached EOL Dec 2022; upgrading to .NET 8 LTS restores security patch coverage and long-term supportability | Staying on 3.1 is no longer viable; intermediate TFM hops (net5.0, net6.0, net7.0) add unnecessary steps with no benefit given the absence of breaking API removals between 3.1 and 8 |
