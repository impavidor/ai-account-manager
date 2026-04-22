using AccountManager.Domain.Shared;

namespace AccountManager.Infrastructure.Dtos;

public class ProviderDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Npi { get; set; } = "";
    public ContactStatus Status { get; set; }
}
