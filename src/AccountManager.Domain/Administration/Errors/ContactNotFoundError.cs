using AccountManager.Common.Errors;

namespace AccountManager.Domain.Administration;

public record ContactNotFoundError(ContactId Id) : Error($"Contact '{Id.Value}' was not found");
