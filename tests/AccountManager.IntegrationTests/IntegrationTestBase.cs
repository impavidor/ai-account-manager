using Microsoft.AspNetCore.Mvc.Testing;

namespace AccountManager.IntegrationTests;

public abstract class IntegrationTestBase
{
    protected WebApplicationFactory<Program> Factory { get; private set; } = null!;
    protected HttpClient Client { get; private set; } = null!;
    protected string DataPath { get; private set; } = null!;

    [OneTimeSetUp]
    public void BaseOneTimeSetUp()
    {
        DataPath = Path.Combine(Path.GetTempPath(), $"am-integration-{Guid.NewGuid()}");
        Directory.CreateDirectory(DataPath);

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("DataPath", DataPath);
            });

        Client = Factory.CreateClient();
    }

    [OneTimeTearDown]
    public void BaseOneTimeTearDown()
    {
        Client.Dispose();
        Factory.Dispose();

        if (Directory.Exists(DataPath))
            Directory.Delete(DataPath, recursive: true);
    }
}
