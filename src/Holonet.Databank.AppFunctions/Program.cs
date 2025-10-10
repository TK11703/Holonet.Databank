using Holonet.Databank.AppFunctions.Configuration;
using Holonet.Databank.AppFunctions.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

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
        services.Configure<AppSettings>(context.Configuration.GetSection("AppSettings"));        
        if (context.HostingEnvironment.IsDevelopment())
        {
            try
            {
                var azuritePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"npm","azurite.cmd");
                Process.Start(new ProcessStartInfo
                {
                    FileName = azuritePath,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
                Console.WriteLine("Azurite started.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start Azurite: {ex.Message}");
            }
        }
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