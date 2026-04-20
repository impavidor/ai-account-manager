# Design Decisions

Architectural decisions that shape the structure of the system. Each entry records what was decided, why, and what was explicitly ruled out.

---

## DD-001 — Bounded Context separation via folders, not projects

**Decision:** The two Bounded Contexts (`Administration` and `SelfService`) are separated into distinct folders within `AccountManager.Domain`, with a `Shared` folder for cross-cutting domain concepts. They are not split into separate C# projects.

**Rationale:** The BCs are small and always deployed together in v1. Separate projects would introduce overhead (project references, explicit cross-BC contracts, NuGet versioning) before the cross-BC communication patterns are understood. Folder separation is sufficient to enforce naming conventions and namespace boundaries, and is trivially promotable to separate projects later.

**Ruled out:** Separate `AccountManager.Domain.Administration` and `AccountManager.Domain.SelfService` projects. Deferred until the Application layer exists and cross-BC communication patterns are stable.

**Folder structure:**
```
AccountManager.Domain/
├── Administration/   ← namespace AccountManager.Domain.Administration
├── SelfService/      ← namespace AccountManager.Domain.SelfService
└── Shared/           ← namespace AccountManager.Domain.Shared
```

---

## DD-002 — `Entity<T>` and `AggregateRoot<T>` live in `AccountManager.Common`

**Decision:** The abstract base classes `Entity<T>` and `AggregateRoot<T>` are defined in `AccountManager.Common`, not in `AccountManager.Domain`.

**Rationale:** These are structural primitives used by all future domain projects, not domain concepts themselves. `AccountManager.Domain` already depends on `AccountManager.Common`, so no new project reference is introduced. Placing them in `Common` keeps them available to any domain project added in future without creating a cross-domain dependency.

**Type constraint:** `where T : IEquatable<T>`. All ID types are `record` types, which implement `IEquatable<T>` by default — no changes to existing ID types required.

**Equality:** Identity-based equality using `Id.Equals(other.Id)`. No `GetType()` check — there is no current use case for comparing entities of different types.

---

## DD-003 — `AggregateRoot<T>` is a marker class; domain events are deferred

**Decision:** `AggregateRoot<T>` inherits `Entity<T>` and adds no members. Domain event collection and dispatch are explicitly out of scope for v1.

**Rationale:** Introducing domain event infrastructure prematurely would add complexity with no immediate consumer. The marker class establishes the concept in the type hierarchy and makes domain events an additive change when the time comes.

---

## DD-004 — Administration BC services take `ContactId` for the actor, not `SystemAdminId`

**Decision:** `IActivateContactService.ActivateAsync` and `IDeleteContactService.DeleteAsync` take `ContactId actorId` as the actor parameter instead of `SystemAdminId actorId`.

**Rationale:** The Administration BC operates exclusively with `Contact` aggregates identified by `ContactId`. Using `SystemAdminId` (a SelfService BC type) as a parameter introduced a direct cross-BC type dependency inside the Administration domain services. Switching to `ContactId` keeps the Administration BC fully self-contained. The self-action guard simplifies to `contactId == actorId` (record equality, same type) rather than cross-type `.Value` comparison.

**Responsibility shift:** The Application layer is responsible for resolving the logged-in user's `SystemAdminId` to their `ContactId` before calling domain services.

---

## DD-005 — `ContactStatus` is a shared domain concept

**Decision:** `ContactStatus` lives in `AccountManager.Domain.Shared`, not in either BC.

**Rationale:** Both BCs reference `ContactStatus`. In the Administration BC it is a mutable field with enforced transitions. In the SelfService BC it is a read-only field reflecting lifecycle state owned by the Administration BC. Duplicating the enum in each BC would create divergence risk. Placing it in `Shared` makes the dependency explicit and unambiguous.

---

## DD-006 — `ContactName` belongs to the Administration BC

**Decision:** `ContactName` (the `FullName` / `OrganizationName` discriminated union) lives in `AccountManager.Domain.Administration`.

**Rationale:** `ContactName` is only used by the `Contact` aggregate. SelfService aggregates use `ProviderName` exclusively. There is no shared usage that would justify moving it to `Shared`.
