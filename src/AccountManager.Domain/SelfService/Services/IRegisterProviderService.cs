using AccountManager.Common.Errors;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.SelfService;

public interface IRegisterProviderService
{
    Task<Result<Provider, Error>> RegisterAsync(ProviderName name, Npi npi);
}
