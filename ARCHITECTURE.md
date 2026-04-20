# Architecture

## Principles

This application follows **Clean Architecture**, **Domain-Driven Design (DDD)**, and **CQRS**.

| Principle              | Decision                                                                 |
| ---------------------- | ------------------------------------------------------------------------ |
| **Clean Architecture** | Dependencies flow inward; domain has no dependency on infrastructure     |
| **DDD**                | Domain model is the source of truth; ubiquitous language is enforced     |
| **CQRS**               | Commands (writes) and Queries (reads) are handled by separate pipelines with separate infrastructure |

---

## Tech Stack

| Concern              | Decision                          | Notes                                                                 |
| -------------------- | --------------------------------- | --------------------------------------------------------------------- |
| **Language**         | C# (.NET 9)                       | Greenfield — no legacy constraints                                    |
| **Project type**     | Class library (DLL per layer)     | Each layer is a separate `.csproj`                                    |
| **Value objects**    | `record` types                    | Built-in structural equality, `init`-only immutability, no boilerplate |
| **Error handling**   | `Result<T, Error>` pattern        | Honest method signatures; no exceptions for domain invariants         |
| **Result library**   | `CSharpFunctionalExtensions`      | Provides `Result<T, E>` and `Unit`; thin utility, not infrastructure  |
| **Error hierarchy**  | Custom `Error` abstract record    | Defined in `Common`; specific errors (e.g. `InvalidNpiError`) derive from it |
| **Test framework**   | NUnit                             |                                                                       |
| **Assertion library**| FluentAssertions                  | Best failure messages; natural language assertions                    |
| **Mocking**          | None (domain layer)               | Pure domain objects only; no external dependencies to mock            |

### Error pattern

Value object factories and aggregate mutation methods use honest signatures:

```csharp
// Value object — private constructor, honest factory
public record Npi
{
    private Npi(string value) => Value = value;
    public string Value { get; }
    public static Result<Npi, Error> Create(string value) => ...
}

// Base error in Common
public abstract record Error(string Message);

// Specific errors in Domain
public record InvalidNpiError(string Value) : Error($"'{Value}' is not a valid NPI");
public record InvalidStatusTransitionError(string From, string To) : Error($"Cannot transition from {From} to {To}");
```

---

## Layer Map

```
┌─────────────────────────────────────────────┐
│                  Presentation               │  HTTP handlers, request/response DTOs
├─────────────────────────────────────────────┤
│                  Application                │  Commands, Queries, Handlers, Use Cases
├─────────────────────────────────────────────┤
│                    Domain                   │  Entities, Value Objects, Domain Events
├───────────────────────┬─────────────────────┤
│   Infrastructure/Write│ Infrastructure/Read │  Separate persistence stacks per side
└───────────────────────┴─────────────────────┘
```

- **Presentation** depends on **Application**
- **Application** depends on **Domain**
- **Infrastructure/Write** and **Infrastructure/Read** depend on **Domain** and **Application** respectively (via inward-defined interfaces)
- **Domain** depends on nothing
- **Infrastructure/Write** and **Infrastructure/Read** have no dependency on each other

---

## CQRS — Write Side

Commands mutate state and return a minimal result (id, status). They go through the domain aggregate.

| Artifact   | Naming convention     | Example                    | Defined in    |
| ---------- | --------------------- | -------------------------- | ------------- |
| Command    | `<Verb><Noun>Command` | `CreateContactCommand`     | `application` |
| Handler    | `<Verb><Noun>Handler` | `CreateContactHandler`     | `application` |
| Result     | `<Verb><Noun>Result`  | `CreateContactResult`      | `application` |
| Repository | `I<Noun>Repository`   | `IContactRepository`       | `domain`      |

**Repository** interface is defined in `domain/` and implemented in `infrastructure/write/`.
It works exclusively with domain entities — never DTOs.

---

## CQRS — Read Side

Queries are read-only and return DTOs. They bypass the domain model entirely.

| Artifact   | Naming convention   | Example                    | Defined in    |
| ---------- | ------------------- | -------------------------- | ------------- |
| Query      | `Get<Noun>Query`    | `GetContactQuery`          | `application` |
| Handler    | `Get<Noun>Handler`  | `GetContactHandler`        | `application` |
| DTO        | `<Noun>Dto`         | `ContactDto`               | `application` |
| Projector  | `I<Noun>Projector`  | `IContactProjector`        | `application` |

**Projector** interface is defined in `application/` and implemented in `infrastructure/read/`.
It works exclusively with DTOs — never domain entities.

---

## Infrastructure Boundary

