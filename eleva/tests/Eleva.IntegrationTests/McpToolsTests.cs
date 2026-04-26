namespace Eleva.IntegrationTests;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;

public class McpToolsTests : IClassFixture<ElevaTestFactory>
{
    private readonly ElevaTestFactory _factory;

    public McpToolsTests(ElevaTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task McpTools_ReturnsList()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Instance-Id", "1");
        var response = await client.GetAsync("/mcp/tools");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("employee_list", content);
    }

    [Fact]
    public async Task McpTools_SystemHealth_ReturnsResult()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Instance-Id", "1");
        var payload = new StringContent(
            "{\"name\":\"system_health\",\"arguments\":{}}",
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await client.PostAsync("/mcp/tools/call", payload);
        Assert.True(response.IsSuccessStatusCode);
    }
}
