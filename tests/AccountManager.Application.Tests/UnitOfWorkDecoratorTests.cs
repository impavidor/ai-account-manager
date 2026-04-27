using AccountManager.Common.Errors;
using AccountManager.Common.Persistence;
using CSharpFunctionalExtensions;
using FluentAssertions;

namespace AccountManager.Application.Tests;

[TestFixture]
public class UnitOfWorkDecoratorTests
{
    [Test]
    public async Task Handle_InnerHandlerSucceeds_CallsSaveChangesOnce()
    {
        var uow = new SpyUnitOfWork();
        var inner = new StubCommandHandler(Result.Success<CommandResult, Error>(CommandResult.Ok()));
        var decorator = new UnitOfWorkCommandHandlerDecorator<StubCommand>(inner, uow);

        await decorator.Handle(new StubCommand());

        uow.SaveChangesCallCount.Should().Be(1);
    }

    [Test]
    public async Task Handle_InnerHandlerFails_DoesNotCallSaveChanges()
    {
        var uow = new SpyUnitOfWork();
        var inner = new StubCommandHandler(Result.Failure<CommandResult, Error>(new StubError()));
        var decorator = new UnitOfWorkCommandHandlerDecorator<StubCommand>(inner, uow);

        await decorator.Handle(new StubCommand());

        uow.SaveChangesCallCount.Should().Be(0);
    }
}

file sealed record StubCommand;
file sealed record StubError() : Error("stub");

file sealed class StubCommandHandler : ICommandHandler<StubCommand>
{
    private readonly Result<CommandResult, Error> _result;
    public StubCommandHandler(Result<CommandResult, Error> result) => _result = result;
    public Task<Result<CommandResult, Error>> Handle(StubCommand command) => Task.FromResult(_result);
}

file sealed class SpyUnitOfWork : IUnitOfWork
{
    public int SaveChangesCallCount { get; private set; }
    public Task SaveChanges() { SaveChangesCallCount++; return Task.CompletedTask; }
}
