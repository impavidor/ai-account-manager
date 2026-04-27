using AccountManager.Application.Administration;
using AccountManager.Domain.Administration;
using AccountManager.Domain.Shared;
using FluentAssertions;

namespace AccountManager.Application.Tests.Administration;

[TestFixture]
public class AdminReadHandlerTests
{
    // --- GetContact ---

    [Test]
    public async Task GetContact_ExistingContact_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var dto = new ContactDto(id, ContactType.Provider, ContactStatus.Active, "Alice", "Smith");
        var projector = new FakeContactProjector([dto], null);
        var handler = new GetContactHandler(projector);
        var query = GetContactQuery.Create(id).Value;

        var result = await handler.Handle(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.FirstName.Should().Be("Alice");
    }

    [Test]
    public async Task GetContact_NotFound_ReturnsError()
    {
        var projector = new FakeContactProjector([], null);
        var handler = new GetContactHandler(projector);
        var query = GetContactQuery.Create(Guid.NewGuid()).Value;

        var result = await handler.Handle(query);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ContactNotFoundError>();
    }

    // --- ListContacts ---

    [Test]
    public async Task ListContacts_ReturnsAllSummaries()
    {
        var summaries = new List<ContactSummaryDto>
        {
            new(Guid.NewGuid(), ContactType.Provider, ContactStatus.Pending, "Alice", "Smith"),
            new(Guid.NewGuid(), ContactType.SystemAdmin, ContactStatus.Active, "Bob", "Jones")
        };
        var projector = new FakeContactProjector([], summaries);
        var handler = new ListContactsHandler(projector);

        var result = await handler.Handle(new ListContactsQuery());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Test]
    public async Task ListContacts_WhenEmpty_ReturnsEmptyList()
    {
        var projector = new FakeContactProjector([], new List<ContactSummaryDto>());
        var handler = new ListContactsHandler(projector);

        var result = await handler.Handle(new ListContactsQuery());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}

file sealed class FakeContactProjector : IContactProjector
{
    private readonly IReadOnlyList<ContactDto> _contacts;
    private readonly IReadOnlyList<ContactSummaryDto>? _summaries;

    public FakeContactProjector(IReadOnlyList<ContactDto> contacts, IReadOnlyList<ContactSummaryDto>? summaries)
    {
        _contacts = contacts;
        _summaries = summaries;
    }

    public Task<ContactDto?> GetById(ContactId id) =>
        Task.FromResult(_contacts.FirstOrDefault(c => c.Id == id.Value));

    public Task<IReadOnlyList<ContactSummaryDto>> GetAll() =>
        Task.FromResult(_summaries ?? (IReadOnlyList<ContactSummaryDto>)Array.Empty<ContactSummaryDto>());
}
