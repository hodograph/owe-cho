using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace Api
{
    public class GetTransactions
    {
        private readonly ILogger _logger;

        public GetTransactions(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetTransactions>();
        }

        [Function("GetTransactions")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "transactions/{tripId}")] HttpRequestData req,
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.TRANSACTIONS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY,
            SqlQuery = "SELECT * FROM Transactions t where t.TripId = {tripId}")] List<Transaction> transactions,
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
                response.WriteAsJsonAsync(transactions);
            }
            else
            {
                response = req.CreateResponse(HttpStatusCode.NotFound);
            }

            return response;
        }
    }
}
