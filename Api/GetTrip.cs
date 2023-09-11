using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class GetTrip
    {
        private readonly ILogger _logger;

        public GetTrip(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetTrip>();
        }

        [Function("GetTrip")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "trip/{tripId}")] HttpRequestData req,
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.TRIPS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY,
            SqlQuery = "SELECT * FROM Trips t where t.id = {tripId}")] List<Trip> trips)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            Trip trip = trips.FirstOrDefault();

            HttpResponseData response;

            ClaimsPrincipal claim = ClaimsPrincipalParser.ParsePrincipal(req);
            if (trip.SharedWith.Append(trip.Owner).Contains(claim.Identity.Name))
            {
                response = req.CreateResponse(HttpStatusCode.OK);
                response.WriteAsJsonAsync(trips.FirstOrDefault());
            }
            else
            {
                response = req.CreateResponse(HttpStatusCode.NotFound);
            }

            return response;
        }
    }
}
