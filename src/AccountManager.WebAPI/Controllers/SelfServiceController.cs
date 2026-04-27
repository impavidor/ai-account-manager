using AccountManager.Application;
using AccountManager.Application.SelfService;
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
        [FromServices] IProviderRepository repo)
    {
        var commandResult = RegisterProviderCommand.Create(request.FirstName, request.LastName, request.Npi);
        if (commandResult.IsFailure) return _mapper.MapError(commandResult.Error);

        var result = await new RegisterProviderHandler(repo).Handle(commandResult.Value);
        return result.Match(_mapper.Map, _mapper.MapError);
    }

    [HttpPost("/self-service/provider-admins")]
    public async Task<IActionResult> RegisterProviderAdmin(
        [FromBody] RegisterNameRequest request,
        [FromServices] IProviderAdminRepository repo)
    {
        var commandResult = RegisterProviderAdminCommand.Create(request.FirstName, request.LastName);
        if (commandResult.IsFailure) return _mapper.MapError(commandResult.Error);

        var result = await new RegisterProviderAdminHandler(repo).Handle(commandResult.Value);
        return result.Match(_mapper.Map, _mapper.MapError);
    }

    [HttpPost("/self-service/system-admins")]
    public async Task<IActionResult> RegisterSystemAdmin(
        [FromBody] RegisterNameRequest request,
        [FromServices] ISystemAdminRepository repo)
    {
        var commandResult = RegisterSystemAdminCommand.Create(request.FirstName, request.LastName);
        if (commandResult.IsFailure) return _mapper.MapError(commandResult.Error);

        var result = await new RegisterSystemAdminHandler(repo).Handle(commandResult.Value);
        return result.Match(_mapper.Map, _mapper.MapError);
    }

    [HttpGet("/self-service/account")]
    public async Task<IActionResult> GetAccount(
        [FromServices] IAccountProjector projector,
        [FromServices] ICurrentActor actor)
    {
        var result = await new GetAccountHandler(projector, actor).Handle(new GetAccountQuery());
        return result.Match(dto => (IActionResult)Ok(dto), _mapper.MapError);
    }

    [HttpPatch("/self-service/account/npi")]
    public async Task<IActionResult> ChangeNpi(
        [FromBody] ChangeNpiRequest request,
        [FromServices] IProviderRepository repo,
        [FromServices] ICurrentActor actor)
    {
        var commandResult = ChangeProviderNpiCommand.Create(request.Npi);
        if (commandResult.IsFailure) return _mapper.MapError(commandResult.Error);

        var result = await new ChangeProviderNpiHandler(repo, actor).Handle(commandResult.Value);
        return result.Match(_mapper.Map, _mapper.MapError);
    }

    [HttpPatch("/self-service/account/name")]
    public async Task<IActionResult> ChangeName(
        [FromBody] RegisterNameRequest request,
        [FromServices] IProviderRepository providerRepo,
        [FromServices] IProviderAdminRepository providerAdminRepo,
        [FromServices] ISystemAdminRepository systemAdminRepo,
        [FromServices] ICurrentActor actor)
    {
        var commandResult = ChangeNameCommand.Create(request.FirstName, request.LastName);
        if (commandResult.IsFailure) return _mapper.MapError(commandResult.Error);

        var result = await new ChangeNameHandler(providerRepo, providerAdminRepo, systemAdminRepo, actor)
            .Handle(commandResult.Value);
        return result.Match(_mapper.Map, _mapper.MapError);
    }
}

public record RegisterProviderRequest(string FirstName, string LastName, string Npi);
public record RegisterNameRequest(string FirstName, string LastName);
public record ChangeNpiRequest(string Npi);
