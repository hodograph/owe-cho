using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api
{
    public class SetTransaction
    {
        private readonly ILogger _logger;

        public SetTransaction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SetTransaction>();
        }

        [Function("SetTransaction")]
        [CosmosDBOutput(databaseName: Constants.DATABASE_NAME,
            collectionName: Constants.TRANSACTIONS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY)]
        public async Task<object> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "SetTransaction/{tripId}")] HttpRequestData req,
            [FromBody] Transaction transaction,
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.TRIPS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY,
            SqlQuery = "SELECT * FROM Trips t where t.id = {tripId}")] List<Trip> trips)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            Trip trip = trips.FirstOrDefault();
            HttpResponseData response;

            if(transaction.TripId == default)
            {
                transaction.TripId = trip.id;
            }

            ClaimsPrincipal claim = ClaimsPrincipalParser.ParsePrincipal(req);
            if (trip.SharedWith.Append(trip.Owner).Contains(claim.Identity.Name) && transaction.TripId == trip.id)
            {
                return transaction;
            }
            else
            {
                return null;
            }
        }
    }
}
