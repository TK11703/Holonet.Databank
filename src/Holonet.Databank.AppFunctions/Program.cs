using Holonet.Databank.AppFunctions.Configuration;
using Holonet.Databank.AppFunctions.Extensions;
using Holonet.Databank.AppFunctions.Middleware;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Drawing;

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
    .ConfigureFunctionsWorkerDefaults(worker =>
    {
        // register middleware in the worker pipeline
        worker.UseMiddleware<AiInvocationMiddleware>();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<AppSettings>(context.Configuration.GetSection("AppSettings"));

        services.AddApplicationInsightsTelemetryWorkerService(configure => 
        {
            configure.ConnectionString = context.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
        });
        services.ConfigureFunctionsApplicationInsights();
        services.AddLogging();

        // register TelemetryClient
        services.AddSingleton(_ =>
        {
            var config = TelemetryConfiguration.CreateDefault();
            config.ConnectionString = context.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
            return new TelemetryClient(config);
        });

        // register your middleware (see next step)
        services.AddSingleton<AiInvocationMiddleware>();

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