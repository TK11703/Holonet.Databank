var builder = DistributedApplication.CreateBuilder(args);

var apiBaseUrl = builder.ExecutionContext.IsPublishMode
    ? builder.Configuration["MyAppSettings:ApiGateway:BaseUrl"]
    : builder.AddProject<Projects.Holonet_Databank_API>("holonet-databank-api").GetEndpoint("http").ToString();


var web = builder.AddProject<Projects.Holonet_Databank_Web>("holonet-databank-web")
            .WithEnvironment("ApiBaseUrl", apiBaseUrl);

builder.AddAzureFunctionsProject<Projects.Holonet_Databank_AppFunctions>("holonet-databank-appfunctions");

await builder.Build().RunAsync();
