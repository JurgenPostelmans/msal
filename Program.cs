using System;
using System.Threading.Tasks; 
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Text.Json;

namespace msal
{
    class Program
    {
        private static string clientId = "269dfb4f-1e9d-43b5-b483-edb328ca7095";
        private static string clientSecret = "KDc7Q~E9gHyHy35riNNmbp8DVfJR~qnwv~S1o";
        private static string authorityUri = "https://login.microsoftonline.com/c13f6e78-545e-4878-a61a-4887d80c3f28";
        private static string redirectUri = "http://localhost";
        //private static string[] scopes = { "https://u2u365.crm4.dynamics.com/user_impersonation" };

        //Client credential flows must have a scope value with /.default suffixed to the resource identifier (application ID URI)
        private static string[] scopes = { "https://u2u365.crm4.dynamics.com/.default" };

        static async Task Main(string[] args)
        {
            AuthenticationResult ar;

            /*
            IPublicClientApplication app;

            app = PublicClientApplicationBuilder.Create(clientId)
                .WithAuthority(authorityUri)
                .WithRedirectUri(redirectUri)
                .Build();

            //Retrieve any cached tokens
            var accounts = await app.GetAccountsAsync();
            IAccount account = accounts.FirstOrDefault();

            ar = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            */

            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithAuthority(authorityUri)
                .WithRedirectUri(redirectUri)
                .WithClientSecret(clientSecret )
                .Build();

            ar = await app.AcquireTokenForClient(scopes).ExecuteAsync();

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ar.AccessToken);

            httpClient.BaseAddress = new Uri("https://u2u365.crm4.dynamics.com/api/data/v9.2/");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,"contacts?$select=fullname");

            HttpResponseMessage response = await httpClient.SendAsync(request);

            string data = await response.Content.ReadAsStringAsync();

            Console.WriteLine(data);

            Console.ReadLine();
        }
    }
}
