using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Company.Function.Models;

// CosmosDb .NET SDK v2
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

// CosmosDb .NET SDK v3
using Microsoft.Azure.Cosmos;

namespace Company.Function
{
    public class GetDataFromCosmosDb
    {

        ///
        /// .NET SDK v2 Approach
        /// https://docs.microsoft.com/en-us/azure/cosmos-db/sql/find-request-unit-charge?tabs=dotnetv2
        ///
        [FunctionName("SDKv2")]
        public async Task<IActionResult> SDKv2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "cosmosdb", collectionName: "posts", ConnectionStringSetting="CosmosDBConnection")] DocumentClient client,
            ILogger log)
        {
            double totalRU = 0;
            log.LogInformation("C# HTTP trigger function processed a request.");

            string creatorId = req.Query["CreatorId"];
            log.LogInformation($"Searching for: {creatorId}");

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("cosmosdb", "posts");
            IDocumentQuery<PostsModel> query = client.CreateDocumentQuery<PostsModel>(collectionUri)
                .Where(p => p.CreatorId == creatorId)
                .OrderByDescending(p => p.CreatedOn)
                .AsDocumentQuery();
                        
            while (query.HasMoreResults)
            {
                Microsoft.Azure.Documents.Client.FeedResponse<PostsModel> queryResult = await query.ExecuteNextAsync<PostsModel>();
                totalRU += queryResult.RequestCharge;
                foreach (PostsModel result in queryResult)
                {
                    log.LogInformation(result.Id);
                }
            }

            string resultMessage = $"RU cost for creatorId='{creatorId}' is '{totalRU}' units.";
            return new OkObjectResult(resultMessage);
        }



        ///
        /// .NET SDK v3 Approach
        /// https://docs.microsoft.com/en-us/azure/cosmos-db/sql/find-request-unit-charge?tabs=dotnetv3
        ///

        private readonly CosmosClient cosmosClient;
        private readonly Container container;
        public GetDataFromCosmosDb(CosmosClient cosmosClient)
        {
            this.cosmosClient = cosmosClient;
            this.container = cosmosClient.GetContainer("cosmosdb", "posts");
        }

        [FunctionName("SDKv3")]
        public async Task<IActionResult> SDKv3(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            double totalRU = 0;
            log.LogInformation("C# HTTP trigger function processed a request.");

            string creatorId = req.Query["CreatorId"];
            log.LogInformation($"Searching for: {creatorId}");
       
            QueryDefinition queryDefinition = new QueryDefinition("select * From c where c.CreatorId = @CreatorId").WithParameter("@CreatorId", creatorId);
            using (Microsoft.Azure.Cosmos.FeedIterator<PostsModel> feedIterator = this.container.GetItemQueryIterator<PostsModel>(queryDefinition))
            {
                while (feedIterator.HasMoreResults)
                {
                    Microsoft.Azure.Cosmos.FeedResponse<PostsModel> response = await feedIterator.ReadNextAsync();
                    totalRU += response.RequestCharge;
                    foreach (var item in response)
                    {
                        log.LogInformation(item.Id);
                    }
                }
            }

            string resultMessage = $"RU cost for creatorId='{creatorId}' is '{totalRU}' units.";
            return new OkObjectResult(resultMessage);
        }
    }
}
