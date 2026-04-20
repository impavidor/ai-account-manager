using System.Text.RegularExpressions;
using AccountManager.Common.Errors;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.SelfService;

public record ProviderName
{
    public string FirstName { get; }
    public string LastName { get; }

    private ProviderName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<ProviderName, Error> Create(string firstName, string lastName)
    {
        var first = (firstName ?? "").Trim();
        var last = (lastName ?? "").Trim();

        if (!IsValid(first))
            return Result.Failure<ProviderName, Error>(new InvalidProviderNameError("first name", first));

        if (!IsValid(last))
            return Result.Failure<ProviderName, Error>(new InvalidProviderNameError("last name", last));

        return Result.Success<ProviderName, Error>(new ProviderName(first, last));
    }

    private static bool IsValid(string name) =>
        name.Length > 0 && name.Length <= 50 && Regex.IsMatch(name, @"^[a-zA-Z]+$");
}
