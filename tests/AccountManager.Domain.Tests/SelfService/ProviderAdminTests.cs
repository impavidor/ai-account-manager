using AccountManager.Domain.SelfService;
using AccountManager.Domain.Shared;
using FluentAssertions;

namespace AccountManager.Domain.Tests.SelfService;

[TestFixture]
public class ProviderAdminTests
{
    private static ProviderName ValidName() => ProviderName.Create("Alice", "Smith").Value;

    [Test]
    public void Register_CreatesProviderAdminWithPendingStatus()
    {
        var admin = ProviderAdmin.Register(ValidName()).Value;

        admin.Status.Should().Be(ContactStatus.Pending);
    }

    [Test]
    public void Register_AssignsProvidedName()
    {
        var name = ValidName();

        var admin = ProviderAdmin.Register(name).Value;

        admin.Name.Should().Be(name);
    }

    [Test]
    public void Register_GeneratesUniqueId()
    {
        var a = ProviderAdmin.Register(ValidName()).Value;
        var b = ProviderAdmin.Register(ValidName()).Value;

        a.Id.Should().NotBe(b.Id);
    }

    [Test]
    public void ChangeName_ReplacesName()
    {
        var admin = ProviderAdmin.Register(ValidName()).Value;
        var newName = ProviderName.Create("Bob", "Jones").Value;

        admin.ChangeName(newName);

        admin.Name.Should().Be(newName);
    }
}
