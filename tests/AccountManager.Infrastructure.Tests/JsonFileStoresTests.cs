using AccountManager.Domain.Administration;
using AccountManager.Domain.Shared;
using AccountManager.Infrastructure;
using AccountManager.Infrastructure.Dtos;
using FluentAssertions;
using System.Text.Json;

namespace AccountManager.Infrastructure.Tests;

[TestFixture]
public class JsonFileStoresTests
{
    private string _tempDir = null!;

    [SetUp]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Test]
    public void Constructor_NoContactsJson_InitializesEmptyContactsList()
    {
        var stores = new JsonFileStores(_tempDir);

        stores.Contacts.Should().BeEmpty();
    }

    [Test]
    public async Task Constructor_ExistingContactsJson_LoadsContacts()
    {
        var dto = new ContactDto { Id = Guid.NewGuid(), Type = ContactType.Provider, Status = ContactStatus.Pending, FirstName = "Alice", LastName = "Smith" };
        await File.WriteAllTextAsync(Path.Combine(_tempDir, "contacts.json"), JsonSerializer.Serialize(new[] { dto }));

        var stores = new JsonFileStores(_tempDir);

        stores.Contacts.Should().HaveCount(1);
        stores.Contacts[0].FirstName.Should().Be("Alice");
    }

    [Test]
    public async Task SaveChanges_PersistsContactsToDisk()
    {
        var stores = new JsonFileStores(_tempDir);
        stores.Contacts.Add(new ContactDto { Id = Guid.NewGuid(), Type = ContactType.SystemAdmin, Status = ContactStatus.Active, FirstName = "Bob", LastName = "Jones" });

        await stores.SaveChanges();

        var reloaded = new JsonFileStores(_tempDir);
        reloaded.Contacts.Should().HaveCount(1);
        reloaded.Contacts[0].FirstName.Should().Be("Bob");
    }

    [Test]
    public async Task AddToContacts_ThenSaveChanges_RoundTrips()
    {
        var id = Guid.NewGuid();
        var stores = new JsonFileStores(_tempDir);
        stores.Contacts.Add(new ContactDto { Id = id, Type = ContactType.ProviderAdmin, Status = ContactStatus.Pending, FirstName = "Carol", LastName = "White" });

        await stores.SaveChanges();

        var reloaded = new JsonFileStores(_tempDir);
        reloaded.Contacts.Should().HaveCount(1);
        reloaded.Contacts[0].Id.Should().Be(id);
        reloaded.Contacts[0].FirstName.Should().Be("Carol");
    }
}
