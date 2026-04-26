using AccountManager.Common.Errors;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class RegisterSystemAdminCommand
{
    public ProviderName Name { get; }

    private RegisterSystemAdminCommand(ProviderName name) => Name = name;

    public static Result<RegisterSystemAdminCommand, Error> Create(string firstName, string lastName)
    {
        var nameResult = ProviderName.Create(firstName, lastName);
        if (nameResult.IsFailure) return nameResult.ConvertFailure<RegisterSystemAdminCommand>();

        return new RegisterSystemAdminCommand(nameResult.Value);
    }
}
