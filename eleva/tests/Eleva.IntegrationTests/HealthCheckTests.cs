namespace Eleva.IntegrationTests;

public class HealthCheckTests : IClassFixture<ElevaTestFactory>
{
    private readonly ElevaTestFactory _factory;

    public HealthCheckTests(ElevaTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/health");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("healthy", content);
    }
}
