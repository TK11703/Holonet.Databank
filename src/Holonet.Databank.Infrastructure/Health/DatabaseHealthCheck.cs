using Holonet.Databank.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace Holonet.Databank.Infrastructure.Health;
public class DatabaseHealthCheck(ISqlDataAccess sqlDataAccess) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            bool completed = await sqlDataAccess.ExecuteHealthCheckAsync();
            if (completed)
            {
                return HealthCheckResult.Healthy();
            }
            return HealthCheckResult.Unhealthy();
        }
        catch(Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}
