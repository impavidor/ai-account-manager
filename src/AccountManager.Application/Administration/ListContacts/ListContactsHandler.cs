using AccountManager.Common.Errors;
using CSharpFunctionalExtensions;

namespace AccountManager.Application.Administration;

public sealed class ListContactsHandler : IQueryHandler<ListContactsQuery, IReadOnlyList<ContactSummaryDto>>
{
    private readonly IContactProjector _projector;

    public ListContactsHandler(IContactProjector projector) => _projector = projector;

    public async Task<Result<IReadOnlyList<ContactSummaryDto>, Error>> Handle(ListContactsQuery query) =>
        Result.Success<IReadOnlyList<ContactSummaryDto>, Error>(await _projector.GetAll());
}
