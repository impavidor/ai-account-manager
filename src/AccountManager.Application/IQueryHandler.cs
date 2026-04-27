using AccountManager.Common.Errors;
using CSharpFunctionalExtensions;

namespace AccountManager.Application;

public interface IQueryHandler<TQuery, TResult>
{
    Task<Result<TResult, Error>> Handle(TQuery query);
}