The write and read infrastructure are fully isolated from each other:

| Concern               | Infrastructure/Write              | Infrastructure/Read               |
| --------------------- | --------------------------------- | --------------------------------- |
| Implements            | `IContactRepository` (domain)     | `IContactProjector` (application) |
| Returns               | Domain entities                   | DTOs                              |
| Schema style          | Normalized (write model)          | Flat / denormalized (read model)  |
| Can use               | ORM, unit-of-work, transactions   | Raw SQL, read replica, cache      |
| Depends on other side | No                                | No                                |

---

## Solution Structure

```
AccountManager.sln
├── src/
│   ├── AccountManager.Common/                  ← Shared base types; no domain logic
│   │   ├── Domain/
│   │   │   ├── Entity.cs                       ← Abstract Entity<T> base class
│   │   │   └── AggregateRoot.cs                ← Abstract AggregateRoot<T> : Entity<T>
│   │   └── Errors/
│   │       └── Error.cs
│   │
│   ├── AccountManager.Domain/                  ← Aggregates, value objects, domain errors
│   │   ├── Administration/                     ← Administration Bounded Context
│   │   │   ├── ValueObjects/
│   │   │   │   ├── ContactId.cs
│   │   │   │   ├── ContactName.cs
│   │   │   │   └── ContactType.cs
│   │   │   ├── Errors/
│   │   │   │   ├── ContactNotFoundError.cs
│   │   │   │   ├── InvalidStatusTransitionError.cs
│   │   │   │   └── SelfActionForbiddenError.cs
│   │   │   ├── Repositories/
│   │   │   │   └── IContactRepository.cs
│   │   │   ├── Services/
│   │   │   │   ├── IActivateContactService.cs
│   │   │   │   ├── ActivateContactService.cs
│   │   │   │   ├── IDeleteContactService.cs
│   │   │   │   └── DeleteContactService.cs
│   │   │   └── Contact.cs
│   │   │
│   │   ├── SelfService/                        ← Self-service Bounded Context
│   │   │   ├── ValueObjects/
│   │   │   │   ├── ProviderId.cs
│   │   │   │   ├── ProviderAdminId.cs
│   │   │   │   ├── SystemAdminId.cs
│   │   │   │   ├── ProviderName.cs
│   │   │   │   └── Npi.cs
│   │   │   ├── Errors/
│   │   │   │   ├── InvalidNpiError.cs
│   │   │   │   └── InvalidProviderNameError.cs
│   │   │   ├── Repositories/
│   │   │   │   ├── IProviderRepository.cs
│   │   │   │   ├── IProviderAdminRepository.cs
│   │   │   │   └── ISystemAdminRepository.cs
│   │   │   ├── Services/
│   │   │   │   ├── IRegisterProviderService.cs
│   │   │   │   └── RegisterProviderService.cs
│   │   │   ├── Provider.cs
│   │   │   ├── ProviderAdmin.cs
│   │   │   └── SystemAdmin.cs
│   │   │
│   │   └── Shared/                             ← Concepts shared across BCs
│   │       └── ValueObjects/
│   │           └── ContactStatus.cs
│   │
│   ├── AccountManager.Application/             ← deferred
│   ├── AccountManager.Infrastructure.Write/    ← deferred
│   ├── AccountManager.Infrastructure.Read/     ← deferred
│   └── AccountManager.Presentation/            ← deferred
│
└── tests/
    ├── AccountManager.Domain.Tests/
    │   ├── Administration/
    │   ├── SelfService/
    │   └── Shared/
    └── AccountManager.Application.Tests/       ← deferred
```

### Project dependencies (in scope for v1 domain work)

```
AccountManager.Domain  →  AccountManager.Common
AccountManager.Domain  →  CSharpFunctionalExtensions (NuGet)
AccountManager.Common  →  CSharpFunctionalExtensions (NuGet)
```

---

## DDD Conventions

| Concept           | Definition in this project                                               |
| ----------------- | ------------------------------------------------------------------------ |
| **Entity**        | Has identity (id); equality is by id; base class `Entity<T>` in `Common` |
| **Value Object**  | No identity; equality is by value; immutable; implemented as `record`    |
| **Aggregate**     | Cluster of entities/value objects with a single Aggregate Root; base class `AggregateRoot<T>` in `Common` |
| **Domain Event**  | Records something that happened in the domain (past tense); deferred     |
| **Repository**    | Interface in `domain/`; implementation in `infrastructure/write/`        |
| **Projector**     | Interface in `application/`; implementation in `infrastructure/read/`    |

---

## Bounded Contexts

