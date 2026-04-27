using AccountManager.Application;
using AccountManager.Application.Administration;
using AccountManager.Common.Errors;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebAPI.Controllers;

[ApiController]
public class AdministrationController : ControllerBase
{
    private readonly ResultMapper _mapper;
    private readonly ICommandHandler<VerifyContactCommand> _verifyContact;
    private readonly ICommandHandler<DeleteContactCommand> _deleteContact;
    private readonly IQueryHandler<GetContactQuery, ContactDto> _getContact;
    private readonly IQueryHandler<ListContactsQuery, IReadOnlyList<ContactSummaryDto>> _listContacts;

    public AdministrationController(
        ResultMapper mapper,
        ICommandHandler<VerifyContactCommand> verifyContact,
        ICommandHandler<DeleteContactCommand> deleteContact,
        IQueryHandler<GetContactQuery, ContactDto> getContact,
        IQueryHandler<ListContactsQuery, IReadOnlyList<ContactSummaryDto>> listContacts)
    {
        _mapper = mapper;
        _verifyContact = verifyContact;
        _deleteContact = deleteContact;
        _getContact = getContact;
        _listContacts = listContacts;
    }

    [HttpPost("/administration/contacts/{id}/verify")]
    public async Task<IActionResult> VerifyContact(Guid id) =>
        await VerifyContactCommand.Create(id)
            .Bind(_verifyContact.Handle)
            .Match(_mapper.Map, _mapper.MapError);

    [HttpDelete("/administration/contacts/{id}")]
    public async Task<IActionResult> DeleteContact(Guid id) =>
        await DeleteContactCommand.Create(id)
            .Bind(_deleteContact.Handle)
            .Match(_mapper.Map, _mapper.MapError);

    [HttpGet("/administration/contacts/{id}")]
    public async Task<IActionResult> GetContact(Guid id) =>
        await GetContactQuery.Create(id)
            .Bind(_getContact.Handle)
            .Match(dto => (IActionResult)Ok(dto), _mapper.MapError);

    [HttpGet("/administration/contacts")]
    public async Task<IActionResult> ListContacts() =>
        await _listContacts.Handle(new ListContactsQuery())
            .Match(list => (IActionResult)Ok(list), _mapper.MapError);
}
