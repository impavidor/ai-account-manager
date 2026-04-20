using AccountManager.Common.Errors;
using AccountManager.Domain.Contacts.Repositories;
using AccountManager.Domain.Contacts.ValueObjects;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.Contacts.Services;

public class RegisterProviderService : IRegisterProviderService
{
    private readonly IProviderRepository _repository;

    public RegisterProviderService(IProviderRepository repository) => _repository = repository;

    public async Task<Result<Provider, Error>> RegisterAsync(ProviderName name, Npi npi, CancellationToken ct = default)
    {
        var provider = Provider.Register(name, npi);
        await _repository.SaveAsync(provider, ct);
        return Result.Success<Provider, Error>(provider);
    }
}
