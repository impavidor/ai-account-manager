using System.Net;
using System.Security.Claims;
using AccountManager.Domain.Administration;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace AccountManager.Application.Tests;

[TestFixture]
public class FakeAuthMiddlewareTests
{
    private IHost _host = null!;
    private HttpClient _client = null!;
    private ClaimsPrincipal? _capturedUser;

    [SetUp]
    public async Task SetUp()
    {
        _capturedUser = null;
        _host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder.UseTestServer();
                webBuilder.Configure(app =>
                {
                    app.UseMiddleware<AccountManager.WebAPI.FakeAuthMiddleware>();
                    app.Run(ctx =>
                    {
                        _capturedUser = ctx.User;
                        return Task.CompletedTask;
                    });
                });
            })
            .StartAsync();
        _client = _host.GetTestClient();
    }

    [TearDown]
    public async Task TearDown()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
    }

    [Test]
    public async Task ValidHeaders_SetClaimsPrincipalOnContext()
    {
        var actorId = Guid.NewGuid();
        var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("X-Actor-Id", actorId.ToString());
        request.Headers.Add("X-Actor-Type", "SystemAdmin");

        await _client.SendAsync(request);

        _capturedUser.Should().NotBeNull();
        _capturedUser!.FindFirstValue(ClaimTypes.NameIdentifier).Should().Be(actorId.ToString());
        _capturedUser!.FindFirstValue(ClaimTypes.Role).Should().Be("SystemAdmin");
    }

    [Test]
    public async Task MissingHeaders_LeavesUserUnchanged()
    {
        await _client.GetAsync("/");

        _capturedUser.Should().NotBeNull();
        _capturedUser!.Identity!.IsAuthenticated.Should().BeFalse();
    }

    [Test]
    public async Task InvalidActorId_LeavesUserUnchanged()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("X-Actor-Id", "not-a-guid");
        request.Headers.Add("X-Actor-Type", "SystemAdmin");

        await _client.SendAsync(request);

        _capturedUser!.Identity!.IsAuthenticated.Should().BeFalse();
    }

    [Test]
    public async Task InvalidActorType_LeavesUserUnchanged()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("X-Actor-Id", Guid.NewGuid().ToString());
        request.Headers.Add("X-Actor-Type", "NotARealType");

        await _client.SendAsync(request);

        _capturedUser!.Identity!.IsAuthenticated.Should().BeFalse();
    }
}
