# Bounded Context Folder Separation

## Problem Statement

All domain code lives under a single `Contacts/` folder inside `AccountManager.Domain`, mixing both the Administration BC and the Self-service BC. There is no structural enforcement of BC boundaries, making it easy to introduce accidental cross-BC dependencies as the domain grows.

Additionally, `ActivateContactService` and `DeleteContactService` take a `SystemAdminId` parameter (a Self-service BC type) to identify the acting user, creating an explicit cross-BC type dependency in the Administration BC.

## Solution

Reorganise `AccountManager.Domain` into three top-level folders — `Administration/`, `SelfService/`, and `Shared/` — each containing the aggregates, value objects, errors, repositories, and services that belong to that context. Fix the cross-BC dependency by replacing `SystemAdminId` with `ContactId` in the Administration BC services.

## Commits

1. **Create `Shared/ValueObjects/ContactStatus.cs`** — move `ContactStatus` enum to the shared folder and update its namespace to `AccountManager.Domain.Shared`. Fix all `using` statements across the domain and test projects. All tests must pass.

2. **Create `Administration/` structure** — move `Contact.cs`, `ContactId`, `ContactName`, `ContactType`, all Administration errors (`InvalidStatusTransitionError`, `ContactNotFoundError`, `SelfActionForbiddenError`), `IContactRepository`, `IActivateContactService`, `ActivateContactService`, `IDeleteContactService`, `DeleteContactService` to `Administration/` subfolders. Update namespaces to `AccountManager.Domain.Administration`. Fix all `using` statements. All tests must pass.

3. **Create `SelfService/` structure** — move `Provider.cs`, `ProviderAdmin.cs`, `SystemAdmin.cs`, `ProviderId`, `ProviderAdminId`, `SystemAdminId`, `ProviderName`, `Npi`, `InvalidNpiError`, `InvalidProviderNameError`, `IProviderRepository`, `IProviderAdminRepository`, `ISystemAdminRepository`, `IRegisterProviderService`, `RegisterProviderService` to `SelfService/` subfolders. Update namespaces to `AccountManager.Domain.SelfService`. Fix all `using` statements. All tests must pass.

4. **Remove the old `Contacts/` folder** — delete the now-empty folder. Verify the solution builds and all tests pass.

5. **Replace `SystemAdminId` with `ContactId` in Administration services** — change `IActivateContactService.ActivateAsync` and `IDeleteContactService.DeleteAsync` signatures to take `ContactId actorId` instead of `SystemAdminId actorId`. Update the concrete implementations and their self-action guards (`contactId == actorId` using record equality, removing the cross-type `.Value` comparison). Update all call sites in tests.

6. **Mirror folder structure in test project** — reorganise `AccountManager.Domain.Tests` to match: `Administration/`, `SelfService/`, and their subfolders. Update namespaces. All tests must pass.

## Decision Document

- **Three folders**: `Administration/`, `SelfService/`, `Shared/` inside `AccountManager.Domain`. No separate projects at this stage — the BC boundary is structural/namespace only, not a compile-time project reference boundary.
- **Shared folder**: Only `ContactStatus` moves here for now. The `Error` base type remains in `AccountManager.Common`. Each BC owns its own error subtypes.
- **ContactName**: Stays in `Administration/` — it is only used by the `Contact` aggregate.
- **Actor identity in Administration services**: `SystemAdminId` is replaced with `ContactId`. The Application layer (not yet built) is responsible for resolving the logged-in user's `SystemAdminId` to their `ContactId` before invoking domain services.
- **Self-action guard**: Changes from `contactId.Value == actorId.Value` (cross-type Guid comparison) to `contactId == actorId` (record structural equality — both are `ContactId`).
- **Namespace convention**: `AccountManager.Domain.Administration`, `AccountManager.Domain.SelfService`, `AccountManager.Domain.Shared`.

## Testing Decisions

- Tests are pure unit/integration tests with no mocking framework; file-scoped in-memory repository fakes are defined per test class.
- Each refactoring commit must leave all existing tests green — no new test logic is introduced during this refactor.
- Test files mirror the production folder structure exactly.

## Out of Scope

- Splitting into separate C# projects (`AccountManager.Domain.Administration`, `AccountManager.Domain.SelfService`). Deferred until the Application layer exists and cross-BC communication patterns are clear.
- Any new domain behaviour, aggregates, or services.
- Changes to `AccountManager.Common`.

