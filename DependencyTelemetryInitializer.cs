using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Company.Function
{

  /*
   * Custom TelemetryInitializer that enriches the telemetry with the CosmosDB RU
   * Charge and the Db query executed.
   */
    public class DependencyTelemetryInitializer : ITelemetryInitializer
    {
        private const string RequestChargeHeaderName = "x-ms-request-charge";
        private const string RequestCharge = "CosmosDBRequestCharge";
        public void Initialize(ITelemetry telemetry)
        {
            if ( !(telemetry is DependencyTelemetry dependencyTelemetry) ) { return;}
            if ( !(dependencyTelemetry.Type == "Azure DocumentDB") ) { return; }
            if ( !dependencyTelemetry.TryGetOperationDetail("HttpResponse", out var responseObject) ) { return; }
            if ( !(responseObject is HttpResponseMessage response) ) { return; }
        
            if (response.Headers == null
                || !response.Headers.TryGetValues(RequestChargeHeaderName, out var requestChargeHeaderValue))
            {
                return;
            }

            if (double.TryParse(requestChargeHeaderValue.FirstOrDefault(), out var requestCharge))
            {
                if ( !dependencyTelemetry.Metrics.ContainsKey(RequestCharge) ) {
                    dependencyTelemetry.Metrics.Add(RequestCharge, requestCharge);
                }
            }

            if ( response.RequestMessage is not null ) {
                if ( response.RequestMessage.Content is not null ) {
                    var cosmosQuery = response.RequestMessage.Content.ReadAsStringAsync().Result;
                    if ( !dependencyTelemetry.Properties.ContainsKey("CosmosDBQuery") ) {
                        dependencyTelemetry.Properties.Add("CosmosDBQuery",cosmosQuery );
                    }
                }
            }
        }
    }
}
