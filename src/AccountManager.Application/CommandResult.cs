namespace AccountManager.Application;

public abstract record CommandResult
{
    private CommandResult() { }

    public sealed record OkResult : CommandResult;
    public sealed record CreatedResult(Guid Id) : CommandResult;

    public static CommandResult Ok() => new OkResult();
    public static CommandResult Created(Guid id) => new CreatedResult(id);
}
