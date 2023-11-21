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
    public class SetTransaction : NotificationController
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
            SqlQuery = "SELECT * FROM Trips t where t.id = {tripId}")] List<Trip> trips,
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.TRANSACTIONS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY,
            SqlQuery = "SELECT * FROM Transactions t where t.TripId = {tripId}")] List<Transaction> transactions,
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.SUBSCRIPTIONS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY)] List<NotificationSubscription> subscriptions)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            Trip trip = trips.FirstOrDefault();
            Transaction oldTransaction = transactions.FirstOrDefault(x => x.id == transaction.id);
            HttpResponseData response;

            if(transaction.TripId == default)
            {
                transaction.TripId = trip.id;
            }

            ClaimsPrincipal claim = ClaimsPrincipalParser.ParsePrincipal(req);
            if (trip.SharedWith.Append(trip.Owner).Contains(claim.Identity.Name) && transaction.TripId == trip.id)
            {
                List<Debt> debts = transaction.CalculatePayments();
                string message = $"Transaction {transaction.TransactionName} updated: ";
                if (oldTransaction == null)
                {
                    message = $"New transaction {transaction.TransactionName}: ";
                }

                foreach (string email in transaction.Debts.Select(x => x.Debtor))
                {
                    if (email != claim.Identity.Name)
                    {
                        foreach (NotificationSubscription subscription in subscriptions.Where(x => x.UserId == email))
                        {
                            message += $"You owe ${debts.FirstOrDefault(x => x.Debtor == email).Amount}";
                            await SendNotificationAsync(trip, subscription, message);
                        }
                    }
                }

                if (transaction.Payer != claim.Identity.Name)
                {
                    foreach (NotificationSubscription subscription in subscriptions.Where(x => x.UserId == transaction.Payer))
                    {
                        message += $"You're owed ${transaction.Total - debts.FirstOrDefault(x => x.Debtor == transaction.Payer).Amount}";
                        await SendNotificationAsync(trip, subscription, message);
                    }
                }

                return transaction;
            }
            else
            {
                return null;
            }
        }
    }
}
