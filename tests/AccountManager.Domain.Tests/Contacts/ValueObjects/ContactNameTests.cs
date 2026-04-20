using AccountManager.Domain.Contacts.ValueObjects;
using FluentAssertions;

namespace AccountManager.Domain.Tests.Contacts.ValueObjects;

[TestFixture]
public class ContactNameTests
{
    [Test]
    public void FullName_StoresFirstAndLastName()
    {
        var name = new FullName("Alice", "Smith");

        name.FirstName.Should().Be("Alice");
        name.LastName.Should().Be("Smith");
    }

    [Test]
    public void OrganizationName_StoresName()
    {
        var name = new OrganizationName("Acme Corp");

        name.Name.Should().Be("Acme Corp");
    }

    [Test]
    public void FullName_IsContactName()
    {
        ContactName name = new FullName("Alice", "Smith");

        name.Should().BeOfType<FullName>();
    }

    [Test]
    public void OrganizationName_IsContactName()
    {
        ContactName name = new OrganizationName("Acme Corp");

        name.Should().BeOfType<OrganizationName>();
    }
}
