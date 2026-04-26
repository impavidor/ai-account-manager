using System.Security.Claims;
using AccountManager.Application;
using AccountManager.Domain.Administration;
using Microsoft.AspNetCore.Http;

namespace AccountManager.WebAPI;

public class HttpContextCurrentActor : ICurrentActor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentActor(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public ContactId ContactId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!;
            return new ContactId(Guid.Parse(value));
        }
    }

    public ContactType ContactType
    {
        get
        {
            var value = _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.Role)!;
            return Enum.Parse<ContactType>(value);
        }
    }
}
