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
            builder.Services.AddSingleton<ITelemetryInitializer, DependencyTelemetryInitializer>();          
        }
    }
}