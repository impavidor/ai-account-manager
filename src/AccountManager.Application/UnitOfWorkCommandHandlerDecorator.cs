using AccountManager.Common.Errors;
using AccountManager.Common.Persistence;
using CSharpFunctionalExtensions;

namespace AccountManager.Application;

public sealed class UnitOfWorkCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
{
    private readonly ICommandHandler<TCommand> _inner;
    private readonly IUnitOfWork _uow;

    public UnitOfWorkCommandHandlerDecorator(ICommandHandler<TCommand> inner, IUnitOfWork uow)
    {
        _inner = inner;
        _uow = uow;
    }

    public async Task<Result<CommandResult, Error>> Handle(TCommand command) =>
        await _inner.Handle(command)
            .Tap(_ => _uow.SaveChanges());
}
