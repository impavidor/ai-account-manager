using AccountManager.Common.Errors;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.Administration;

public sealed class ListContactsHandler
{
    private readonly IContactProjector _projector;

    public ListContactsHandler(IContactProjector projector) => _projector = projector;

    public async Task<Result<IReadOnlyList<ContactSummaryDto>, Error>> Handle(ListContactsQuery query)
    {
        var summaries = await _projector.GetAll();
        return Result.Success<IReadOnlyList<ContactSummaryDto>, Error>(summaries);
    }
}
