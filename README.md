# Inject Cosmos DB RU charge into Application Insights telemetry from an Azure Function

This repo contains sample code to extend the Application Insights telemetry being sent to include the RU charge and the 'SQL query' of a CosmosDB call.

## Background

A recent challenge from a customer was brought to my attention regarding how to view the Azure Cosmos DB Request Unit (RU) charge in the telemetry being sent to Azure Application Insights from their Azure Function (written using the C# .NET language). While there is the ability to send diagnostics from the Cosmos DB into the same Log Analytics Workspace and query and join both tables in KQL, I was keen on solving the challenge by visualising the information in Application Insights.

DISCLAIMER: The following telemetry injection approach has been validated with the Cosmos DB SDK v2. Validation with the Cosmos DB SDK v3 is in progress, and I will provide an update when completed.

## Getting Started

### Code Structure

This Azure Function sample is written in C# and uses Visual Studio Code.

### Packages Required

dotnet add package Microsoft.Azure.Functions.Extensions --version 1.1.0
dotnet add package Microsoft.Azure.WebJobs.Extensions.CosmosDB --version 3.0.10 
dotnet add package Microsoft.ApplicationInsights --version 2.18.0
dotnet add package Microsoft.Azure.Cosmos --version 3.25.0

### Information

This is a sample application to show you how to use a custom Application Insights TelemetryInitializer in an Azure Function to enrich the telemetry with the Cosmos DB RU charge and the 'SQL' of the Cosmos DB call.


## Additional Reading

- [Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview) - Overview of Application Insights
- [ITelemetryInitializer Interface](https://docs.microsoft.com/en-us/dotnet/api/microsoft.applicationinsights.extensibility.itelemetryinitializer?view=azure-dotnet) - To learn more about the ITelemetryInitializer Interface
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-3.1) - Framework for building web applications