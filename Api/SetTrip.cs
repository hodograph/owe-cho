using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorApp.Shared;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class SetTrip
    {
        private readonly ILogger _logger;

        public SetTrip(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SetTrip>();
        }

        [Function("SetTrip")]
        [CosmosDBOutput(databaseName: Constants.DATABASE_NAME,
            collectionName: Constants.TRIPS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY)]
        public async Task<object> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put")] HttpRequestData req,
            [FromBody] Trip trip)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            HttpResponseData response;

            ClaimsPrincipal claim = ClaimsPrincipalParser.ParsePrincipal(req);
            if (trip.SharedWith.Append(trip.Owner).Contains(claim.Identity.Name))
            {
                return trip;
            }
            else
            {
                return null;
            }
        }
    }
}
