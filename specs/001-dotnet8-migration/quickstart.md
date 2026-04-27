# Quickstart: Migrate to .NET 8

**Feature**: `001-dotnet8-migration`  
**Phase**: 1 — Design  
**Date**: 2025-07-14

---

## What changes

Two project files are updated. No source code changes are required.

### 1. `src/SimpleEventSourcing/SimpleEventSourcing.csproj`

Change `TargetFramework` from `netcoreapp3.1` to `net8.0`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

</Project>
```

### 2. `src/SimpleEventSourcing.UnitTests/SimpleEventSourcing.UnitTests.csproj`

Change `TargetFramework` and update all four package versions:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.4">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\SimpleEventSourcing\SimpleEventSourcing.csproj" />
  </ItemGroup>

</Project>
```

---

## Validation steps

Run these commands from the repository root after making the changes above:

```bash
# 1. Restore packages (must complete with 0 net8.0-incompatibility warnings)
dotnet restore

# 2. Build the solution (must complete with 0 errors)
dotnet build

# 3. Run all tests (must report 4/4 passing, 0 failures)
dotnet test
```

### Expected output

```
Build succeeded.
    0 Warning(s)
    0 Error(s)

Passed!  - Failed: 0, Passed: 4, Skipped: 0, Total: 4
```

---

## No source code changes

The codebase uses only:
- `System` — `DateTime`, `Guid`, `Action<T>`, `Exception`
- `System.Collections.Generic` — `List<T>`, `IEnumerable<T>`
- `System.Linq` — `LastOrDefault()`, `ToArray()`, `Where()`

All of these APIs are present and fully compatible in .NET 8. No compiler errors from removed
or renamed APIs are expected. The build is the final confirmation.

---

## Developer setup (fresh clone)

```bash
git clone <repo-url>
cd SimpleEventSourcingExample
dotnet restore
dotnet test   # should be green immediately after the migration PR is merged
```

Total time from clone to green: < 5 minutes on a standard developer machine (SC-004).
