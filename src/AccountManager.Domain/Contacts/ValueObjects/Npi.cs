using System.Text.RegularExpressions;
using AccountManager.Common.Errors;
using AccountManager.Domain.Contacts.Errors;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.Contacts.ValueObjects;

public record Npi
{
    public string Value { get; }

    private Npi(string value) => Value = value;

    public static Result<Npi, Error> Create(string value)
    {
        if (!Regex.IsMatch(value ?? "", @"^\d{10}$"))
            return Result.Failure<Npi, Error>(new InvalidNpiError(value ?? ""));

        return Result.Success<Npi, Error>(new Npi(value!));
    }
}
