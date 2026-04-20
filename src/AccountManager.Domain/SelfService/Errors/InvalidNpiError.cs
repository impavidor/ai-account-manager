using AccountManager.Common.Errors;

namespace AccountManager.Domain.SelfService;

public record InvalidNpiError(string Value) : Error($"'{Value}' is not a valid NPI");
