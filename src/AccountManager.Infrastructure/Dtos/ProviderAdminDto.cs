using AccountManager.Domain.Shared;

namespace AccountManager.Infrastructure.Dtos;

public class ProviderAdminDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public ContactStatus Status { get; set; }
}
