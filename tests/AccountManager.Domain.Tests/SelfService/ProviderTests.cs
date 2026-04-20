using AccountManager.Domain.SelfService;
using AccountManager.Domain.Shared;
using FluentAssertions;

namespace AccountManager.Domain.Tests.SelfService;

[TestFixture]
public class ProviderTests
{
    private static ProviderName ValidName() => ProviderName.Create("Alice", "Smith").Value;
    private static Npi ValidNpi() => Npi.Create("1234567890").Value;

    [Test]
    public void Register_CreatesProviderWithPendingStatus()
    {
        var provider = Provider.Register(ValidName(), ValidNpi()).Value;

        provider.Status.Should().Be(ContactStatus.Pending);
    }

    [Test]
    public void Register_AssignsProvidedNameAndNpi()
    {
        var name = ValidName();
        var npi = ValidNpi();

        var provider = Provider.Register(name, npi).Value;

        provider.Name.Should().Be(name);
        provider.Npi.Should().Be(npi);
    }

    [Test]
    public void Register_GeneratesUniqueId()
    {
        var a = Provider.Register(ValidName(), ValidNpi()).Value;
        var b = Provider.Register(ValidName(), ValidNpi()).Value;

        a.Id.Should().NotBe(b.Id);
    }

    [Test]
    public void ChangeName_ReplacesName()
    {
        var provider = Provider.Register(ValidName(), ValidNpi()).Value;
        var newName = ProviderName.Create("Bob", "Jones").Value;

        provider.ChangeName(newName);

        provider.Name.Should().Be(newName);
    }

    [Test]
    public void ChangeNpi_ReplacesNpi()
    {
        var provider = Provider.Register(ValidName(), ValidNpi()).Value;
        var newNpi = Npi.Create("9876543210").Value;

        provider.ChangeNpi(newNpi);

        provider.Npi.Should().Be(newNpi);
    }
}
