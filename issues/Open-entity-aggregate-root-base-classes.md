# Introduce Entity<T> and AggregateRoot<T> Base Classes

## Problem Statement

All four aggregates (`Contact`, `Provider`, `ProviderAdmin`, `SystemAdmin`) manually re-implement identity-based equality (`Equals` / `GetHashCode`) by ID. There is no shared base class that encodes the DDD concept of an entity (identity-based equality) or distinguishes aggregate roots from inner entities. As the domain grows, this pattern will be duplicated across every new aggregate.

## Solution

Introduce `Entity<T>` and `AggregateRoot<T>` abstract base classes in `AccountManager.Common`, parameterised on the ID type. All existing aggregates inherit from `AggregateRoot<T>`. Equality is centralised in `Entity<T>`.

## Commits

1. **Add `Entity<T>` to `AccountManager.Common`** — create `AccountManager.Common/Domain/Entity.cs`. Abstract class, single `public T Id { get; }` property, `protected` constructor. Implements `IEquatable<Entity<T>>`. Overrides `Equals(object?)` and `GetHashCode()` using `Id.Equals()`. Constraint: `where T : IEquatable<T>`. No other members. Solution must build.

2. **Add `AggregateRoot<T>` to `AccountManager.Common`** — create `AccountManager.Common/Domain/AggregateRoot.cs`. Abstract class inheriting `Entity<T>`, same `where T : IEquatable<T>` constraint. No additional members — marker only for now. Solution must build.

3. **Migrate `Contact` to `AggregateRoot<ContactId>`** — remove manual `Equals` / `GetHashCode` overrides, call `base(id)` in constructor. All tests must pass.

4. **Migrate `Provider` to `AggregateRoot<ProviderId>`** — same pattern. All tests must pass.

5. **Migrate `ProviderAdmin` to `AggregateRoot<ProviderAdminId>`** — same pattern. All tests must pass.

6. **Migrate `SystemAdmin` to `AggregateRoot<SystemAdminId>`** — same pattern. All tests must pass.

## Decision Document

- **Location**: `Entity<T>` and `AggregateRoot<T>` live in `AccountManager.Common` under a `Domain/` subfolder, namespace `AccountManager.Common.Domain`. `AccountManager.Domain` already depends on `AccountManager.Common`, so no new project reference is needed.
- **ID constraint**: `where T : IEquatable<T>`. All current ID types are `record` structs/classes, which implement `IEquatable<T>` by default — no changes to ID types required.
- **Equality**: `Id.Equals(other.Id)` — no `GetType()` cross-type check. There is no current use case for comparing entities of different types.
- **`AggregateRoot<T>`**: Marker class only — inherits `Entity<T>`, adds nothing. Domain events are out of scope.
- **Constructor visibility**: `Entity<T>` has a `protected` constructor. Each aggregate controls its own constructor visibility (e.g. `Contact` keeps its `public` constructor for infrastructure reconstitution; `Provider` keeps its `private` constructor with a static factory).

## Testing Decisions

- No new tests are introduced — this is a structural refactor.
- Existing aggregate and service tests provide sufficient coverage: any regression in equality or construction will surface immediately.
- After each migration commit, the full test suite must be green before proceeding to the next aggregate.

## Out of Scope

- Domain events on `AggregateRoot<T>`.
- Any constraint beyond `IEquatable<T>` on the ID type (e.g. no `IEntityId` marker interface).
- Changes to value object base types (`record` remains the right choice for value objects).
- Introducing `Entity<T>` for non-aggregate-root entities — no inner entities exist yet.

