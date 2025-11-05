using Holonet.Databank.AppFunctions.Configuration;
using Holonet.Databank.AppFunctions.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            Console.WriteLine("Development environment detected. Loaded local.settings.json and user secrets.");
        }
        config.AddEnvironmentVariables();
        config.AddCommandLine(args);
        Console.WriteLine("Configuration sources have been set up.");
    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.Configure<AppSettings>(context.Configuration.GetSection("AppSettings"));

        services.AddApplicationInsightsTelemetryWorkerService(configure => 
        {
            configure.ConnectionString = context.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
        });
        services.ConfigureFunctionsApplicationInsights();
        services.AddLogging(logging =>
        {
            logging.AddConsole(); // Optional: add other providers like Application Insights
        });
        services.ConfigureClients(context.Configuration);
        Console.WriteLine("Services have been configured.");
    })
    .Build();
    
    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"? Application startup failed: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");

    var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
    var telemetryClient = new TelemetryClient(telemetryConfiguration);
    telemetryClient.Context.Cloud.RoleName = "Holonet-Func-Databank-8108";
    telemetryClient.TrackException(ex);
    telemetryClient.Flush();
    throw;
}