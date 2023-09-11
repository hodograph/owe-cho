using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api
{
    public static class ClaimsPrincipalParser
    {
        /// <summary>
        /// Code below originally from Microsoft Docs - https://docs.microsoft.com/en-gb/azure/static-web-apps/user-information?tabs=csharp#api-functions
        /// </summary>
        /// <param name="req">The HttpRequestData header.</param>
        /// <returns>Parsed ClaimsPrincipal from 'x-ms-client-principal' header.</returns>
        public static ClaimsPrincipal ParsePrincipal(this HttpRequestData req)
        {
            var principal = ParseClient(req);

            principal.UserRoles = principal.UserRoles?.Except(new[] { "anonymous" }, StringComparer.CurrentCultureIgnoreCase);

            if (!principal.UserRoles?.Any() ?? true)
            {
                return new ClaimsPrincipal();
            }

            var identity = new ClaimsIdentity(principal.IdentityProvider);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principal.UserId));
            identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserDetails));
            identity.AddClaims(principal.UserRoles.Select(r => new Claim(ClaimTypes.Role, r)));

            return new ClaimsPrincipal(identity);
        }

        public static ClientPrincipal ParseClient(this HttpRequestData req)
        {
            ClientPrincipal principal = new ClientPrincipal();

            if (req.Headers.TryGetValues("x-ms-client-principal", out var header))
            {
                var data = header.First();
                var decoded = Convert.FromBase64String(data);
                var json = Encoding.UTF8.GetString(decoded);
                principal = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }

            return principal;
        }
    }
}
