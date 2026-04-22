using System.Text.Json;

namespace AccountManager.Infrastructure;

public class JsonFileStore<TDto>
{
    private readonly string _filePath;

    public JsonFileStore(string filePath) => _filePath = filePath;

    public async Task<IReadOnlyList<TDto>> LoadAllAsync(CancellationToken ct = default)
    {
        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"Data file not found: {_filePath}", _filePath);

        await using var stream = File.OpenRead(_filePath);
        var result = await JsonSerializer.DeserializeAsync<List<TDto>>(stream, cancellationToken: ct);
        return result ?? throw new JsonException($"Expected a JSON array in: {_filePath}");
    }

    public async Task SaveAllAsync(IReadOnlyList<TDto> items, CancellationToken ct = default)
    {
        var dir = Path.GetDirectoryName(_filePath);
        if (dir is not null && !Directory.Exists(dir))
            throw new DirectoryNotFoundException($"Data directory not found: {dir}");

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, items, cancellationToken: ct);
    }
}
