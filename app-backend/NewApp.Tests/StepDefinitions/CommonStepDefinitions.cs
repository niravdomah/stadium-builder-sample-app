using Reqnroll;

namespace NewApp.Tests.StepDefinitions;

/// <summary>
/// Step definitions shared across feature files (generic HTTP request/response steps).
/// </summary>
[Binding]
public sealed class CommonStepDefinitions(ApiHostFixture apiHost, HttpScenarioContext context)
{
    private readonly ApiHostFixture _apiHost = apiHost;
    private readonly HttpScenarioContext _context = context;

    [When("I send a GET request to {string}")]
    public async Task WhenISendAGetRequestTo(string path)
    {
        _context.Response = await _apiHost.Client.GetAsync(path);
    }

    [Then("the response status code is {int}")]
    public void ThenTheResponseStatusCodeIs(int statusCode)
    {
        Assert.NotNull(_context.Response);
        Assert.Equal(statusCode, (int)_context.Response!.StatusCode);
    }
}
