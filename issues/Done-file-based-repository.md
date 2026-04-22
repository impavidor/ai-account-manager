# File-based repository implementation (Infrastructure prototype)

## Problem Statement

The domain layer has four repository interfaces (`IContactRepository`, `IProviderRepository`, `IProviderAdminRepository`, `ISystemAdminRepository`) with no concrete implementations. There is no way to run the application end-to-end or prototype workflows without a persistence layer in place. A real database is not yet warranted at this stage.

## Solution

Introduce a file-based persistence implementation inside a new `AccountManager.Infrastructure` project. Each aggregate type is stored as a JSON array in a dedicated file (one file per aggregate type, mirroring a SQL table). The implementation is intentionally simple and not optimised for performance — it exists to unblock end-to-end prototyping with a small number of records.

## User Stories

1. As a developer, I want a concrete `IContactRepository` implementation, so that I can run contact-related workflows end-to-end without a database.
2. As a developer, I want a concrete `IProviderRepository` implementation, so that I can run provider registration workflows end-to-end.
3. As a developer, I want a concrete `IProviderAdminRepository` implementation, so that provider admin workflows can be exercised end-to-end.
4. As a developer, I want a concrete `ISystemAdminRepository` implementation, so that system admin workflows can be exercised end-to-end.
5. As a developer, I want all repository implementations to read and write from configurable file paths, so that I can point the data directory at any location without recompiling.
6. As a developer, I want the base data directory to be injected via a shared options object, so that I configure it once and all repositories pick it up.
7. As a developer, I want the repositories to fail fast when the data directory or file does not exist, so that misconfiguration is detected immediately rather than silently producing empty results.
8. As a developer, I want domain aggregates to be reconstituted from persisted data without re-running validation, so that loading is reliable even if business rules change after initial writes.
9. As a developer, I want serialization concerns to be fully contained within the Infrastructure project, so that domain objects remain unaware of persistence formats.
10. As a developer, I want all file I/O and JSON parsing logic encapsulated in a single reusable component, so that each repository implementation stays thin and the complex logic can be tested in isolation.
11. As a developer, I want `Contact` to be serialized with nullable `FirstName`, `LastName`, and `OrgName` fields, so that both `FullName` and `OrganizationName` contact name variants are handled without a custom JSON converter.
12. As a developer, I want `ContactType` to act as the discriminator for `ContactName` deserialization, so that the correct name subtype is reconstructed on load without an extra type field.
13. As a developer, I want the Infrastructure project to reference `AccountManager.Domain` via a project reference, so that it implements the domain-defined repository interfaces without creating circular dependencies.

## Implementation Decisions

### Modules

**New project: `AccountManager.Infrastructure`**

- **`FileRepositoryOptions`** — a configuration record with a single `BasePath` property. Injected into all four repository implementations. Registered once in DI.

- **`JsonFileStore<TDto>`** — deep module. Encapsulates all file I/O and JSON serialization for a single JSON array file. Interface: load all DTOs from file, save a new list of DTOs to file. Throws on missing file or directory. All JSON parsing, file reading, and file writing logic lives here and nowhere else.

- **Serialization DTOs** — plain POCOs: `ContactDto`, `ProviderDto`, `ProviderAdminDto`, `SystemAdminDto`. Live in Infrastructure. Never cross into the Domain layer.

- **`FileContactRepository`** — implements `IContactRepository`. Uses `JsonFileStore<ContactDto>`. Maps between `ContactDto` and `Contact` via the `internal` constructor.

- **`FileProviderRepository`** — implements `IProviderRepository`. Same pattern.

- **`FileProviderAdminRepository`** — implements `IProviderAdminRepository`. Same pattern.

- **`FileSystemAdminRepository`** — implements `ISystemAdminRepository`. Same pattern.

**Modified: `AccountManager.Domain`**

- Add `internal` constructors to `Contact`, `Provider`, `ProviderAdmin`, and `SystemAdmin`. These constructors accept raw primitives and do not re-run validation. They are used exclusively by the Infrastructure mapping layer.
- Add `[assembly: InternalsVisibleTo("AccountManager.Infrastructure")]` so that the Infrastructure project can call the `internal` constructors.

### Architectural decisions

- Domain objects are never passed into Infrastructure DTOs. Infrastructure reads primitives out of DTOs and passes them into `internal` domain constructors.
- `ContactName` serialization: `ContactDto` carries nullable `FirstName`, `LastName`, and `OrgName` fields. On load, `ContactType` determines which `ContactName` subtype to construct (`Organization` → `OrganizationName` using `OrgName`; all others → `FullName` using `FirstName` + `LastName`).
- File layout: one JSON file per aggregate type (`contacts.json`, `providers.json`, `provideradmins.json`, `systemadmins.json`) under `BasePath`. Mirrors a SQL table per aggregate.
- Missing file/directory: throw immediately. The Application layer is responsible for initialising the data directory before use.
- JSON library: `System.Text.Json` (built into .NET 9, no additional dependency).

## Testing Decisions

**What makes a good test:** tests should verify externally observable behaviour only — what comes out given what goes in — not implementation details like which methods were called or how the file was structured internally.

**Modules to test:** `JsonFileStore<TDto>` only.

**What to test:**
- Loading from a file that contains a valid JSON array returns the expected DTOs.
- Loading from a file that does not exist throws.
- Loading from a file that is empty or malformed throws or returns a meaningful error.
- Saving a list of DTOs writes valid JSON that can be round-tripped back via a subsequent load.
- Saving to a path whose directory does not exist throws.

**Prior art:** existing service tests in `AccountManager.Domain.Tests` (e.g., `ActivateContactServiceTests`, `RegisterProviderServiceTests`) demonstrate the NUnit + FluentAssertions pattern used throughout the test suite. `JsonFileStore` tests should follow the same style: `[SetUp]` for test scaffolding, `[Test]` per case, FluentAssertions for assertions.

The four repository implementations are thin wrappers and do not require unit tests. End-to-end testing of the repositories is integration territory and is out of scope here.

## Out of Scope

- Application layer initialisation of the data directory.
- Dependency injection registration / wiring up the Infrastructure project in a host.
- Read-side (query/projection) infrastructure.
- Concurrency safety or file locking.
- Performance optimisation of any kind.
- Migration tooling or versioning of the JSON format.
- Any repository beyond the four already defined in the Domain layer.

## Further Notes

This implementation is explicitly a prototype. The file-per-aggregate-type layout was chosen over file-per-instance precisely because it maps more directly to a SQL table, reducing conceptual distance when migrating to a real database later. The `JsonFileStore<TDto>` abstraction is the only component with long-term value; the rest is throwaway scaffolding.
