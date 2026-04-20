using AccountManager.Common.Errors;

namespace AccountManager.Domain.Administration;

public record SelfActionForbiddenError() : Error("A SystemAdmin cannot act on their own Contact record");
