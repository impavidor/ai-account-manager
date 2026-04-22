using AccountManager.Domain.Administration;
using AccountManager.Domain.Shared;

namespace AccountManager.Infrastructure.Dtos;

public class ContactDto
{
    public Guid Id { get; set; }
    public ContactType Type { get; set; }
    public ContactStatus Status { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? OrgName { get; set; }
}
