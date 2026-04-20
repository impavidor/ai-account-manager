using AccountManager.Common.Errors;
using AccountManager.Domain.Contacts.ValueObjects;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.Contacts.Services;

public interface IRegisterProviderService
{
    Task<Result<Provider, Error>> RegisterAsync(ProviderName name, Npi npi, CancellationToken ct = default);
}
