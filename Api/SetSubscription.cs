using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    public class SetSubscription
    {
        private readonly ILogger _logger;

        public SetSubscription(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SetSubscription>();
        }

        [Function("subscribe")]
        [CosmosDBOutput(databaseName: Constants.DATABASE_NAME,
            collectionName: Constants.SUBSCRIPTIONS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY)]
        public async Task<object> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put")] HttpRequestData req,
            [FromBody] NotificationSubscription subscription)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            HttpResponseData response;

            ClaimsPrincipal claim = ClaimsPrincipalParser.ParsePrincipal(req);
            if (subscription.UserId == claim.Identity.Name)
            {
                return subscription;
            }
            else
            {
                return null;
            }
        }
    }
}
