using AccountManager.Domain.Administration;
using AccountManager.Domain.Shared;

namespace AccountManager.Application.SelfService;

public record AccountDto(Guid Id, ContactType Type, ContactStatus Status, string FirstName, string LastName, string? Npi);
