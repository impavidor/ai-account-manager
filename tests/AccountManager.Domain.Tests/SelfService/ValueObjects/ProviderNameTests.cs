using AccountManager.Domain.SelfService;
using FluentAssertions;

namespace AccountManager.Domain.Tests.Contacts.ValueObjects;

[TestFixture]
public class ProviderNameTests
{
    [Test]
    public void Create_WithValidNames_Succeeds()
    {
        var result = ProviderName.Create("Alice", "Smith");

        result.IsSuccess.Should().BeTrue();
        result.Value.FirstName.Should().Be("Alice");
        result.Value.LastName.Should().Be("Smith");
    }

    [Test]
    public void Create_TrimsWhitespace()
    {
        var result = ProviderName.Create("  Alice  ", "  Smith  ");

        result.IsSuccess.Should().BeTrue();
        result.Value.FirstName.Should().Be("Alice");
        result.Value.LastName.Should().Be("Smith");
    }

    [Test]
    public void Create_WithEmptyFirstName_Fails()
    {
        var result = ProviderName.Create("", "Smith");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidProviderNameError>();
    }

    [Test]
    public void Create_WithEmptyLastName_Fails()
    {
        var result = ProviderName.Create("Alice", "");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidProviderNameError>();
    }

    [Test]
    public void Create_WithNonLetterFirstName_Fails()
    {
        var result = ProviderName.Create("Al1ce", "Smith");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidProviderNameError>();
    }

    [Test]
    public void Create_WithNonLetterLastName_Fails()
    {
        var result = ProviderName.Create("Alice", "Sm1th");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidProviderNameError>();
    }

    [Test]
    public void Create_WithFirstNameExceedingFiftyChars_Fails()
    {
        var result = ProviderName.Create(new string('A', 51), "Smith");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidProviderNameError>();
    }

    [Test]
    public void Create_WithLastNameExceedingFiftyChars_Fails()
    {
        var result = ProviderName.Create("Alice", new string('S', 51));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidProviderNameError>();
    }

    [Test]
    public void Create_WithExactlyFiftyCharName_Succeeds()
    {
        var result = ProviderName.Create(new string('A', 50), "Smith");

        result.IsSuccess.Should().BeTrue();
    }
}
