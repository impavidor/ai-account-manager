using AccountManager.Common.Errors;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class RegisterProviderAdminCommand
{
    public ProviderName Name { get; }

    private RegisterProviderAdminCommand(ProviderName name) => Name = name;

    public static Result<RegisterProviderAdminCommand, Error> Create(string firstName, string lastName)
    {
        var nameResult = ProviderName.Create(firstName, lastName);
        if (nameResult.IsFailure) return nameResult.ConvertFailure<RegisterProviderAdminCommand>();

        return new RegisterProviderAdminCommand(nameResult.Value);
    }
}
