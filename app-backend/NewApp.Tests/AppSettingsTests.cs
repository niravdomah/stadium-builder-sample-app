namespace NewApp.Tests;

public sealed class AppSettingsTests
{
    [Fact]
    public void ApiHostUri_defaults_to_all_interfaces_on_port_3000()
    {
        var settings = new NewApp.AppSettings();

        Assert.Equal("http://0.0.0.0:3000", settings.ApiHostUri);
    }
}
