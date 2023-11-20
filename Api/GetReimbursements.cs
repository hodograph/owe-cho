using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    public class GetReimbursements
    {
        private readonly ILogger _logger;

        public GetReimbursements(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetReimbursements>();
        }

        [Function("GetReimbursements")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "reimbursements/{tripId}")] HttpRequestData req,
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.REIMBURSEMENTS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY,
            SqlQuery = "SELECT * FROM Reimbursements r where r.TripId = {tripId}")] List<Reimbursement> reimbursements,
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
                response.WriteAsJsonAsync(reimbursements);
            }
            else
            {
                response = req.CreateResponse(HttpStatusCode.NotFound);
            }

            return response;
        }
    }
}
