using AccountManager.Application;
using AccountManager.Application.Administration;
using AccountManager.Common.Persistence;
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
        [FromServices] IUnitOfWork uow,
        [FromServices] ICurrentActor actor)
    {
        var command = VerifyContactCommand.Create(id).Value;
        var result = await new VerifyContactHandler(service, uow, actor).Handle(command);
        return result.Match(_mapper.Map, _mapper.MapError);
    }

    [HttpDelete("/administration/contacts/{id}")]
    public async Task<IActionResult> DeleteContact(
        Guid id,
        [FromServices] IDeleteContactService service,
        [FromServices] IUnitOfWork uow,
        [FromServices] ICurrentActor actor)
    {
        var command = DeleteContactCommand.Create(id).Value;
        var result = await new DeleteContactHandler(service, uow, actor).Handle(command);
        return result.Match(_mapper.Map, _mapper.MapError);
    }
}
