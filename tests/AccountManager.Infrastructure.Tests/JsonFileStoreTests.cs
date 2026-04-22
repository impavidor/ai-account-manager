using System.Text.Json;
using AccountManager.Infrastructure;
using FluentAssertions;

namespace AccountManager.Infrastructure.Tests;

[TestFixture]
public class JsonFileStoreTests
{
    private string _tempDir = null!;
    private string _filePath = null!;

    [SetUp]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        _filePath = Path.Combine(_tempDir, "test.json");
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Test]
    public async Task LoadAllAsync_ValidJsonArray_ReturnsDtos()
    {
        await File.WriteAllTextAsync(_filePath, """[{"Name":"Alice"},{"Name":"Bob"}]""");
        var store = new JsonFileStore<TestDto>(_filePath);

        var result = await store.LoadAllAsync();

        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Alice");
        result[1].Name.Should().Be("Bob");
    }

    [Test]
    public async Task LoadAllAsync_FileMissing_ThrowsFileNotFoundException()
    {
        var store = new JsonFileStore<TestDto>(Path.Combine(_tempDir, "missing.json"));

        Func<Task> act = () => store.LoadAllAsync();

        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    [Test]
    public async Task LoadAllAsync_EmptyFile_ThrowsJsonException()
    {
        await File.WriteAllTextAsync(_filePath, "");
        var store = new JsonFileStore<TestDto>(_filePath);

        Func<Task> act = () => store.LoadAllAsync();

        await act.Should().ThrowAsync<JsonException>();
    }

    [Test]
    public async Task LoadAllAsync_MalformedJson_ThrowsJsonException()
    {
        await File.WriteAllTextAsync(_filePath, "{not valid json");
        var store = new JsonFileStore<TestDto>(_filePath);

        Func<Task> act = () => store.LoadAllAsync();

        await act.Should().ThrowAsync<JsonException>();
    }

    [Test]
    public async Task SaveAllAsync_ThenLoadAllAsync_RoundTripsData()
    {
        var store = new JsonFileStore<TestDto>(_filePath);
        var items = new List<TestDto> { new("Alice"), new("Bob") };

        await store.SaveAllAsync(items);
        var result = await store.LoadAllAsync();

        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Alice");
        result[1].Name.Should().Be("Bob");
    }

    [Test]
    public async Task SaveAllAsync_DirectoryMissing_ThrowsDirectoryNotFoundException()
    {
        var store = new JsonFileStore<TestDto>(Path.Combine(_tempDir, "nonexistent", "test.json"));

        Func<Task> act = () => store.SaveAllAsync(new List<TestDto> { new("X") });

        await act.Should().ThrowAsync<DirectoryNotFoundException>();
    }
}

file record TestDto(string Name);
