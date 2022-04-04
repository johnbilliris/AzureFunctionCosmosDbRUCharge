using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Cosmos;

namespace Company.Function
{

  /*
   * Custom RequestHandler that enriches the telemetry with the CosmosDB RU
   * Charge and the Db query executed.
   * https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.requesthandler?view=azure-dotnet
   */
    public class DependencyTelemetryRequestHandler : RequestHandler 
    {
        private readonly TelemetryClient telemetryClient;

        private const string RequestCharge = "CosmosDBRequestCharge";

        public DependencyTelemetryRequestHandler()
        {
            this.telemetryClient = new TelemetryClient();
        }
        public DependencyTelemetryRequestHandler(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        
        public override async Task<ResponseMessage> SendAsync(RequestMessage request, CancellationToken cancellationToken)
        {
            using (Microsoft.ApplicationInsights.Extensibility.IOperationHolder<DependencyTelemetry> operation = this.telemetryClient.StartOperation<DependencyTelemetry>("CosmosDBRequest"))
            {
                this.telemetryClient.TrackTrace($"{request.Method.Method} - {request.RequestUri.ToString()}");
                ResponseMessage response = await base.SendAsync(request, cancellationToken);

                var telemetry = operation.Telemetry;
                telemetry.Type = "Azure DocumentDB";
                telemetry.Data = request.RequestUri.OriginalString;
                telemetry.ResultCode = ((int)response.StatusCode).ToString();
                telemetry.Success = response.IsSuccessStatusCode;
                
                telemetry.Metrics[RequestCharge] = response.Headers.RequestCharge;

                if ( !telemetry.Properties.ContainsKey("CosmosDBQuery") ) {
                    if ( response.RequestMessage is not null ) {
                        if ( response.RequestMessage.Content is not null ) {
                            using (StreamReader reader = new StreamReader( response.RequestMessage.Content ))
                            {
                                var cosmosQuery = reader.ReadToEnd();
                                telemetry.Properties.Add("CosmosDBQuery",cosmosQuery );
                            }
                        }
                    }
                }
                
                this.telemetryClient.StopOperation(operation);
                return response;
            }
        }
    }
}