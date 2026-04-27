using AccountManager.Common.Errors;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class ChangeProviderNpiCommand
{
    public Npi Npi { get; }

    private ChangeProviderNpiCommand(Npi npi) => Npi = npi;

    public static Result<ChangeProviderNpiCommand, Error> Create(string npi)
    {
        var npiResult = Npi.Create(npi);
        if (npiResult.IsFailure) return npiResult.ConvertFailure<ChangeProviderNpiCommand>();
        return new ChangeProviderNpiCommand(npiResult.Value);
    }
}
