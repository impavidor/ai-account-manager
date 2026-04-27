using System.Text.Json;

namespace AccountManager.IntegrationTests;

/// <summary>
/// Reads and writes contacts.json in the temp data directory.
/// Uses locally-defined record types — no imports from Infrastructure, Application, or Domain.
/// Enum integer values match those serialized by System.Text.Json defaults.
/// </summary>
public class DataStore
{
    private readonly string _dataPath;

    // ContactType: Provider=0, ProviderAdmin=1, Organization=2, SystemAdmin=3
    public const int ContactTypeProvider = 0;
    public const int ContactTypeProviderAdmin = 1;
    public const int ContactTypeOrganization = 2;
    public const int ContactTypeSystemAdmin = 3;

    // ContactStatus: Pending=0, Active=1, Deleted=2
    public const int ContactStatusPending = 0;
    public const int ContactStatusActive = 1;
    public const int ContactStatusDeleted = 2;

    public DataStore(string dataPath) => _dataPath = dataPath;

    public List<ContactRecord> ReadContacts()
    {
        var filePath = Path.Combine(_dataPath, "contacts.json");
        if (!File.Exists(filePath))
            return [];

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<ContactRecord>>(json) ?? [];
    }

    public void WriteContacts(List<ContactRecord> contacts)
    {
        var filePath = Path.Combine(_dataPath, "contacts.json");
        File.WriteAllText(filePath, JsonSerializer.Serialize(contacts));
    }
}

public record ContactRecord(
    Guid Id,
    int Type,
    int Status,
    string? FirstName,
    string? LastName,
    string? OrgName,
    string? Npi);
