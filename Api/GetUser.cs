using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Api
{
    public class GetUser
    {
        private readonly ILogger _logger;

        public GetUser(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetUser>();
        }

        [Function("GetUser")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{email}")] HttpRequestData req,
            [CosmosDBInput(databaseName: Constants.DATABASE_NAME,
            collectionName:Constants.USERS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY,
            SqlQuery = "SELECT * FROM Users t where t.email = {email}")] List<User> users)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            HttpResponseData response;

            User user = users.FirstOrDefault();

            ClaimsPrincipal claim = ClaimsPrincipalParser.ParsePrincipal(req);
            if (user.email == claim.Identity.Name)
            {
                response = req.CreateResponse(HttpStatusCode.OK);
                response.WriteAsJsonAsync(user);
            }
            else
            {
                response = req.CreateResponse(HttpStatusCode.NotFound);
            }

            return response;
        }
    }
}
