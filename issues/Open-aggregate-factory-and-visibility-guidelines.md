# Aggregate Factory and Visibility Design Guidelines

## Problem Statement

The domain has inconsistent aggregate creation patterns: `Contact` exposes a `public` constructor used directly by infrastructure, while `Provider`, `ProviderAdmin`, and `SystemAdmin` have private constructors but their factories return the aggregate directly rather than `Result<T, Error>`. There is no enforced guideline distinguishing aggregate visibility from entity visibility, and no uniform factory interface across aggregates.

## Solution

Enforce three design guidelines across all aggregates:

1. **Entities are `internal`; Aggregates are `public`.** Internal entities are accessible to the test project via `InternalsVisibleTo` if needed.
2. **Aggregate constructors are always `private`.** Infrastructure projects access the private constructor via `InternalsVisibleTo` — a deliberate design concession to the data layer.
3. **Aggregate creation always goes through a static factory method returning `Result<T, Error>`.** Factory names are domain-meaningful (e.g. `Register`). Even when no validation is currently required, `Result` is returned to keep the interface uniform and accommodate future guard conditions without breaking call sites.

## Commits

1. **Add `InternalsVisibleTo` for `AccountManager.Infrastructure.Write`** — add `[assembly: InternalsVisibleTo("AccountManager.Infrastructure.Write")]` to `AccountManager.Domain`. This project does not yet exist but the attribute must be in place before infrastructure is scaffolded. Solution must build.

2. **Add `Contact.Register` factory and make constructor `private`** — add `static Result<Contact, Error> Register(ContactType type, ContactName name)` to `Contact`. The factory generates a new `ContactId` internally and sets `Status` to `ContactStatus.Pending`. Change the existing `public` constructor to `private`. Update all test setup code that currently instantiates `Contact` directly to use `Contact.Register(...)` instead, unwrapping the `Result`. All tests must pass.

3. **Change `Provider.Register` to return `Result<Provider, Error>`** — update the factory signature from `static Provider Register(ProviderName name, Npi npi)` to `static Result<Provider, Error> Register(ProviderName name, Npi npi)`. Update `RegisterProviderService` and its tests to unwrap the `Result`. All tests must pass.

4. **Change `ProviderAdmin.Register` to return `Result<ProviderAdmin, Error>`** — same pattern. Update call sites and tests. All tests must pass.

5. **Change `SystemAdmin.Register` to return `Result<SystemAdmin, Error>`** — same pattern. Update call sites and tests. All tests must pass.

## Decision Document

- **Factory naming**: Factory methods use domain-meaningful names. `Register` is preferred over `Create` where the domain operation is a registration act. Other contexts may use other names (e.g. `Open`, `Issue`) — the invariant is the return type, not the name.
- **`Result<T, Error>` always**: Factories always return `Result<T, Error>` even when they cannot currently fail. This is a forward-compatibility contract — future validation conditions can be added without changing call-site signatures.
- **Private constructor + `InternalsVisibleTo`**: Infrastructure projects reconstruct aggregates by calling the private constructor directly via `InternalsVisibleTo`, bypassing the factory. This is a conscious design concession: the factory enforces domain invariants at creation time, but the data layer is trusted to reconstruct valid state from storage.
- **Entity visibility**: No inner entities exist yet. The rule — entities are `internal`, aggregates are `public` — is established now as a forward guideline.
- **`InternalsVisibleTo` scope**: Only `AccountManager.Infrastructure.Write` receives access to private constructors. The test project does not need it — tests use factory methods for setup, and in-memory repos hold in-memory object references with no reconstitution needed.
- **`Contact.Register` parameters**: `(ContactType type, ContactName name)` — the factory generates the ID and fixes the initial status to `Pending`. The existing 4-parameter constructor signature (which accepted an external `ContactId` and `ContactStatus`) is for infrastructure reconstitution only and remains `private`.

## Testing Decisions

- No new tests are introduced — this is a structural refactor.
- Test setup code that currently calls `new Contact(...)` directly must be updated to call `Contact.Register(...).Value` (or equivalent unwrap) since the Result cannot fail in test setup with valid inputs.
- Existing service tests fully cover the factory-then-operate pattern: any regression will surface immediately.

## Out of Scope

- Renaming `Register` to a generic `Create` across all aggregates.
- Adding validation logic inside any factory — value objects remain responsible for their own invariants.
- `InternalsVisibleTo` for `AccountManager.Domain.Tests` — not needed with the current test strategy.
- `AggregateRoot<T>` base class (covered in a separate issue).

