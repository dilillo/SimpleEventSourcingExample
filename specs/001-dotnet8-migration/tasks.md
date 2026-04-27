---

description: "Task list template for feature implementation"
---

# Tasks: Migrate to .NET 8

**Input**: Design documents from `/specs/001-dotnet8-migration/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, quickstart.md ✅

**Tests**: No new test tasks generated — the 4 existing xUnit facts in `UnitTests.cs` serve as the complete acceptance gate per FR-003 and SC-002. Test tasks would require rewriting test logic which is explicitly out of scope.

**Organization**: Tasks are grouped by user story. US1 (retarget TFM) and US2 (upgrade NuGet packages) map to the two `.csproj` files in the repository.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to (US1, US2)
- Exact file paths are included in each description

---

## Phase 1: Setup (Verify Environment)

**Purpose**: Confirm the .NET 8 SDK is available and establish the pre-migration baseline before making any changes.

- [X] T001 Verify .NET 8 SDK is installed (`dotnet --list-sdks`) and confirm current baseline by running `dotnet build` against `SimpleEventSourcingExample.sln` to record pre-migration state

**Checkpoint**: Environment confirmed ready — .NET 8 SDK present, current 3.1 build state known

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: No shared infrastructure changes are required for this migration — the repository layout, solution file, and domain source files (`CarAggregate.cs`, `CarEvents.cs`, `CarEventFactory.cs`, `CarView.cs`, `CarViewProjector.cs`, `CarException.cs`, `UnitTests.cs`) are all carried forward **unchanged**. Phase 2 is intentionally empty; user story phases may begin immediately after Phase 1.

**⚠️ CRITICAL**: Confirm no source-code changes are needed before proceeding (see research.md §2 — no removed or renamed BCL APIs are in use).

**Checkpoint**: Foundation confirmed — proceed directly to User Story phases

---

## Phase 3: User Story 1 — Build and Run on .NET 8 (Priority: P1) 🎯 MVP

**Goal**: Retarget both project files from `netcoreapp3.1` to `net8.0` so that `dotnet build` succeeds on .NET 8 LTS.

**Independent Test**: Run `dotnet build SimpleEventSourcingExample.sln` after applying T002 and T003. Build must complete with 0 errors. (Full test pass requires US2 packages to also be updated, but the build itself can be validated independently.)

### Implementation for User Story 1

- [X] T002 [P] [US1] Update `<TargetFramework>` from `netcoreapp3.1` to `net8.0` in `src/SimpleEventSourcing/SimpleEventSourcing.csproj`
- [X] T003 [P] [US1] Update `<TargetFramework>` from `netcoreapp3.1` to `net8.0` in `src/SimpleEventSourcing.UnitTests/SimpleEventSourcing.UnitTests.csproj`

**Checkpoint**: Both projects now declare `net8.0` as their target framework. User Story 1 implementation complete — build validation follows in Polish phase.

---

## Phase 4: User Story 2 — Updated Dependencies Remain Compatible (Priority: P2)

**Goal**: Upgrade all four NuGet package references in the test project to versions that declare `net8.0` support, eliminating any restore warnings about unsupported frameworks.

**Independent Test**: After applying T004–T007, run `dotnet restore` and confirm zero warnings referencing `net8.0` incompatibility. Then run `dotnet test` to confirm all 4 tests pass.

### Implementation for User Story 2

> All tasks below modify `src/SimpleEventSourcing.UnitTests/SimpleEventSourcing.UnitTests.csproj`. Apply sequentially (same file).

- [X] T004 [US2] Update `Microsoft.NET.Test.Sdk` from `16.7.1` to `17.12.0` in `src/SimpleEventSourcing.UnitTests/SimpleEventSourcing.UnitTests.csproj`
- [X] T005 [US2] Update `xunit` from `2.4.1` to `2.9.3` in `src/SimpleEventSourcing.UnitTests/SimpleEventSourcing.UnitTests.csproj`
- [X] T006 [US2] Update `xunit.runner.visualstudio` from `2.4.3` to `2.8.2` in `src/SimpleEventSourcing.UnitTests/SimpleEventSourcing.UnitTests.csproj`
- [X] T007 [US2] Update `coverlet.collector` from `1.3.0` to `6.0.4` in `src/SimpleEventSourcing.UnitTests/SimpleEventSourcing.UnitTests.csproj`

**Checkpoint**: All package references now target .NET 8-compatible versions. User Stories 1 AND 2 are both complete — full validation follows.

---

## Phase 5: Polish & Validation

**Purpose**: Run the three acceptance commands from `quickstart.md` to confirm all success criteria (SC-001 through SC-004) are satisfied before merging.

- [X] T008 Run `dotnet restore` from the repository root and confirm 0 errors and 0 warnings about `net8.0`-incompatible packages (SC-003)
- [X] T009 Run `dotnet build SimpleEventSourcingExample.sln` from the repository root and confirm build succeeds with 0 errors and 0 framework-targeting warnings (SC-001 / FR-002)
- [X] T010 Run `dotnet test` from the repository root and confirm output shows `Failed: 0, Passed: 4, Skipped: 0, Total: 4` with 0 regressions (SC-002 / FR-003)

**Checkpoint**: All acceptance criteria met — migration is complete and ready to merge.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: N/A for this migration — no blocking infra changes needed
- **User Story 1 (Phase 3)**: Depends on Phase 1 completion; T002 and T003 can run **in parallel** (different files)
- **User Story 2 (Phase 4)**: Depends on Phase 3 completion (TFM must be set before package updates are meaningful); T004–T007 must run **sequentially** (same file)
- **Polish (Phase 5)**: Depends on all of Phase 3 and Phase 4 completing

### User Story Dependencies

- **US1 (P1)**: Can start after T001 — no dependency on US2
- **US2 (P2)**: Should follow US1 (TFM changes first, then package updates) for clarity; technically T004–T007 only modify the test `.csproj`, which is separate from the library `.csproj`, so US2 is still testable independently after T003 is applied

### Within Each User Story

- US1: T002 and T003 are independent (different files) — run in parallel
- US2: T004 → T005 → T006 → T007 are sequential (same file) — apply in order

---

## Parallel Execution Examples

### User Story 1 — Parallel TFM Updates

```bash
# Both edits touch different files — safe to apply in parallel:
Task T002: Update TargetFramework in src/SimpleEventSourcing/SimpleEventSourcing.csproj
Task T003: Update TargetFramework in src/SimpleEventSourcing.UnitTests/SimpleEventSourcing.UnitTests.csproj
```

### User Story 2 — Sequential Package Updates (same file)

```bash
# Apply T004–T007 in order within src/SimpleEventSourcing.UnitTests/SimpleEventSourcing.UnitTests.csproj:
T004 → Microsoft.NET.Test.Sdk: 16.7.1 → 17.12.0
T005 → xunit: 2.4.1 → 2.9.3
T006 → xunit.runner.visualstudio: 2.4.3 → 2.8.2
T007 → coverlet.collector: 1.3.0 → 6.0.4
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 (T001): Verify environment
2. Complete Phase 3 (T002–T003): Retarget both projects to `net8.0`
3. **STOP and VALIDATE**: Run `dotnet build` — confirms TFM migration works
4. Optionally merge as a partial milestone (build passes, tests may require US2 to pass)

### Full Migration Delivery

1. Phase 1 → Phase 3 → Phase 4 → Phase 5 (sequential as listed above)
2. Each phase is a discrete, verifiable increment
3. `dotnet test` in Phase 5 is the definitive green gate before merging

### Single Developer — Recommended Order

```
T001 → T002 + T003 (parallel) → T004 → T005 → T006 → T007 → T008 → T009 → T010
```

Total wall-clock time: < 15 minutes (< 5 minutes of edits; remainder is restore/build/test runtime)

---

## Notes

- [P] marks tasks that can run in parallel (T002 and T003 only — different files)
- [US1] / [US2] labels map each task to its user story for full traceability to spec.md
- No domain logic, event model, or test assertion changes are made anywhere in this migration
- The solution file (`SimpleEventSourcingExample.sln`) requires **no changes** — it is already valid and references both projects correctly (FR-006)
- `<IsPackable>false</IsPackable>` in the test project requires **no changes** — identical behaviour in .NET 8 (research.md §5)
- If any unexpected compiler errors appear after T002–T003, cross-reference research.md §2 — the BCL APIs in use have no breaking changes between 3.1 and 8.0
- Commit after T003 (US1 complete) and again after T007 (US2 complete) for clean git history
