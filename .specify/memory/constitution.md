<!--
## Sync Impact Report

**Version change**: (unversioned template) → 1.0.0
**Bump rationale**: Initial constitution ratification — all principles authored from scratch.

### Principles added
- I. Event-First Design (new)
- II. Aggregate Integrity (new)
- III. Test-Driven Development — NON-NEGOTIABLE (new)
- IV. Immutable Event Stream (new)
- V. Read/Write Separation (new)

### Sections added
- Technology Constraints (new)
- Development Workflow (new)
- Governance (new)

### Sections removed
- None

### Templates reviewed
- `.specify/templates/plan-template.md` ✅ — Constitution Check gate placeholder is correctly
  deferred to per-feature plan generation; no template changes required.
- `.specify/templates/spec-template.md` ✅ — Scope and requirements structure aligns with
  principles; no changes required.
- `.specify/templates/tasks-template.md` ✅ — TDD instruction ("Tests MUST be written and FAIL
  before implementation") is consistent with Principle III; no changes required.
- `.specify/templates/checklist-template.md` ✅ — Generic structure; no changes required.
- `.github/copilot-instructions.md` ✅ — Coding conventions and build instructions are aligned
  with all five principles; no changes required.

### Deferred TODOs
- None — all placeholders resolved.
-->

# SimpleEventSourcingExample Constitution

## Core Principles

### I. Event-First Design

All state changes in the domain MUST be represented as immutable domain events that are appended
to an event stream. Direct mutation of aggregate state outside of event processing is prohibited.
Every new domain capability MUST be modelled as one or more new `CarEvent` subtypes before any
command method is added to the aggregate.

**Rationale**: Event-first design is the foundational invariant of an event-sourced system.
Bypassing it — even once — breaks replayability, auditability, and the integrity of all projected
read models.

### II. Aggregate Integrity

`CarAggregate` is the sole authority for enforcing business rules. Every business-rule violation
MUST be communicated by throwing a `CarException`; no other exception type may be used for domain
rule failures. Command methods MUST validate preconditions (e.g., non-decreasing mileage, no
duplicate consecutive oil change) before creating and appending an event.

**Rationale**: Centralising invariant enforcement in the aggregate prevents rule duplication
across services, projectors, or tests and ensures a single, auditable enforcement point.

### III. Test-Driven Development (NON-NEGOTIABLE)

A unit test in `UnitTests.cs` MUST be written before or alongside every new behaviour or bug fix.
The test MUST fail before implementation and pass once implementation is complete
(Red → Green → Refactor). Existing tests MUST NOT be modified unless the production behaviour
they cover has intentionally changed and the change has been reviewed.

**Rationale**: The test suite is the living specification of the library. Untested code has no
verified contract and cannot be safely refactored or extended.

### IV. Immutable Event Stream

Once a `CarEvent` has been appended to the aggregate's event stream it MUST NOT be altered,
deleted, or re-ordered. Event properties MUST be set at construction time via `CarEventFactory`
and treated as read-only thereafter. Reconstructing aggregate state MUST always produce the same
result given the same ordered event stream.

**Rationale**: Immutability guarantees deterministic replay, which is the core value proposition
of event sourcing. Any mutable event record undermines audit trails and temporal queries.

### V. Read/Write Separation

Read models (e.g., `CarView`) MUST be derived exclusively from the event stream by a dedicated
projector class (e.g., `CarViewProjector`). Projectors MUST NOT mutate aggregate state, write
back to the event stream, or maintain independent persistent storage. New read-model requirements
MUST be satisfied by adding a new projector or extending an existing one — never by adding
query logic to `CarAggregate`.

**Rationale**: Clean separation of reads and writes allows read models to evolve independently,
supports multiple simultaneous views of the same event stream, and prevents projectors from
accidentally violating aggregate invariants.

## Technology Constraints

- **Language / Framework**: C# targeting `netcoreapp3.1`. No framework upgrades without an
  explicit decision recorded in a PR description.
- **Naming conventions**: PascalCase for all types and public members; camelCase for local
  variables. Standard .NET naming MUST be followed throughout.
- **Domain exceptions**: `CarException` is the only permitted exception type for business-rule
  violations. System exceptions (e.g., `ArgumentNullException`) MAY be used for programming
  errors unrelated to domain rules.
- **New event types**: MUST extend `CarEvent`, MUST be handled in `CarAggregate` (command method
  + `Mutate` application), and MUST be covered by at least one unit test.
- **Dependencies**: Third-party packages MUST be justified in the PR description. Test projects
  MAY use xUnit and related helpers; the core library MUST have zero runtime third-party
  dependencies.

## Development Workflow

- **Build**: `dotnet restore && dotnet build`
- **Test**: `dotnet test`
- All tests MUST pass before a PR is merged.
- Each commit MUST be focused on a single logical change.
- PR descriptions MUST reference the principle(s) exercised by the change.
- Contributions that add a new event type MUST include: the event class, the aggregate command
  method, any projector update, and a corresponding unit test — all in the same PR.

## Governance

This constitution supersedes all other development guidelines for
SimpleEventSourcingExample. Where a conflict exists between this document and any other
guidance (README, inline comments, PR comments), this constitution takes precedence.

**Amendment procedure**: Amendments require a dedicated PR that (a) updates this file,
(b) increments the version according to semantic versioning rules below, (c) updates the
`Last Amended` date, and (d) includes a Sync Impact Report at the top of the file.

**Versioning policy**:
- MAJOR — backward-incompatible removal or redefinition of a principle.
- MINOR — addition of a new principle or section, or material expansion of existing guidance.
- PATCH — clarifications, wording improvements, or non-semantic refinements.

**Compliance review**: All PRs MUST be reviewed against the Constitution Check section of the
relevant `plan.md`. Any principle violation MUST either be resolved before merge or explicitly
justified in the Complexity Tracking table of the plan with a documented rationale.

Runtime development guidance is maintained in `.github/copilot-instructions.md`.

**Version**: 1.0.0 | **Ratified**: 2026-04-26 | **Last Amended**: 2026-04-26
