using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Api
{
    public  class SetUser
    {
        private readonly ILogger _logger;

        public SetUser(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SetUser>();
        }

        [Function("SetUser")]
        [CosmosDBOutput(databaseName: Constants.DATABASE_NAME,
            collectionName: Constants.USERS_TABLE_NAME,
            ConnectionStringSetting = Constants.CONNECTION_SETTINGS_KEY)]
        public async Task<object> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "put")] HttpRequestData req,
            [FromBody] User user)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return user;
        }
    }
}
