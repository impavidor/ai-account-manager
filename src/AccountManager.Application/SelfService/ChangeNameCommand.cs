using AccountManager.Common.Errors;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.SelfService;

public sealed class ChangeNameCommand
{
    public ProviderName Name { get; }

    private ChangeNameCommand(ProviderName name) => Name = name;

    public static Result<ChangeNameCommand, Error> Create(string firstName, string lastName)
    {
        var nameResult = ProviderName.Create(firstName, lastName);
        if (nameResult.IsFailure) return nameResult.ConvertFailure<ChangeNameCommand>();
        return new ChangeNameCommand(nameResult.Value);
    }
}
