using System.Security.Claims;
using AccountManager.Domain.Administration;

namespace AccountManager.WebAPI;

public class FakeAuthMiddleware
{
    private readonly RequestDelegate _next;

    public FakeAuthMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Actor-Id", out var actorIdValue) &&
            context.Request.Headers.TryGetValue("X-Actor-Type", out var actorTypeValue) &&
            Guid.TryParse(actorIdValue, out var actorId) &&
            Enum.TryParse<ContactType>(actorTypeValue, out var actorType))
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, actorId.ToString()),
                new Claim(ClaimTypes.Role, actorType.ToString())
            };
            context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "FakeAuth"));
        }

        await _next(context);
    }
}
