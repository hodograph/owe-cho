using System.Collections.Generic;
using System.Net;
using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class GetTrips
    {
        private readonly ILogger _logger;

        public GetTrips(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetTrips>();
        }

        [Function("GetTrips")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="trips/{email}")] HttpRequestData req,
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.TRIPS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY,
            SqlQuery = "SELECT * FROM Trips t where t.Owner = {email} OR ARRAY_CONTAINS(t.SharedWith, {email})")] IEnumerable<Trip> trips)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteAsJsonAsync(trips);

            return response;
        }
    }
}
