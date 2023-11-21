using BlazorApp.Shared;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebPush;

namespace Api
{
    public class NotificationController
    {
        protected async Task SendNotificationAsync(Trip trip, NotificationSubscription subscription, string message)
        {
            string privateKey = Environment.GetEnvironmentVariable("notiPrivate");
            string publicKey = Environment.GetEnvironmentVariable("notiPublic");

            var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
            var vapidDetails = new VapidDetails("mailto:jjdeanman@gmail.com", publicKey, privateKey);
            var webPushClient = new WebPushClient();
            try
            {
                var payload = JsonSerializer.Serialize(new
                {
                    message,
                    url = $"Trip/{trip.id}",
                });
                await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
            }
            catch(WebPushException webEx)
            {
                // TODO: Delete bad subscriptions
                Console.Error.WriteLine("Error sending push notification: " + webEx.Message);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error sending push notification: " + ex.Message);
            }
        }
    }
}
