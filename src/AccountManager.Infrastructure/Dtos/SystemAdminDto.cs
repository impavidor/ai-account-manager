using AccountManager.Domain.Shared;

namespace AccountManager.Infrastructure.Dtos;

public class SystemAdminDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public ContactStatus Status { get; set; }
}
