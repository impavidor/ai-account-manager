using AccountManager.Common.Errors;

namespace AccountManager.Domain.Contacts.Errors;

public record InvalidNpiError(string Value) : Error($"'{Value}' is not a valid NPI");
