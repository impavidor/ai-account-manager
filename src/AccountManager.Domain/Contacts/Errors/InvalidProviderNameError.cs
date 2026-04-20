using AccountManager.Common.Errors;

namespace AccountManager.Domain.Contacts.Errors;

public record InvalidProviderNameError(string FieldName, string Value)
    : Error($"'{Value}' is not a valid {FieldName}");
