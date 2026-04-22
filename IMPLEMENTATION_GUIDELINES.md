# Implementation Guidelines

Coding conventions and patterns that must be followed consistently across the domain. These complement the architectural decisions in `DESIGN_DECISIONS.md`.

---

## Aggregate and Entity Visibility

| Concept          | Visibility   | Reason                                                                 |
| ---------------- | ------------ | ---------------------------------------------------------------------- |
| **Aggregate**    | `public`     | Aggregates are the primary API of the domain; consumed by services and application layer |
| **Entity**       | `internal`   | Inner entities are implementation details; not directly consumed outside the domain project |

If a test project needs access to an internal entity, use `[assembly: InternalsVisibleTo("...Tests")]` — do not promote visibility to `public`.

---

## Trusted vs Untrusted Sources

Domain objects distinguish between **trusted** and **untrusted** callers:

| Source | Examples |
| ------ | -------- |
| **Trusted** | The domain object itself, static factory methods within the same class, repositories reconstituting from DB, other code within the domain assembly |
| **Untrusted** | Application services, API controllers, external integrations — anything outside the domain assembly |

The trust boundary is the **domain assembly**. The C# `internal` keyword enforces it: trusted callers in the same assembly get direct access; trusted callers in a different assembly (e.g. infrastructure) are explicitly granted access via `InternalsVisibleTo`.

Trusted callers can create domain objects directly without going through validation — data originating from DB or from within the domain is already known-good. Untrusted callers must go through a static factory method that validates before constructing.

`InternalsVisibleTo` declarations granting infrastructure access are placed in `AccountManager.Domain`:

```csharp
[assembly: InternalsVisibleTo("AccountManager.Infrastructure")]
```

---

## Aggregate Constructor Convention

**Every aggregate has exactly one `internal` constructor.**

The constructor is the single trusted entry point for assembling an aggregate. It must be **pure assignment only** — no validation, no transformation logic. It accepts already-typed arguments: strongly-typed IDs, value objects, enums. It never accepts raw primitives (e.g. `Guid`, `string`) and never constructs value objects internally. Callers are responsible for preparing the correct typed values before calling it.

```csharp
// Correct — internal, pure assignment, fully-typed arguments
public class Provider : AggregateRoot<ProviderId>
{
    internal Provider(ProviderId id, ProviderName name, Npi npi, ContactStatus status) : base(id)
    {
        Name = name;
        Npi = npi;
        Status = status;
    }
}

// Wrong — public constructor bypasses the trust boundary
public Provider(ProviderId id, ProviderName name, Npi npi, ContactStatus status) { ... }

// Wrong — transformation logic inside the constructor
internal Provider(Guid id, ProviderName name, Npi npi, ContactStatus status)
    : base(new ProviderId(id)) { ... }

// Wrong — two constructors (one for DB, one for factory) — redundant
private Provider(ProviderId id, ProviderName name, Npi npi) : base(id) { ... }
internal Provider(Guid id, ProviderName name, Npi npi, ContactStatus status) : base(new ProviderId(id)) { ... }
```

---

## Aggregate Factory Method Convention

Static factory methods are the **untrusted entry point** for aggregate creation. Every aggregate exposes one or more. Factories:

1. **Always return `Result<TAggregate, Error>`** — even when no validation is currently required. This is a forward-compatibility contract: future guard conditions can be added without changing call-site signatures.
2. **Use domain-meaningful names** — prefer names that reflect the domain operation rather than a generic `Create`. Examples: `Register`, `Open`, `Issue`.
3. **Generate the aggregate ID internally** — callers never supply an ID to a factory. ID generation is the factory's responsibility.
4. **Set the initial lifecycle state explicitly** — the factory passes the starting `ContactStatus` (always `Pending` for v1 aggregates) as an explicit argument to the internal constructor. Initial state is never hardcoded inside a private constructor.
5. **Own all transformation** — the factory constructs typed IDs and value objects from whatever inputs it receives before calling the internal constructor. The constructor receives only already-typed values.

```csharp
// Correct — domain-meaningful name, returns Result, generates ID, sets initial state explicitly,
// constructs typed ID before calling internal constructor
public static Result<Provider, Error> Register(ProviderName name, Npi npi) =>
    Result.Success<Provider, Error>(new Provider(new ProviderId(Guid.NewGuid()), name, npi, ContactStatus.Pending));

// Wrong — generic name
public static Result<Provider, Error> Create(ProviderName name, Npi npi) => ...

// Wrong — returns entity directly instead of Result
public static Provider Register(ProviderName name, Npi npi) => ...

// Wrong — caller supplies the ID
public static Result<Provider, Error> Register(ProviderId id, ProviderName name, Npi npi) => ...
```

---

## Value Object Convention

Value objects are `record` types with an `internal` constructor and a static `Create` factory returning `Result<TValueObject, Error>`. The same trusted/untrusted split applies: the `internal` constructor is pure assignment for trusted callers; `Create` validates raw input for untrusted callers.

```csharp
public record Npi
{
    public string Value { get; }
    internal Npi(string value) => Value = value;   // trusted — pure assignment
    public static Result<Npi, Error> Create(string value) => ...   // untrusted — validates, then calls internal
}
```

`Create` is the standard name for value object factories (unlike aggregates, value objects have no domain-meaningful verb for their construction).

---

## Error Convention

- `Error` (abstract record) lives in `AccountManager.Common.Errors` — it is the only type in `Common` with domain semantics.
- Each specific error type lives alongside the code that produces it — in the same BC folder, under `Errors/`.
- Errors are `record` types inheriting `Error`. Constructor parameters encode the context needed to diagnose the failure.

```csharp
// In AccountManager.Domain.SelfService.Errors
public record InvalidNpiError(string Value) : Error($"'{Value}' is not a valid NPI");
```

---

## Domain Service Convention

Each domain service has a co-located interface prefixed with `I`:

- Interface and implementation live in the same folder under `Services/`.
- The interface is what the Application layer depends on (for DI and test doubles).
- Services receive already-validated value objects and domain entities as inputs — no primitive obsession at the service boundary.
- Services save the aggregate only on success.

```csharp
// Correct — save only on success
public async Task<UnitResult<Error>> ActivateAsync(ContactId contactId, ContactId actorId, CancellationToken ct)
{
    if (contactId == actorId)
        return UnitResult.Failure<Error>(new SelfActionForbiddenError());

    var contact = await _repository.GetByIdAsync(contactId, ct);
    if (contact is null)
        return UnitResult.Failure<Error>(new ContactNotFoundError(contactId));

    var result = contact.Activate();
    if (result.IsFailure) return result;

    await _repository.SaveAsync(contact, ct);
    return UnitResult.Success<Error>();
}
```

---

## Testing Convention

- No mocking framework in the domain test project.
- Repository fakes are file-scoped (`file` access modifier) in-memory implementations defined at the bottom of the test file that uses them.
- Tests cover external behavior only — state transitions, returned errors, persistence calls. Never test private methods or internal state directly.
- Test project mirrors the production folder structure exactly: `Administration/`, `SelfService/`, `Shared/`.
