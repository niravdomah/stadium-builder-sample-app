namespace NewApp.Tests.StepDefinitions;

/// <summary>
/// Holds state shared between step definition classes for the duration of a single scenario,
/// most notably the most recent HTTP response returned by the REST host under test.
/// </summary>
public sealed class HttpScenarioContext
{
    public HttpResponseMessage? Response { get; set; }
}
