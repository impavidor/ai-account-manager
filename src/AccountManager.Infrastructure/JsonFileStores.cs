using System.Text.Json;
using AccountManager.Common.Persistence;
using AccountManager.Infrastructure.Dtos;

namespace AccountManager.Infrastructure;

public class JsonFileStores : IUnitOfWork
{
    private readonly string _filePath;

    public List<ContactDto> Contacts { get; }

    public JsonFileStores(string basePath)
    {
        _filePath = Path.Combine(basePath, "contacts.json");
        Contacts = File.Exists(_filePath)
            ? JsonSerializer.Deserialize<List<ContactDto>>(File.ReadAllText(_filePath)) ?? []
            : [];
    }

    public async Task SaveChanges()
    {
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, Contacts);
    }
}
