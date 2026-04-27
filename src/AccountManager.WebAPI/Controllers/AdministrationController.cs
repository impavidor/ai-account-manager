using AccountManager.Application;
using AccountManager.Application.Administration;
using AccountManager.Domain.Administration;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebAPI.Controllers;

[ApiController]
public class AdministrationController : ControllerBase
{
    private readonly ResultMapper _mapper;

    public AdministrationController(ResultMapper mapper) => _mapper = mapper;

    [HttpPost("/administration/contacts/{id}/verify")]
    public async Task<IActionResult> VerifyContact(
        Guid id,
        [FromServices] IActivateContactService service,
        [FromServices] ICurrentActor actor)
    {
        var command = VerifyContactCommand.Create(id).Value;
        var result = await new VerifyContactHandler(service, actor).Handle(command);
        return result.Match(_mapper.Map, _mapper.MapError);
    }

    [HttpDelete("/administration/contacts/{id}")]
    public async Task<IActionResult> DeleteContact(
        Guid id,
        [FromServices] IDeleteContactService service,
        [FromServices] ICurrentActor actor)
    {
        var command = DeleteContactCommand.Create(id).Value;
        var result = await new DeleteContactHandler(service, actor).Handle(command);
        return result.Match(_mapper.Map, _mapper.MapError);
    }

    [HttpGet("/administration/contacts/{id}")]
    public async Task<IActionResult> GetContact(
        Guid id,
        [FromServices] IContactProjector projector)
    {
        var query = GetContactQuery.Create(id).Value;
        var result = await new GetContactHandler(projector).Handle(query);
        return result.Match(dto => (IActionResult)Ok(dto), _mapper.MapError);
    }

    [HttpGet("/administration/contacts")]
    public async Task<IActionResult> ListContacts(
        [FromServices] IContactProjector projector)
    {
        var result = await new ListContactsHandler(projector).Handle(new ListContactsQuery());
        return result.Match(list => (IActionResult)Ok(list), _mapper.MapError);
    }
}
