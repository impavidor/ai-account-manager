using AccountManager.Common.Errors;

namespace AccountManager.Domain.Administration;

public record InvalidStatusTransitionError(string From, string To)
    : Error($"Cannot transition from {From} to {To}");
