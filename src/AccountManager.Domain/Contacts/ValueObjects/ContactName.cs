namespace AccountManager.Domain.Contacts.ValueObjects;

public abstract record ContactName;

public sealed record FullName(string FirstName, string LastName) : ContactName;

public sealed record OrganizationName(string Name) : ContactName;
