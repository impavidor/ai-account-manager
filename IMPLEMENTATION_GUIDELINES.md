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

## Aggregate Constructor Convention

**Aggregate constructors are always `private`.**

Creation is always mediated by a static factory method (see below). Infrastructure projects that need to reconstitute aggregates from storage access the private constructor directly via `InternalsVisibleTo` — a deliberate concession to the data layer. The factory enforces domain invariants at creation time; the infrastructure layer is trusted to reconstruct valid state from a trusted data store.

```csharp
// Correct
public class Provider : AggregateRoot<ProviderId>
{
    private Provider(ProviderId id, ProviderName name, Npi npi) : base(id) { ... }

    public static Result<Provider, Error> Register(ProviderName name, Npi npi) => ...
}

// Wrong — public constructor bypasses domain invariants
public class Provider : AggregateRoot<ProviderId>
{
    public Provider(ProviderId id, ProviderName name, Npi npi) { ... }
}
```

**`InternalsVisibleTo` declarations** for infrastructure access are placed in `AccountManager.Domain`:

```csharp
[assembly: InternalsVisibleTo("AccountManager.Infrastructure.Write")]
```

---

## Aggregate Factory Method Convention

Every aggregate exposes one or more static factory methods as its sole public creation path. Factories:

1. **Always return `Result<TAggregate, Error>`** — even when no validation is currently required. This is a forward-compatibility contract: future guard conditions can be added without changing call-site signatures.
2. **Use domain-meaningful names** — prefer names that reflect the domain operation rather than a generic `Create`. Examples: `Register`, `Open`, `Issue`.
3. **Generate the aggregate ID internally** — callers never supply an ID to a factory. ID generation is the factory's responsibility.
4. **Set the initial lifecycle state** — the factory fixes the starting `ContactStatus` (always `Pending` for v1 aggregates).

```csharp
// Correct — domain-meaningful name, returns Result, generates ID
public static Result<Provider, Error> Register(ProviderName name, Npi npi) =>
    Result.Success<Provider, Error>(new Provider(new ProviderId(Guid.NewGuid()), name, npi));

// Wrong — generic name
public static Result<Provider, Error> Create(ProviderName name, Npi npi) => ...

// Wrong — returns entity directly instead of Result
public static Provider Register(ProviderName name, Npi npi) => ...

// Wrong — caller supplies the ID
public static Result<Provider, Error> Register(ProviderId id, ProviderName name, Npi npi) => ...
```

---

## Value Object Convention

Value objects are `record` types with a private constructor and a static `Create` factory returning `Result<TValueObject, Error>`. Validation lives entirely in the factory.

```csharp
public record Npi
{
    public string Value { get; }
    private Npi(string value) => Value = value;
    public static Result<Npi, Error> Create(string value) => ...
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
