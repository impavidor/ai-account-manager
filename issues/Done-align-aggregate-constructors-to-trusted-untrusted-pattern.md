# Align Aggregate Constructors to Trusted/Untrusted Pattern

## Status

Open

## What to build

Refactor all four aggregates (`Contact`, `Provider`, `ProviderAdmin`, `SystemAdmin`) so their constructors conform to the pattern defined in `IMPLEMENTATION_GUIDELINES.md`:

- Each aggregate has **exactly one `internal` constructor**: pure assignment, fully-typed arguments (typed ID, VOs, enums), no transformation logic.
- The redundant `private` constructor on each aggregate is deleted.
- Static factory methods own all transformation: construct the typed ID from `Guid.NewGuid()` and pass `ContactStatus.Pending` explicitly.
- Repositories own their transformation: construct the typed ID from `dto.Id` at the call site before passing it to the internal constructor.

## Acceptance criteria

- [ ] `Contact` has one `internal` constructor: `(ContactId id, ContactType type, ContactStatus status, ContactName name)` — pure assignment, `private` constructor deleted
- [ ] `Provider` has one `internal` constructor: `(ProviderId id, ProviderName name, Npi npi, ContactStatus status)` — pure assignment, `private` constructor deleted
- [ ] `ProviderAdmin` has one `internal` constructor: `(ProviderAdminId id, ProviderName name, ContactStatus status)` — pure assignment, `private` constructor deleted
- [ ] `SystemAdmin` has one `internal` constructor: `(SystemAdminId id, ProviderName name, ContactStatus status)` — pure assignment, `private` constructor deleted
- [ ] Each factory method constructs its typed ID (`new XxxId(Guid.NewGuid())`) and passes `ContactStatus.Pending` as an explicit argument
- [ ] `FileContactRepository`, `FileProviderRepository`, `FileProviderAdminRepository`, `FileSystemAdminRepository` construct the typed ID at the call site (`new XxxId(dto.Id)`) before calling the internal constructor
- [ ] Solution builds with no errors
- [ ] All existing tests pass unchanged

## Blocked by

None — can start immediately.

## Notes

The `private` constructors were introduced as a stepping-stone: the factory created a typed ID and passed it to the private constructor, which then hardcoded `ContactStatus.Pending`. The internal constructors (taking raw `Guid`) existed for the repositories and wrapped the Guid internally. Both are wrong under the updated guidelines:

- Transformation inside the constructor violates pure assignment.
- Hardcoding initial state in a private constructor hides the domain invariant from the factory.

Value objects (`ProviderName`, `Npi`) already have `internal` constructors with pure assignment — no changes needed there.
