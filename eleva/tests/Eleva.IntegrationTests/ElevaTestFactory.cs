namespace Eleva.IntegrationTests;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;

public class ElevaTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection", "Data Source=:memory:");
        builder.UseEnvironment("Testing");
    }
}
