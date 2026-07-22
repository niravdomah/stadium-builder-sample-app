using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Twenty57.Builder.ApplicationRuntime.Interfaces;

namespace NewApp.Services.ApiHost;

/// <summary>
/// Hosts the ApiHost REST service. The service can be started and stopped independently
/// and listens on the URI configured via <see cref="AppSettings.ApiHostUri"/>.
/// </summary>
public sealed class Service(AppSettings appSettings, IHostEnvironment environment, ILogger<Service> logger) : IService
{
    private const string SwaggerSpecPath = "/swagger/v1/swagger.yaml";
    private const string EmbeddedSpecResourceName = "NewApp.ApiHost.yaml";

    private readonly AppSettings _appSettings = appSettings;
    private readonly IHostEnvironment _environment = environment;
    private readonly ILogger<Service> _logger = logger;

    private WebApplication? _app;

    public bool IsRunning { get; private set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var uri = new Uri(_appSettings.ApiHostUri);
        string schemeAndAuthority = uri.GetLeftPart(UriPartial.Authority);
        string pathBase = uri.AbsolutePath.TrimEnd('/');

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls(schemeAndAuthority);
        ConfigureBuilder(builder);

        _app = builder.Build();
        ConfigurePipeline(_app, pathBase, _environment.IsDevelopment());

        await _app.StartAsync(cancellationToken);
        IsRunning = true;
        _logger.LogInformation("ApiHost service started at {Uri}.", _appSettings.ApiHostUri);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_app is not null)
        {
            await _app.StopAsync(cancellationToken);
            await _app.DisposeAsync();
            _app = null;
        }

        IsRunning = false;
        _logger.LogInformation("ApiHost service stopped.");
    }

    /// <summary>
    /// Registers the services required by the ApiHost REST host. Shared by production hosting and tests.
    /// </summary>
    internal static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers()
            .AddApplicationPart(typeof(Service).Assembly);
    }

    /// <summary>
    /// Configures the ApiHost request pipeline. Shared by production hosting and tests.
    /// The path base is applied before any other middleware so routing works correctly.
    /// </summary>
    internal static void ConfigurePipeline(WebApplication app, string pathBase, bool isDevelopment)
    {
        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);
        }

        // Expose the Swagger UI and its OpenAPI spec only in the Development environment.
        if (isDevelopment)
        {
            ConfigureSwagger(app);
        }

        app.MapControllers();
    }

    private static void ConfigureSwagger(WebApplication app)
    {
        // Swagger UI middleware is registered before authentication/authorization so its assets
        // are reachable anonymously even when a default authorization policy is applied.
        app.UseSwaggerUI(options => options.SwaggerEndpoint(SwaggerSpecPath, "ApiHost"));

        // Serve the OpenAPI spec from the embedded docs/ApiHost.yaml file, accessible anonymously.
        app.MapGet(SwaggerSpecPath, () =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(EmbeddedSpecResourceName)
                ?? throw new InvalidOperationException($"Embedded OpenAPI spec '{EmbeddedSpecResourceName}' was not found.");
            using var reader = new StreamReader(stream);
            return Results.Text(reader.ReadToEnd(), "application/yaml");
        }).AllowAnonymous();
    }
}
