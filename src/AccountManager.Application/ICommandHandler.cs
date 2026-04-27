using AccountManager.Common.Errors;
using CSharpFunctionalExtensions;

namespace AccountManager.Application;

public interface ICommandHandler<TCommand>
{
    Task<Result<CommandResult, Error>> Handle(TCommand command);
}
