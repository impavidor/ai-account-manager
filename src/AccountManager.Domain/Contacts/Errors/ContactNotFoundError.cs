using AccountManager.Common.Errors;
using AccountManager.Domain.Contacts.ValueObjects;

namespace AccountManager.Domain.Contacts.Errors;

public record ContactNotFoundError(ContactId Id) : Error($"Contact '{Id.Value}' was not found");
