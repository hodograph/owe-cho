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
    public class SetReimbursement : NotificationController
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
            SqlQuery = "SELECT * FROM Trips t where t.id = {tripId}")] List<Trip> trips, 
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.REIMBURSEMENTS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY,
            SqlQuery = "SELECT * FROM Reimbursements r where r.TripId = {tripId}")] List<Reimbursement> reimbursements,
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.SUBSCRIPTIONS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY)] List<NotificationSubscription> subscriptions)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            Trip trip = trips.FirstOrDefault();
            HttpResponseData response;

            if (reimbursement.TripId == default)
            {
                reimbursement.TripId = trip.id;
            }

            Reimbursement oldReimbursement = reimbursements.FirstOrDefault(x => x.id == reimbursement.id);

            ClaimsPrincipal claim = ClaimsPrincipalParser.ParsePrincipal(req);
            if (trip.SharedWith.Append(trip.Owner).Contains(claim.Identity.Name) && reimbursement.TripId == trip.id)
            {
                // if the payer isn't who submitted the request then notify the payer.
                if (reimbursement.Payer != claim.Identity.Name)
                {
                    foreach(NotificationSubscription subscription in subscriptions.Where(x => x.UserId == reimbursement.Payer))
                    {
                        if(oldReimbursement != null)
                        {
                            if (oldReimbursement.Confirmed == false && reimbursement.Confirmed == true)
                            {
                                await SendNotificationAsync(trip, subscription, $"Reimbursement Confirmed");
                            }
                            else
                            {
                                await SendNotificationAsync(trip, subscription, $"Reimbursement updated");
                            }
                        }
                        else
                        {
                            await SendNotificationAsync(trip, subscription, $"You reimbursed someone!");
                        }
                    }
                }
                // if the recipient isn't who submitted the request then notify the recipient.
                else if(reimbursement.Recipient != claim.Identity.Name)
                {
                    foreach (NotificationSubscription subscription in subscriptions.Where(x => x.UserId == reimbursement.Payer))
                    {
                        if (oldReimbursement != null)
                        {
                            if (oldReimbursement.Confirmed == false && reimbursement.Confirmed == true)
                            {
                                await SendNotificationAsync(trip, subscription, $"Reimbursement Confirmed");
                            }
                            else
                            {
                                await SendNotificationAsync(trip, subscription, $"Reimbursement updated");
                            }
                        }
                        else
                        {
                            await SendNotificationAsync(trip, subscription, $"You've been reimbursed!");
                        }
                    }
                }

                return reimbursement;
            }
            else
            {
                return null;
            }
        }
    }
}
