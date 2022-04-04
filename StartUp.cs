using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using Company.Function;

[assembly: FunctionsStartup(typeof(StartUp))]
namespace Company.Function
{
    internal class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // For the Telemetry Initializer to grab the RU charge per call to CosmosDB
            /// .NET SDK v2 Approach
            builder.Services.AddSingleton<ITelemetryInitializer, DependencyTelemetryInitializer>();

            /// .NET SDK v3 Approach
            builder.Services.AddSingleton((s) => {
                string connectionString = Environment.GetEnvironmentVariable("CosmosDBConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentNullException("Please specify a valid CosmosDBConnection in the appSettings.json file or your Azure Functions Settings.");
                }

                CosmosClientBuilder configurationBuilder = new CosmosClientBuilder(connectionString);
                configurationBuilder.AddCustomHandlers( new DependencyTelemetryRequestHandler() );
                return configurationBuilder.Build();
            });
        }
    }
}