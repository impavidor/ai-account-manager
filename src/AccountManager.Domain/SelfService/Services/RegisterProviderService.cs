using AccountManager.Common.Errors;
using CSharpFunctionalExtensions;

namespace AccountManager.Domain.SelfService;

public class RegisterProviderService : IRegisterProviderService
{
    private readonly IProviderRepository _repository;

    public RegisterProviderService(IProviderRepository repository) => _repository = repository;

    public async Task<Result<Provider, Error>> RegisterAsync(ProviderName name, Npi npi, CancellationToken ct = default)
    {
        var result = Provider.Register(name, npi);
        if (result.IsFailure) return result;

        await _repository.Add(result.Value, ct);
        return result;
    }
}
