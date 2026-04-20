using AccountManager.Domain.SelfService;
using AccountManager.Domain.Shared;
using FluentAssertions;

namespace AccountManager.Domain.Tests.Contacts;

[TestFixture]
public class SystemAdminTests
{
    private static ProviderName ValidName() => ProviderName.Create("Alice", "Smith").Value;

    [Test]
    public void Register_CreatesSystemAdminWithPendingStatus()
    {
        var admin = SystemAdmin.Register(ValidName());

        admin.Status.Should().Be(ContactStatus.Pending);
    }

    [Test]
    public void Register_AssignsProvidedName()
    {
        var name = ValidName();

        var admin = SystemAdmin.Register(name);

        admin.Name.Should().Be(name);
    }

    [Test]
    public void Register_GeneratesUniqueId()
    {
        var a = SystemAdmin.Register(ValidName());
        var b = SystemAdmin.Register(ValidName());

        a.Id.Should().NotBe(b.Id);
    }

    [Test]
    public void ChangeName_ReplacesName()
    {
        var admin = SystemAdmin.Register(ValidName());
        var newName = ProviderName.Create("Bob", "Jones").Value;

        admin.ChangeName(newName);

        admin.Name.Should().Be(newName);
    }
}
