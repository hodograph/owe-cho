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
    public class SetReimbursement
    {
        private readonly ILogger _logger;

        public SetReimbursement(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SetReimbursement>();
        }

        [Function("SetReimbursement")]
        [CosmosDBOutput(databaseName: Constants.DATABASE_NAME,
            collectionName: Constants.REIMBURSEMENTS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY)]
        public async Task<object> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "SetReimbursement/{tripId}")] HttpRequestData req,
            [FromBody] Reimbursement reimbursement,
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.TRIPS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY,
            SqlQuery = "SELECT * FROM Trips t where t.id = {tripId}")] List<Trip> trips)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            Trip trip = trips.FirstOrDefault();
            HttpResponseData response;

            if (reimbursement.TripId == default)
            {
                reimbursement.TripId = trip.id;
            }

            ClaimsPrincipal claim = ClaimsPrincipalParser.ParsePrincipal(req);
            if (trip.SharedWith.Append(trip.Owner).Contains(claim.Identity.Name) && reimbursement.TripId == trip.id)
            {
                return reimbursement;
            }
            else
            {
                return null;
            }
        }
    }
}
