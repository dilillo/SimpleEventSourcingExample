# Research: Migrate to .NET 8

**Feature**: `001-dotnet8-migration`  
**Phase**: 0 — Research  
**Date**: 2025-07-14

---

## 1. Target Framework Moniker (TFM)

**Decision**: Use `net8.0` as the new `TargetFramework` in both project files.

**Rationale**: `net8.0` is the correct TFM for .NET 8 LTS. It replaces the deprecated
`netcoreapp3.1` moniker. All standard BCL APIs used in this codebase (`System`,
`System.Collections.Generic`, `System.Linq`) are available and fully compatible with
`net8.0` — no source-level changes are required.

**Alternatives considered**:
- `net6.0` — LTS, but superseded by .NET 8 with no benefit for this migration.
- `net7.0` — STS (Standard Term Support), reached EOL May 2024; not suitable.
- `net9.0` / `net10.0` — Current/preview releases; not LTS. Spec explicitly targets .NET 8 LTS.

---

## 2. Breaking API Changes (3.1 → 8.0)

**Decision**: No source code changes needed for API compatibility.

**Rationale**: The codebase uses only three BCL namespaces:
- `System` — `DateTime`, `Guid`, `Action<T>`, `Exception` — all stable and unchanged.
- `System.Collections.Generic` — `List<T>`, `IEnumerable<T>` — unchanged.
- `System.Linq` — `LastOrDefault()`, `ToArray()`, `Where()` — unchanged.

No platform-specific, runtime-specific, or removed APIs are in use. The .NET 8 migration
guide confirms these APIs have no breaking changes between 3.1 and 8.0 in the categories
used here. The clean build will be the final validation gate.

**Alternatives considered**: N/A — this is a discovery task, not a choice.

---

## 3. NuGet Package Versions for .NET 8

**Decision**: Update test project packages to the following versions:

| Package | Current | Target | Notes |
|---------|---------|--------|-------|
| `Microsoft.NET.Test.Sdk` | 16.7.1 | **17.12.0** | First version requiring .NET ≥ 4.6.2 / net8.0-compatible |
| `xunit` | 2.4.1 | **2.9.3** | Latest stable; full .NET 8 support confirmed |
| `xunit.runner.visualstudio` | 2.4.3 | **2.8.2** | Matches xunit 2.x series; .NET 8 compatible |
| `coverlet.collector` | 1.3.0 | **6.0.4** | Updated to match current SDK toolchain |

**Rationale**: 
- `Microsoft.NET.Test.Sdk` 16.x predates .NET 5 and does not ship TFM-specific assets for
  `net8.0`. Version 17.x ships proper multi-TFM props/targets and is required for `dotnet test`
  to discover tests correctly on .NET 8.
- `xunit` 2.9.x is the latest stable release of the v2 series and has no breaking API changes
  from 2.4.x — existing test code compiles without modification.
- `xunit.runner.visualstudio` 2.8.x is required to pair with `xunit` 2.9.x; 2.4.x runners
  do not correctly handle .NET 8 test host discovery.
- `coverlet.collector` 6.x aligns with the .NET 8 SDK's coverage toolchain; 1.x is incompatible
  with SDK 17.x.
- **Security**: All four recommended versions were checked against the GitHub Advisory Database —
  zero known vulnerabilities.

**Alternatives considered**:
- Remaining on xunit 2.4.x — incompatible with `Microsoft.NET.Test.Sdk` 17.x on .NET 8;
  test discovery fails.
- Migrating to xunit v3 — a major API break requiring test rewrites; out of scope per FR-003
  ("no changes to test logic").

---

## 4. SDK Version Pinning

**Decision**: No `global.json` pinning required.

**Rationale**: The build environment has .NET SDK 8.0.419 and 8.0.319 installed. A `net8.0`
TFM will automatically use the highest compatible 8.x SDK via roll-forward rules. Because the
project uses no SDK-specific features or preview APIs, SDK pinning would only add friction
without benefit. If a future CI pipeline requires an exact SDK version, a `global.json` can be
added at that point.

**Alternatives considered**: Pinning to `8.0.419` via `global.json` — unnecessary friction;
deferred.

---

## 5. `IsPackable` Property

**Decision**: Retain `<IsPackable>false</IsPackable>` in the test project; no changes needed.

**Rationale**: This property correctly prevents `dotnet pack` from packaging the test project as
a NuGet artifact. Its behaviour is identical in .NET 8. No change required.

---

## Summary: All NEEDS CLARIFICATION Items Resolved

| Item | Resolution |
|------|-----------|
| Correct TFM for .NET 8 | `net8.0` |
| Breaking API changes | None — no source changes required |
| Compatible package versions | `Microsoft.NET.Test.Sdk` 17.12.0, `xunit` 2.9.3, `xunit.runner.visualstudio` 2.8.2, `coverlet.collector` 6.0.4 |
| SDK pinning | Not required |
| Security advisories | None found |
