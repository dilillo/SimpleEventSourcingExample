# Feature Specification: Migrate to .NET 8

**Feature Branch**: `001-dotnet8-migration`  
**Created**: 2025-07-14  
**Status**: Draft  
**Input**: User description: "migrate to .net 8"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Build and Run on .NET 8 (Priority: P1)

As a developer working on the SimpleEventSourcingExample project, I want the library and test projects to target .NET 8 so that I can build, run, and test the code using the current .NET LTS release and benefit from runtime improvements.

**Why this priority**: This is the core migration goal. Without successfully building on .NET 8, no other migration work can be validated. Delivering a compiling, passing build on .NET 8 provides immediate value to all contributors.

**Independent Test**: Can be fully tested by running `dotnet build` and `dotnet test` against the solution and confirming zero errors and all existing tests pass.

**Acceptance Scenarios**:

1. **Given** the solution targets .NET 8, **When** a developer runs `dotnet build`, **Then** the build completes successfully with no errors or breaking-change warnings.
2. **Given** the solution targets .NET 8, **When** a developer runs `dotnet test`, **Then** all existing unit tests pass without modification to test logic.
3. **Given** a developer clones the repository fresh, **When** they run `dotnet restore` followed by `dotnet build`, **Then** all packages resolve and the build succeeds without manual intervention.

---

### User Story 2 - Updated Dependencies Remain Compatible (Priority: P2)

As a developer, I want all NuGet package references upgraded to versions that are compatible with .NET 8 so that no deprecated or unsupported packages are present after the migration.

**Why this priority**: Outdated packages that don't support .NET 8 will cause build failures or runtime issues. Updating them is a prerequisite to a clean build but is secondary to validating the build itself.

**Independent Test**: Can be fully tested by inspecting package references and confirming no packages target only netcoreapp3.1 or earlier, and no `dotnet restore` warnings about unsupported frameworks appear.

**Acceptance Scenarios**:

1. **Given** the migrated solution, **When** `dotnet restore` is executed, **Then** no warnings about incompatible or deprecated package versions targeting .NET Core 3.1 are emitted.
2. **Given** the test project references updated test SDK packages, **When** `dotnet test` runs, **Then** the test runner discovers and executes all tests as before with no runner-compatibility errors.
3. **Given** the updated package references, **When** reviewing the solution, **Then** no package is pinned to a version that does not support `net8.0`.

---

### Edge Cases

- What happens if any source code uses APIs that were removed between .NET Core 3.1 and .NET 8? The build must fail clearly with a compiler error identifying the breaking change so the developer can address it.
- How does the system handle transitive dependency conflicts introduced by package upgrades? `dotnet restore` must either resolve them automatically or report a clear conflict error.
- What if the build succeeds on .NET 8 but test results differ from the baseline? All tests must still pass; any regression must be identified and resolved.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Both `SimpleEventSourcing` and `SimpleEventSourcing.UnitTests` projects MUST target `net8.0` as their `TargetFramework`.
- **FR-002**: The solution MUST build successfully (`dotnet build`) with zero errors on .NET 8 SDK.
- **FR-003**: All existing unit tests MUST continue to pass (`dotnet test`) after the migration with no changes to test logic.
- **FR-004**: All NuGet package references MUST be updated to versions that declare support for `net8.0` (or `netstandard2.x` compatible with .NET 8).
- **FR-005**: The `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`, and `coverlet.collector` packages in the test project MUST be updated to current stable versions compatible with .NET 8.
- **FR-006**: The solution file (`SimpleEventSourcingExample.sln`) MUST remain valid and loadable after migration.
- **FR-007**: No deprecated or removed .NET Core 3.1-only APIs MAY remain in source code after migration.

### Key Entities

- **SimpleEventSourcing project** (`SimpleEventSourcing.csproj`): The main library currently targeting `netcoreapp3.1`; must be re-targeted to `net8.0`.
- **SimpleEventSourcing.UnitTests project** (`SimpleEventSourcing.UnitTests.csproj`): The test project currently targeting `netcoreapp3.1` with pinned test tooling; must be re-targeted to `net8.0` and package references updated.
- **NuGet package references**: Test SDK and runner packages pinned to 2020-era versions; must be updated to .NET 8–compatible releases.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: `dotnet build` completes in under 60 seconds with 0 errors and 0 warnings related to framework targeting or deprecated APIs.
- **SC-002**: `dotnet test` reports 100% of previously passing tests still passing (0 regressions, 0 new failures).
- **SC-003**: `dotnet restore` completes with 0 package-compatibility warnings for the `net8.0` target framework.
- **SC-004**: A developer unfamiliar with the migration can clone the repository, run `dotnet test`, and see a green build within 5 minutes with no additional setup steps.

## Assumptions

- The codebase contains no custom platform-specific code that targets Windows-only APIs or other OS-specific features unavailable on .NET 8.
- .NET Core 3.1 reached end-of-life (December 2022); the migration goes directly to .NET 8 LTS without intermediate framework hops.
- No breaking API removals between .NET Core 3.1 and .NET 8 affect the current source code (this is assumed based on the project's simple event sourcing scope, but will be confirmed by a clean build).
- The existing unit tests fully cover the library's public surface and serve as the acceptance gate for the migration.
- Package versions for `Microsoft.NET.Test.Sdk`, `xunit`, and related packages will be updated to the latest stable releases compatible with .NET 8 at time of migration.
- No CI/CD pipeline configuration changes are required beyond the project file updates (e.g., the runtime is already available in the build environment, or that is handled separately).
