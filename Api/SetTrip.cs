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
    public class SetTrip : NotificationController
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
        public async Task<object> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "SetTrip/{tripId}")] HttpRequestData req,
            [FromBody] Trip trip,
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.TRIPS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY,
            SqlQuery = "SELECT * FROM Trips t where t.id = {tripId}")] List<Trip> trips,
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.SUBSCRIPTIONS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY)] List<NotificationSubscription> subscriptions)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            Trip oldTrip = trips.FirstOrDefault();
            HttpResponseData response;

            ClaimsPrincipal claim = ClaimsPrincipalParser.ParsePrincipal(req);
            if (trip.SharedWith.Append(trip.Owner).Contains(claim.Identity.Name))
            {
                foreach (string newEmail in trip.SharedWith.Where(x => oldTrip?.SharedWith.All(y => y != x) ?? true))
                {
                    foreach (NotificationSubscription subscription in subscriptions.Where(x => x.UserId == newEmail))
                    {
                        await SendNotificationAsync(trip, subscription, $"{trip.Owner} added you to trip {trip.Name}");
                    }
                }

                return trip;
            }
            else
            {
                return null;
            }
        }
    }
}
