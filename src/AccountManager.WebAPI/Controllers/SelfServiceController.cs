using AccountManager.Application;
using AccountManager.Application.SelfService;
using AccountManager.Common.Errors;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebAPI.Controllers;

[ApiController]
public class SelfServiceController : ControllerBase
{
    private readonly ResultMapper _mapper;
    private readonly ICommandHandler<RegisterProviderCommand> _registerProvider;
    private readonly ICommandHandler<RegisterProviderAdminCommand> _registerProviderAdmin;
    private readonly ICommandHandler<RegisterSystemAdminCommand> _registerSystemAdmin;
    private readonly ICommandHandler<ChangeProviderNpiCommand> _changeProviderNpi;
    private readonly ICommandHandler<ChangeNameCommand> _changeName;
    private readonly IQueryHandler<GetAccountQuery, AccountDto> _getAccount;

    public SelfServiceController(
        ResultMapper mapper,
        ICommandHandler<RegisterProviderCommand> registerProvider,
        ICommandHandler<RegisterProviderAdminCommand> registerProviderAdmin,
        ICommandHandler<RegisterSystemAdminCommand> registerSystemAdmin,
        ICommandHandler<ChangeProviderNpiCommand> changeProviderNpi,
        ICommandHandler<ChangeNameCommand> changeName,
        IQueryHandler<GetAccountQuery, AccountDto> getAccount)
    {
        _mapper = mapper;
        _registerProvider = registerProvider;
        _registerProviderAdmin = registerProviderAdmin;
        _registerSystemAdmin = registerSystemAdmin;
        _changeProviderNpi = changeProviderNpi;
        _changeName = changeName;
        _getAccount = getAccount;
    }

    [HttpPost("/self-service/providers")]
    public async Task<IActionResult> RegisterProvider([FromBody] RegisterProviderRequest request) =>
        await RegisterProviderCommand.Create(request.FirstName, request.LastName, request.Npi)
            .Bind(_registerProvider.Handle)
            .Match(_mapper.Map, _mapper.MapError);

    [HttpPost("/self-service/provider-admins")]
    public async Task<IActionResult> RegisterProviderAdmin([FromBody] RegisterNameRequest request) =>
        await RegisterProviderAdminCommand.Create(request.FirstName, request.LastName)
            .Bind(_registerProviderAdmin.Handle)
            .Match(_mapper.Map, _mapper.MapError);

    [HttpPost("/self-service/system-admins")]
    public async Task<IActionResult> RegisterSystemAdmin([FromBody] RegisterNameRequest request) =>
        await RegisterSystemAdminCommand.Create(request.FirstName, request.LastName)
            .Bind(_registerSystemAdmin.Handle)
            .Match(_mapper.Map, _mapper.MapError);

    [HttpGet("/self-service/account")]
    public async Task<IActionResult> GetAccount() =>
        await _getAccount.Handle(new GetAccountQuery())
            .Match(dto => (IActionResult)Ok(dto), _mapper.MapError);

    [HttpPatch("/self-service/account/npi")]
    public async Task<IActionResult> ChangeNpi([FromBody] ChangeNpiRequest request) =>
        await ChangeProviderNpiCommand.Create(request.Npi)
            .Bind(_changeProviderNpi.Handle)
            .Match(_mapper.Map, _mapper.MapError);

    [HttpPatch("/self-service/account/name")]
    public async Task<IActionResult> ChangeName([FromBody] RegisterNameRequest request) =>
        await ChangeNameCommand.Create(request.FirstName, request.LastName)
            .Bind(_changeName.Handle)
            .Match(_mapper.Map, _mapper.MapError);
}

public record RegisterProviderRequest(string FirstName, string LastName, string Npi);
public record RegisterNameRequest(string FirstName, string LastName);
public record ChangeNpiRequest(string Npi);
