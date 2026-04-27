using AccountManager.Domain.Administration;
using AccountManager.Domain.Shared;

namespace AccountManager.Application.Administration;

public record ContactSummaryDto(Guid Id, ContactType Type, ContactStatus Status, string FirstName, string LastName);