| Context                | Responsibility                                                                 | Actors                          | Status        |
| ---------------------- | ------------------------------------------------------------------------------ | ------------------------------- | ------------- |
| **Self-service**       | Registration wizard, read own profile, update own profile                      | Provider, ProviderAdmin         | In scope (v1) |
| **Administration**     | Verify contacts, list all contacts, read/update/delete any contact             | SystemAdmin                     | In scope (v1) |
| **NPI Associations**   | Linking Contacts via NPI relationships                                         | —                               | Deferred      |
| **Authentication**     | Username/password, sessions, tokens                                            | —                               | Deferred      |

### Aggregates per Bounded Context

| Bounded Context    | Aggregates                        | Rationale                                                                 |
| ------------------ | --------------------------------- | ------------------------------------------------------------------------- |
| **Self-service**   | `Provider`, `ProviderAdmin`, `SystemAdmin` | Type-specific: registration flow, fields, and invariants differ by type   |
| **Administration** | `Contact`                         | Type-agnostic: verify/delete/list operate the same regardless of type     |

### Operations per Bounded Context

**Administration BC** — operates on `Contact` aggregate:

| Command / Query    | Description                                  |
| ------------------ | -------------------------------------------- |
| `VerifyContact`    | Transitions `Pending` → `Active`             |
| `DeleteContact`    | Removes the record                           |
| `GetContact`       | Returns a single `ContactDto`                |
| `ListContacts`     | Returns all contacts as `ContactSummaryDto`  |

> `CreateContact` and `UpdateContact` in the Administration BC are **deferred to a future phase**. In v1, all contacts enter the system via the Self-service registration wizard.

**Self-service BC** — operates on `Provider` / `ProviderAdmin` aggregates:

| Command / Query    | Description                                  |
| ------------------ | -------------------------------------------- |
| `RegisterProvider`      | Creates a `Provider` with status `Pending`          |
| `RegisterProviderAdmin` | Creates a `ProviderAdmin` with status `Pending`     |
| `RegisterSystemAdmin`   | Creates a `SystemAdmin` with status `Pending`       |
| `ChangeProviderNpi`     | Replaces the `Npi` on a `Provider`                  |
| `ChangeName`            | Replaces the `ProviderName` on any Self-service aggregate — one command, type resolved at application layer |
| `GetAccount`            | Returns own record as `AccountDto`                  |

### Boundary rationale

The same underlying data ("a ProviderAdmin record") is modelled with different intent in each context:

- In **Self-service**, a `ProviderAdmin` is *"my account"* — the lifecycle state is invisible; operations are scoped to self; the type is central to the invariants.
- In **Administration**, any record is a `Contact` — lifecycle state is central; type is just an attribute; operations do not branch on type.

`Contact` is a **domain aggregate** in the Administration BC. It is a **technical base only** in the Self-service BC (shared id, status — not an aggregate you operate on directly).

---

## Domain Model

### Administration BC — `Contact` aggregate

| Field    | Type              | Notes                                         |
| -------- | ----------------- | --------------------------------------------- |
| `id`     | `ContactId`       | Value object wrapping the identifier          |
| `type`   | `ContactType`     | `Provider` \| `ProviderAdmin` \| `Organization` \| `SystemAdmin` |
| `status` | `ContactStatus`   | `Pending` \| `Active` \| `Deleted`            |
| `name`   | `ContactName`     | Value object — see below                      |

Domain methods: `Activate()` → `Result<Unit, Error>`, `Delete()` → `Result<Unit, Error>`

#### `ContactName` value object

Discriminated union — no nullable fields:

```
ContactName =
  | FullName         { firstName: string, lastName: string }
  | OrganizationName { name: string }
```

`FullName` is used for `Provider`, `ProviderAdmin`, `SystemAdmin`.
`OrganizationName` is used for `Organization`.

### Self-service BC — `Provider` aggregate

| Field    | Type            | Notes                                                                               |
| -------- | --------------- | ----------------------------------------------------------------------------------- |
| `id`     | `ProviderId`    | Typed UUID wrapper; generated by the aggregate at construction; not assigned by DB  |
| `name`   | `ProviderName`  | Value object — see below                                                            |
| `npi`    | `Npi`           | Value object; required; exactly one                                                 |
| `status` | `ContactStatus` | `Pending` \| `Active`; **read-only** in this BC — only mutated by Administration BC |

Domain methods: `changeName(name: ProviderName)`, `changeNpi(npi: Npi)`

### Self-service BC — `ProviderAdmin` aggregate

