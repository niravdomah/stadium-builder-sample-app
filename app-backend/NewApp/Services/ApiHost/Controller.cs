using Microsoft.AspNetCore.Mvc;

namespace NewApp.Services.ApiHost;

/// <summary>
/// Concrete controller for the ApiHost REST service, implementing the operations
/// defined by the generated <see cref="ControllerBase"/> from the OpenAPI specification.
/// </summary>
public sealed class Controller : ControllerBase
{
    public override Task<ActionResult<HealthResponse>> GetHealth(CancellationToken cancellationToken = default)
    {
        ActionResult<HealthResponse> result = Ok(new HealthResponse { Status = "Healthy" });
        return Task.FromResult(result);
    }
}
