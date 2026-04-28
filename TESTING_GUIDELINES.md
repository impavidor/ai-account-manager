# Testing Guidelines

## Test Layers

This project uses three distinct test layers. Each layer owns a specific slice of confidence — do not duplicate coverage across layers.

| Layer | Project | What it verifies |
|-------|---------|-----------------|
| **Unit** | `Domain.Tests`, `Application.Tests`, `Infrastructure.Tests` | Business logic, domain rules, input permutations, edge cases |
| **Integration** | `IntegrationTests` *(planned)* | HTTP pipeline wiring: routing, middleware, result mapping, auth enforcement, persistence |
| **Functional** | *(reserved — requires UI)* | End-to-end user journeys through the full system including the frontend |

Trust unit tests. Integration tests do not re-test logic — they verify that the layers are correctly connected.

---

## Integration Test Strategy

Integration tests boot the real application via `WebApplicationFactory<Program>` and send HTTP requests against it.

### Four-scenario rule

For each feature area, cover exactly these four scenarios:

1. **Happy path** — the golden path succeeds end-to-end (may be more than one if distinct flows exist)
2. **Domain error** — one representative case confirming `ResultMapper` maps domain errors to the correct HTTP status
3. **API error** — one case confirming model binding returns `400` for a malformed request
4. **Auth** — missing or wrong-role actor headers are rejected

Do not add more scenarios at this layer. Edge cases and input permutations belong in unit tests.

### Infrastructure conventions

- **Shared instance per fixture:** One `WebApplicationFactory` and `HttpClient` per `[TestFixture]` class, created in `[OneTimeSetUp]`, torn down in `[OneTimeTearDown]`. No isolation between tests within a fixture — they run sequentially and share state.
- **No shared types:** The integration test project must not import DTOs, enums, or any other types from `Application`, `Domain`, or `Infrastructure`. Define local record types for any data shapes needed in tests.
- **Auth via headers:** Use the `X-Actor-Id` / `X-Actor-Type` headers consumed by `FakeAuthMiddleware`. A helper extension method on `HttpRequestMessage` stamps these on each request.

### State seeding

Prefer seeding state through HTTP calls (if the relevant endpoint exists). When an endpoint does not exist yet, seed by writing JSON files directly to the temp data directory.

---

## What belongs where — quick reference

| Scenario type | Unit | Integration |
|---------------|------|-------------|
| Domain rule (e.g. invalid NPI format) | Yes — exhaustive | No — one representative case only |
| Status transition edge cases | Yes | No |
| ResultMapper wiring | No | Yes — one per error type |
| Auth enforcement | No | Yes |
| Controller routing | No | Yes |
| Happy path flow | Implicit in handler tests | Yes |
