using Twenty57.Builder.ApplicationRuntime.Interfaces;

namespace NewApp;

public sealed class AppSettings : IAppSettings
{
    /// <summary>
    /// The URI on which the ApiHost REST service listens. May include a path base (e.g. http://0.0.0.0:3000/v1).
    /// </summary>
    public string ApiHostUri { get; set; } = "http://0.0.0.0:3000";
}
