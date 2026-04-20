using AccountManager.Domain.SelfService;
using FluentAssertions;

namespace AccountManager.Domain.Tests.Contacts.ValueObjects;

[TestFixture]
public class NpiTests
{
    [Test]
    public void Create_WithValidTenDigitNumber_Succeeds()
    {
        var result = Npi.Create("1234567890");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("1234567890");
    }

    [Test]
    public void Create_WithNonNumericInput_Fails()
    {
        var result = Npi.Create("123456789a");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidNpiError>();
    }

    [Test]
    public void Create_WithFewerThanTenDigits_Fails()
    {
        var result = Npi.Create("123456789");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidNpiError>();
    }

    [Test]
    public void Create_WithMoreThanTenDigits_Fails()
    {
        var result = Npi.Create("12345678901");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidNpiError>();
    }

    [Test]
    public void Create_WithEmptyString_Fails()
    {
        var result = Npi.Create("");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<InvalidNpiError>();
    }
}
