using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using NewApp.Services.ApiHost;

namespace NewApp.Tests.StepDefinitions;

/// <summary>
/// Encapsulates an in-memory instance of the ApiHost REST host, started via
/// <see cref="Microsoft.AspNetCore.TestHost"/>, so step definitions can call its endpoints
/// through the exposed <see cref="Client"/> using the same configuration as production.
/// </summary>
public sealed class ApiHostFixture : IDisposable
{
    private readonly WebApplication _app;

    public ApiHostFixture()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        Service.ConfigureBuilder(builder);

        _app = builder.Build();

        Service.ConfigurePipeline(_app, pathBase: string.Empty, isDevelopment: false);

        _app.StartAsync().GetAwaiter().GetResult();

        Client = _app.GetTestClient();
    }

    /// <summary>
    /// An <see cref="HttpClient"/> that routes requests to the in-memory ApiHost test server.
    /// </summary>
    public HttpClient Client { get; }

    public void Dispose()
    {
        Client.Dispose();
        _app.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}
