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

            HttpResponseData response;

            ClaimsPrincipal claim = ClaimsPrincipalParser.ParsePrincipal(req);
            if (!string.IsNullOrWhiteSpace(claim.Identity.Name))
            {
                response = req.CreateResponse(HttpStatusCode.OK);
                response.WriteAsJsonAsync(trips.Where(x => x.SharedWith.Append(x.Owner).Contains(claim.Identity.Name)));
            }
            else
            {
                response = req.CreateResponse(HttpStatusCode.NotFound);
            }

            return response;
        }
    }
}
