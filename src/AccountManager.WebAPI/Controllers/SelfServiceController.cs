using AccountManager.Application.SelfService;
using AccountManager.Common.Persistence;
using AccountManager.Domain.SelfService;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebAPI.Controllers;

[ApiController]
public class SelfServiceController : ControllerBase
{
    private readonly ResultMapper _mapper;

    public SelfServiceController(ResultMapper mapper) => _mapper = mapper;

    [HttpPost("/self-service/providers")]
    public async Task<IActionResult> RegisterProvider(
        [FromBody] RegisterProviderRequest request,
        [FromServices] IProviderRepository repo,
        [FromServices] IUnitOfWork uow)
    {
        var commandResult = RegisterProviderCommand.Create(request.FirstName, request.LastName, request.Npi);
        if (commandResult.IsFailure) return _mapper.MapError(commandResult.Error);

        var result = await new RegisterProviderHandler(repo, uow).Handle(commandResult.Value);
        return result.Match(_mapper.Map, _mapper.MapError);
    }

    [HttpPost("/self-service/provider-admins")]
    public async Task<IActionResult> RegisterProviderAdmin(
        [FromBody] RegisterNameRequest request,
        [FromServices] IProviderAdminRepository repo,
        [FromServices] IUnitOfWork uow)
    {
        var commandResult = RegisterProviderAdminCommand.Create(request.FirstName, request.LastName);
        if (commandResult.IsFailure) return _mapper.MapError(commandResult.Error);

        var result = await new RegisterProviderAdminHandler(repo, uow).Handle(commandResult.Value);
        return result.Match(_mapper.Map, _mapper.MapError);
    }

    [HttpPost("/self-service/system-admins")]
    public async Task<IActionResult> RegisterSystemAdmin(
        [FromBody] RegisterNameRequest request,
        [FromServices] ISystemAdminRepository repo,
        [FromServices] IUnitOfWork uow)
    {
        var commandResult = RegisterSystemAdminCommand.Create(request.FirstName, request.LastName);
        if (commandResult.IsFailure) return _mapper.MapError(commandResult.Error);

        var result = await new RegisterSystemAdminHandler(repo, uow).Handle(commandResult.Value);
        return result.Match(_mapper.Map, _mapper.MapError);
    }
}

public record RegisterProviderRequest(string FirstName, string LastName, string Npi);
public record RegisterNameRequest(string FirstName, string LastName);
