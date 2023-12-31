﻿using Azure;
using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace Api
{
    public class GetUsers
    {
        private readonly ILogger _logger;

        public GetUsers(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetUsers>();
        }

        [Function("GetUsers")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")] HttpRequestData req,
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.USERS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY,
            SqlQuery = "SELECT * FROM Users")] List<User> users)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            HttpResponseData response;

            ClaimsPrincipal claim = ClaimsPrincipalParser.ParsePrincipal(req);
            if (!string.IsNullOrWhiteSpace(claim.Identity.Name))
            {
                response = req.CreateResponse(HttpStatusCode.OK);
                response.WriteAsJsonAsync(users);
            }
            else
            {
                response = req.CreateResponse(HttpStatusCode.NotFound);
            }

            return response;
        }
    }
}
