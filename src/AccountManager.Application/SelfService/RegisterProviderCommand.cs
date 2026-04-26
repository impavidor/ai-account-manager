using AccountManager.Common.Errors;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class RegisterProviderCommand
{
    public ProviderName Name { get; }
    public Npi Npi { get; }

    private RegisterProviderCommand(ProviderName name, Npi npi)
    {
        Name = name;
        Npi = npi;
    }

    public static Result<RegisterProviderCommand, Error> Create(string firstName, string lastName, string npi)
    {
        var nameResult = ProviderName.Create(firstName, lastName);
        if (nameResult.IsFailure) return nameResult.ConvertFailure<RegisterProviderCommand>();

        var npiResult = Npi.Create(npi);
        if (npiResult.IsFailure) return npiResult.ConvertFailure<RegisterProviderCommand>();

        return new RegisterProviderCommand(nameResult.Value, npiResult.Value);
    }
}
