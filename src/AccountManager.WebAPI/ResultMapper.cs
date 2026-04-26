using AccountManager.Application;
using AccountManager.Common.Errors;
using AccountManager.Domain.Administration;
using AccountManager.Domain.SelfService;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebAPI;

public class ResultMapper
{
    public IActionResult Map(CommandResult result) => result switch
    {
        CommandResult.OkResult => new NoContentResult(),
        CommandResult.CreatedResult(var id) => new ObjectResult(new { id }) { StatusCode = 201 },
        _ => new ObjectResult("Unexpected command result") { StatusCode = 500 }
    };

    public IActionResult MapError(Error error) => error switch
    {
        ContactNotFoundError e => new ObjectResult(e.Message) { StatusCode = 404 },
        InvalidStatusTransitionError e => new ObjectResult(e.Message) { StatusCode = 409 },
        SelfActionForbiddenError e => new ObjectResult(e.Message) { StatusCode = 403 },
        InvalidNpiError e => new ObjectResult(e.Message) { StatusCode = 422 },
        InvalidProviderNameError e => new ObjectResult(e.Message) { StatusCode = 422 },
        _ => new ObjectResult(error.Message) { StatusCode = 500 }
    };
}
