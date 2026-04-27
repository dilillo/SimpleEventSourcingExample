# Data Model: Migrate to .NET 8

**Feature**: `001-dotnet8-migration`  
**Phase**: 1 — Design  
**Date**: 2025-07-14

---

## Overview

This migration makes **no changes** to the domain model, entity relationships, or public API
surface. All entities and types listed below are carried forward unchanged; they are documented
here to establish a stable baseline for the migrated codebase and to confirm that no entity
modifications are required by FR-001 through FR-007.

---

## Entities

### `CarEvent` (base)

**File**: `src/SimpleEventSourcing/CarEvents.cs`  
**Role**: Immutable record of a single car maintenance occurrence.

| Property | Type | Description | Validation |
|----------|------|-------------|-----------|
| `ID` | `string` | Unique event identifier (GUID string) | Set by `CarEventFactory`; must be non-null |
| `AggregateID` | `string` | Identifier of the owning `CarAggregate` | Set at construction; must be non-null |
| `Version` | `int` | Monotonically increasing position in the stream | Must be `previous.Version + 1` |
| `Date` | `DateTime` | Date the maintenance occurred | No constraint enforced at entity level |
| `Mileage` | `int` | Odometer reading at time of event | Must be ≥ previous event mileage (enforced by aggregate) |
| `Cost` | `decimal` | Cost of the maintenance activity | No constraint enforced at entity level |

**Subtypes**:

| Type | Additional Property | Description |
|------|--------------------|-----------:|
| `OilChanged` | `OilType: string` | Records oil type used (e.g., `"5W-40"`) |
| `BrakesServiced` | _(none)_ | No additional data |
| `BatteryTested` | `BatteryLevel: double` | Battery charge level measured |

**State transitions (append-only)**:
```
(empty) → OilChanged | BrakesServiced | BatteryTested
any → OilChanged | BrakesServiced | BatteryTested
           (except OilChanged → OilChanged is prohibited by aggregate rule)
```

---

### `CarAggregate`

**File**: `src/SimpleEventSourcing/CarAggregate.cs`  
**Role**: Sole authority for enforcing business rules; maintains the mutable write-side state.

| Property | Type | Description |
|----------|------|-------------|
| `AggregateID` | `string` | Stable identifier (e.g., `"Lambo"`) |
| `Version` | `int` | Current stream version (matches last event's `Version`) |
| `Mileage` | `int` | Current mileage (matches last event's `Mileage`) |
| `TotalMaintenceCost` | `decimal` | Running sum of all event costs |
| `EventStream` | `CarEvent[]` | Read-only snapshot of the internal append-only list |

**Command methods**:

| Method | Preconditions | Creates Event |
|--------|---------------|--------------|
| `OilChanged(date, mileage, cost, oilType)` | `mileage ≥ Mileage`; last event must not be `OilChanged` | `OilChanged` |
| `BrakesServiced(date, mileage, cost)` | `mileage ≥ Mileage` | `BrakesServiced` |
| `BatteryTested(date, mileage, cost, batteryLevel)` | `mileage ≥ Mileage` | `BatteryTested` |
| `Load(eventStream)` | _(none — replay path)_ | _(replays existing events)_ |

**Business rule violations**: thrown as `CarException` (see below).

---

### `CarEventFactory`

**File**: `src/SimpleEventSourcing/CarEventFactory.cs`  
**Role**: Constructs strongly-typed `CarEvent` instances with guaranteed base-property population.

| Method | Signature | Notes |
|--------|-----------|-------|
| `Create<T>` | `(aggregateID, version, date, mileage, cost, configurator?)` | Generic factory; `T` must be `CarEvent, new()` |

---

### `CarView` (read model)

**File**: `src/SimpleEventSourcing/CarView.cs`  
**Role**: Read-side projection snapshot; populated exclusively by `CarViewProjector`.

| Property | Type | Description |
|----------|------|-------------|
| `TotalMainteances` | `int` | Count of all events in the projected stream |
| `OilLastChanged` | `DateTime` | Date of the most recent `OilChanged` event |
| `AverageMaintenanceCost` | `decimal` | `TotalCost / TotalMainteances` |

---

### `CarViewProjector`

**File**: `src/SimpleEventSourcing/CarViewProjector.cs`  
**Role**: Derives `CarView` from an ordered `IEnumerable<CarEvent>`.

| Method | Signature | Notes |
|--------|-----------|-------|
| `Project` | `(IEnumerable<CarEvent>) → CarView` | Pure function; no side effects; no aggregate mutation |

---

### `CarException`

**File**: `src/SimpleEventSourcing/CarException.cs`  
**Role**: Domain exception for all business-rule violations.

Extends `Exception`; used exclusively within `CarAggregate` to signal constraint failures
(mileage decrease, duplicate consecutive oil change).

---

## Migration Impact on Data Model

| Area | Change Required | Rationale |
|------|----------------|-----------|
| Entity types / properties | **None** | All BCL types used (`DateTime`, `string`, `int`, `decimal`, `double`) are identical in .NET 8 |
| Relationships | **None** | Inheritance hierarchy (`OilChanged : CarEvent`, etc.) is unchanged |
| Validation rules | **None** | Business rules live in `CarAggregate`; no .NET 8 API changes affect them |
| Serialisation format | **None** | No serialisation is used in this codebase |
| Public API surface | **None** | All `public` members carry forward without modification |
