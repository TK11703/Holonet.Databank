using Holonet.Databank.AppFunctions.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

try
{
    var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        if (context.HostingEnvironment.IsDevelopment())
        {
            config.AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, $"local.settings.json"), optional: true, reloadOnChange: false);
            config.AddUserSecrets<Program>();
        }
        config.AddEnvironmentVariables();
        config.AddCommandLine(args);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddLogging();
        services.ConfigureClients(context.Configuration);
    })
    .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
    var telemetryClient = new TelemetryClient(telemetryConfiguration);
    telemetryClient.TrackException(ex);
    telemetryClient.Flush();
    throw;
}