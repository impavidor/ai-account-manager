using AccountManager.Domain.Contacts;
using AccountManager.Domain.Contacts.ValueObjects;
using FluentAssertions;

namespace AccountManager.Domain.Tests.Contacts;

[TestFixture]
public class ProviderAdminTests
{
    private static ProviderName ValidName() => ProviderName.Create("Alice", "Smith").Value;

    [Test]
    public void Register_CreatesProviderAdminWithPendingStatus()
    {
        var admin = ProviderAdmin.Register(ValidName());

        admin.Status.Should().Be(ContactStatus.Pending);
    }

    [Test]
    public void Register_AssignsProvidedName()
    {
        var name = ValidName();

        var admin = ProviderAdmin.Register(name);

        admin.Name.Should().Be(name);
    }

    [Test]
    public void Register_GeneratesUniqueId()
    {
        var a = ProviderAdmin.Register(ValidName());
        var b = ProviderAdmin.Register(ValidName());

        a.Id.Should().NotBe(b.Id);
    }

    [Test]
    public void ChangeName_ReplacesName()
    {
        var admin = ProviderAdmin.Register(ValidName());
        var newName = ProviderName.Create("Bob", "Jones").Value;

        admin.ChangeName(newName);

        admin.Name.Should().Be(newName);
    }
}