| Field    | Type              | Notes                                                                               |
| -------- | ----------------- | ----------------------------------------------------------------------------------- |
| `id`     | `ProviderAdminId` | Typed UUID wrapper; generated by the aggregate at construction; not assigned by DB  |
| `name`   | `ProviderName`    | Value object — see below                                                            |
| `status` | `ContactStatus`   | `Pending` \| `Active`; **read-only** in this BC — only mutated by Administration BC |

Domain methods: `changeName(name: ProviderName)`

### Self-service BC — `SystemAdmin` aggregate

| Field    | Type            | Notes                                                                               |
| -------- | --------------- | ----------------------------------------------------------------------------------- |
| `id`     | `SystemAdminId` | Typed UUID wrapper; generated by the aggregate at construction; not assigned by DB  |
| `name`   | `ProviderName`  | Value object — see below                                                            |
| `status` | `ContactStatus` | `Pending` \| `Active`; **read-only** in this BC — only mutated by Administration BC |

Domain methods: `changeName(name: ProviderName)`

---

### Value Objects

| Value Object      | Invariants                                                                    |
| ----------------- | ----------------------------------------------------------------------------- |
| `ProviderId`      | Typed UUID wrapper; no validation (internally generated, trusted by design)   |
| `ProviderAdminId` | Typed UUID wrapper; no validation (internally generated, trusted by design)   |
| `SystemAdminId`   | Typed UUID wrapper; no validation (internally generated, trusted by design)   |
| `ContactId`       | Typed UUID wrapper; no validation (Administration BC counterpart)             |
| `ProviderName`    | `{ firstName, lastName }` — letters only, max 50 chars each, non-empty, trimmed |
| `Npi`             | Exactly 10 numeric digits                                                     |
| `ContactStatus`   | `Pending \| Active \| Deleted`; valid transitions: `Pending → Active` via `Activate()`, `Pending/Active → Deleted` via `Delete()` |
| `ContactName`     | Discriminated union: `FullName { firstName, lastName }` \| `OrganizationName { name }` |

> `ProviderId`, `ProviderAdminId`, `SystemAdminId`, and `ContactId` are distinct types per Bounded Context. They share the same UUID value at runtime but are never interchangeable in the type system. Cross-BC translation happens in domain event handlers.

> `ProviderName` is reused across `Provider`, `ProviderAdmin`, and `SystemAdmin` — identical invariants, same real-world concept (a person's name).

> `OrganizationName` invariants are deferred — no Create Organization operation in v1.

---

### Domain Services — General Convention

All domain services have a corresponding interface, prefixed with `I`, defined in the same layer:

| Concrete                   | Interface                    | Reason                                      |
| -------------------------- | ---------------------------- | ------------------------------------------- |
| `RegisterProviderService`  | `IRegisterProviderService`   | DI registration; test doubles at boundaries |
| `ActivateContactService`   | `IActivateContactService`    | DI registration; test doubles at boundaries |
| `DeleteContactService`     | `IDeleteContactService`      | DI registration; test doubles at boundaries |

> Interfaces live alongside their implementations in `domain/`. Application-layer handlers depend on the interface, not the concrete class.

---

### Self-service BC — Domain Services

| Service                   | Interface                   | Responsibility                                                                                  |
| ------------------------- | --------------------------- | ----------------------------------------------------------------------------------------------- |
| `RegisterProviderService` | `IRegisterProviderService`  | Receives already-validated `ProviderName` and `Npi` value objects; delegates to `Provider.Register`; returns `Result<Provider, Error>` |

---

### Administration BC — Domain Services

| Service                  | Interface                   | Responsibility                                                                |
| ------------------------ | --------------------------- | ----------------------------------------------------------------------------- |
| `ActivateContactService` | `IActivateContactService`   | Checks eligibility, delegates to `Contact.Activate()`, returns `Result`       |
| `DeleteContactService`   | `IDeleteContactService`     | Checks eligibility, delegates to `Contact.Delete()`, returns `Result`         |

**Eligibility invariant:** a `SystemAdmin` cannot act on their own `Contact` record. Enforced by comparing `ContactId` (actor) against `ContactId` (target) using record equality. The Application layer is responsible for resolving the logged-in user's `SystemAdminId` to their `ContactId` before invoking domain services.

> The application-layer command is `VerifyContact` (user intent). The domain service is `ActivateContactService` (domain effect). The `VerifyContactHandler` calls `ActivateContactService`.

---

## Open Questions

- Transport layer (REST, gRPC, other)
- Persistence (ORM, raw SQL, document store)
- Command/Query bus: library vs hand-rolled
- Application framework (ASP.NET Core, minimal API, other)
