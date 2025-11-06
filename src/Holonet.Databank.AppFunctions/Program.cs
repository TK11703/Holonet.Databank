using Holonet.Databank.AppFunctions.Configuration;
using Holonet.Databank.AppFunctions.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

try
{    
    logger.LogInformation("Starting Holonet.Databank.AppFunctions...");
    var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        if (context.HostingEnvironment.IsDevelopment())
        {
            config.AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, $"local.settings.json"), optional: true, reloadOnChange: false);
            config.AddUserSecrets<Program>();
            logger.LogInformation("Development environment detected. Loaded local.settings.json and user secrets.");
        }
        config.AddEnvironmentVariables();
        config.AddCommandLine(args);
        logger.LogInformation("Configuration sources have been set up.");
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
        logger.LogInformation("Services have been configured.");
    })
    .Build();
    
    await host.RunAsync();
}
catch (Exception ex)
{
    logger.LogError(ex, "Application startup failed: {Message}", ex.Message);
    logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
    var connectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
    var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
    telemetryConfiguration.ConnectionString = connectionString;
    var telemetryClient = new TelemetryClient(telemetryConfiguration);
    telemetryClient.Context.Cloud.RoleName = "Holonet-Func-Databank-8108";
    telemetryClient.TrackException(ex);
    telemetryClient.Flush();

    // Rethrow with contextual information to address S2139 and S112
    throw new InvalidOperationException("Holonet.Databank.AppFunctions failed to start. See inner exception for details.", ex);
}