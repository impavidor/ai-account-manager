using AccountManager.Domain.Administration;
using AccountManager.Domain.Shared;
using FluentAssertions;

namespace AccountManager.Domain.Tests.Administration;

[TestFixture]
public class ContactTests
{
    private static Contact PendingContact() => new(
        new ContactId(Guid.NewGuid()),
        ContactType.Provider,
        ContactStatus.Pending,
        new FullName("Alice", "Smith"));

    private static Contact ActiveContact() => new(
        new ContactId(Guid.NewGuid()),
        ContactType.Provider,
        ContactStatus.Active,
        new FullName("Alice", "Smith"));

    private static Contact DeletedContact() => new(
        new ContactId(Guid.NewGuid()),
        ContactType.Provider,
        ContactStatus.Deleted,
        new FullName("Alice", "Smith"));

    // --- Activate ---

    [Test]
    public void Activate_WhenPending_Succeeds()
    {
        var contact = PendingContact();

        var result = contact.Activate();

        result.IsSuccess.Should().BeTrue();
        contact.Status.Should().Be(ContactStatus.Active);
    }

    [Test]
    public void Activate_WhenAlreadyActive_Fails()
    {
        var contact = ActiveContact();

        var result = contact.Activate();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidStatusTransitionError>();
    }

    [Test]
    public void Activate_WhenDeleted_Fails()
    {
        var contact = DeletedContact();

        var result = contact.Activate();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidStatusTransitionError>();
    }

    // --- Delete ---

    [Test]
    public void Delete_WhenPending_Succeeds()
    {
        var contact = PendingContact();

        var result = contact.Delete();

        result.IsSuccess.Should().BeTrue();
        contact.Status.Should().Be(ContactStatus.Deleted);
    }

    [Test]
    public void Delete_WhenActive_Succeeds()
    {
        var contact = ActiveContact();

        var result = contact.Delete();

        result.IsSuccess.Should().BeTrue();
        contact.Status.Should().Be(ContactStatus.Deleted);
    }

    [Test]
    public void Delete_WhenAlreadyDeleted_Fails()
    {
        var contact = DeletedContact();

        var result = contact.Delete();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidStatusTransitionError>();
    }
}
