# Ubiquitous Language

## Contact Types

### Administration BC

| Term                 | Definition                                                                                                                      | Aliases to avoid            |
| -------------------- | ------------------------------------------------------------------------------------------------------------------------------- | --------------------------- |
| **Contact**          | A managed entity in the system — the domain aggregate used by a **SystemAdmin** to verify, list, update, or delete any record. Type is an attribute, not a branching concern. | User, Account, Entity |

### Self-service BC

| Term                 | Definition                                                                                                                      | Aliases to avoid            |
| -------------------- | ------------------------------------------------------------------------------------------------------------------------------- | --------------------------- |
| **Provider**         | An individual who holds an NPI and delivers healthcare services, identified by first and last name. Domain aggregate in the Self-service BC. | Doctor, Clinician  |
| **ProviderAdmin**    | An individual who administers one or more **Providers**, identified by first and last name, holding no NPI directly. Domain aggregate in the Self-service BC. | Admin, Staff |

### Shared (both BCs)

| Term                 | Definition                                                                                                                      | Aliases to avoid            |
| -------------------- | ------------------------------------------------------------------------------------------------------------------------------- | --------------------------- |
| **Organization**     | A legal or institutional entity, identified by organization name and holding an NPI                                             | Company, Group, Employer    |
| **SystemAdmin**      | An operator of the system, identified by first and last name, holding no NPI, with permissions to manage all **Contacts**       | Admin, Superuser            |

## Identity & Associations

| Term                 | Definition                                                                                                      | Aliases to avoid        |
| -------------------- | --------------------------------------------------------------------------------------------------------------- | ----------------------- |
| **NPI**              | A National Provider Identifier — a unique 10-digit number assigned to healthcare providers and organizations    | ID, Code, Number        |
| **NPI Association**  | A link from a **Provider** or **ProviderAdmin** to a target NPI, expressing a working relationship              | Link, Mapping, Relation |

## Lifecycle

| State       | Definition                                                                 |
| ----------- | -------------------------------------------------------------------------- |
| **Pending** | A **Contact** that has registered but has not yet been verified            |
| **Active**  | A **Contact** that has been verified by a **SystemAdmin** and can be used  |
| **Deleted** | A **Contact** that has been soft-deleted by a **SystemAdmin**; record is retained but no longer operational |

## Operations

| Operation         | Who can perform it                                              | Scope                                      |
| ----------------- | --------------------------------------------------------------- | ------------------------------------------ |
| Create Contact    | **SystemAdmin** (all types); self-registration (**Provider**, **ProviderAdmin**) | New account starts as **Pending**     |
| Verify Contact    | **SystemAdmin**                                                 | Transitions **Pending** → **Active**       |
| Read (single)     | **Provider**, **ProviderAdmin** (own record); **SystemAdmin** (any) | —                                       |
| Read (list)       | **SystemAdmin** only                                            | All **Contacts**                           |
| Delete            | **SystemAdmin** only                                            | Any **Contact** — deferred to future phase: Update by SystemAdmin |

## Relationships

- A **Contact** is a **Provider**, a **ProviderAdmin**, a **SystemAdmin**, or an **Organization** — never more than one.
- A **Provider** holds exactly one **NPI**.
- An **Organization** holds exactly one **NPI**.
- A **ProviderAdmin** holds no **NPI** directly.
- A **SystemAdmin** holds no **NPI** directly.
- A **Provider** may have one or more **NPI Associations**, each pointing to an **Organization**'s NPI — meaning the **Provider** works for multiple **Organizations**.
- A **ProviderAdmin** may have one or more **NPI Associations**, each pointing to a **Provider**'s NPI — meaning the **ProviderAdmin** manages that **Provider**.

## Example dialogue

> **Dev:** "If a doctor works at two clinics, how do we model that?"
>
> **Domain expert:** "The doctor is a **Provider** with their own **NPI**. Each clinic is an **Organization** with its own **NPI**. You create two **NPI Associations** on the **Provider**, one pointing to each **Organization**'s NPI."
>
> **Dev:** "And what about the office manager who handles admin for that doctor?"
>
> **Domain expert:** "That's a **ProviderAdmin** — a **Contact** with first and last name but no NPI. They have an **NPI Association** pointing to the **Provider**'s NPI — that's how you know which **Provider** they administer."
>
> **Dev:** "So a **ProviderAdmin** could manage multiple **Providers**?"
>
> **Domain expert:** "Yes — one **NPI Association** per **Provider** they manage."

## Flagged ambiguities

- **"User" vs "Contact"**: "User" appeared early in the conversation as the subject of the account manager. In this domain, **Contact** is the correct term for the stored record. "User" is reserved for future authentication concepts and should not be used to mean a **Contact**.
- **"Account"**: Used colloquially to mean a **Contact** record. Avoid — it overlaps with future authentication/account management concepts.
- **"NPI"**: Used both as the identifier value (a string) and as a property of an entity ("has NPI"). Be explicit: an entity *holds an NPI* (property); an **NPI Association** *references a target NPI* (value).
