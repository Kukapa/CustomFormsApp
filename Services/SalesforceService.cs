using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CustomFormsApp.Services
{
    public class SalesforceService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _username;
        private readonly string _password;
        private readonly string _securityToken;
        private string _accessToken;
        private string _instanceUrl;

        public SalesforceService(IConfiguration configuration)
        {
            var salesforceConfig = configuration.GetSection("Salesforce");
            _clientId = salesforceConfig["ClientId"];
            _clientSecret = salesforceConfig["ClientSecret"];
            _username = salesforceConfig["Username"];
            _password = salesforceConfig["Password"];
            _securityToken = salesforceConfig["SecurityToken"];
        }

        public async Task AuthenticateAsync()
        {
            var client = new RestClient("https://login.salesforce.com");
            var request = new RestRequest("/services/oauth2/token", Method.Post);
            request.AddParameter("grant_type", "password");
            request.AddParameter("client_id", _clientId);
            request.AddParameter("client_secret", _clientSecret);
            request.AddParameter("username", _username);
            request.AddParameter("password", _password + _securityToken);

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Could not authenticate with Salesforce: {response.Content}");
            }

            using var document = JsonDocument.Parse(response.Content);
            var root = document.RootElement;
            _accessToken = root.GetProperty("access_token").GetString();
            _instanceUrl = root.GetProperty("instance_url").GetString();
        }

        public async Task CreateAccountAndContactAsync(string accountName, string contactFirstName, string contactLastName, string contactEmail)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                await AuthenticateAsync();
            }

            var client = new RestClient(_instanceUrl);
            var request = new RestRequest("/services/data/v56.0/sobjects/Account", Method.Post);
            request.AddHeader("Authorization", $"Bearer {_accessToken}");
            request.AddJsonBody(new { Name = accountName });

            var accountResponse = await client.ExecuteAsync(request);
            if (!accountResponse.IsSuccessful)
            {
                throw new Exception($"Error creating Salesforce Account: {accountResponse.Content}");
            }

            using var accountDocument = JsonDocument.Parse(accountResponse.Content);
            var accountRoot = accountDocument.RootElement;
            string accountId = accountRoot.GetProperty("id").GetString();

            var contactRequest = new RestRequest("/services/data/v56.0/sobjects/Contact", Method.Post);
            contactRequest.AddHeader("Authorization", $"Bearer {_accessToken}");
            contactRequest.AddJsonBody(new
            {
                FirstName = contactFirstName,
                LastName = contactLastName,
                Email = contactEmail,
                AccountId = accountId
            });

            var contactResponse = await client.ExecuteAsync(contactRequest);
            if (!contactResponse.IsSuccessful)
            {
                throw new Exception($"Error creating Salesforce Contact: {contactResponse.Content}");
            }
        }
    }
}
