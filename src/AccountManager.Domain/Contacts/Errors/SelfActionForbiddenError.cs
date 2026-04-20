using AccountManager.Common.Errors;

namespace AccountManager.Domain.Contacts.Errors;

public record SelfActionForbiddenError() : Error("A SystemAdmin cannot act on their own Contact record");
