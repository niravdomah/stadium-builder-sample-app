using System.Net.Http.Json;
using NewApp.Services.ApiHost;
using Reqnroll;

namespace NewApp.Tests.StepDefinitions;

/// <summary>
/// Step definitions specific to the /health endpoint of the ApiHost service.
/// </summary>
[Binding]
public sealed class HealthStepDefinitions(HttpScenarioContext context)
{
    private readonly HttpScenarioContext _context = context;

    [Then("the reported health status is {string}")]
    public async Task ThenTheReportedHealthStatusIs(string expectedStatus)
    {
        Assert.NotNull(_context.Response);
        var body = await _context.Response!.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(body);
        Assert.Equal(expectedStatus, body!.Status);
    }
}
