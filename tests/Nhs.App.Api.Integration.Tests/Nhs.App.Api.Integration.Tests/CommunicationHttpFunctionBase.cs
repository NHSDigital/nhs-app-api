using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nhs.App.Api.Integration.Tests
{
    public abstract class CommunicationHttpFunctionBase
    {
        private static string _applicationUrl;
        private static string _apigeeApiKey;

        protected static void TestClassSetup(TestContext context)
        {
            _applicationUrl = context!.Properties["ApplicationUrl"]?.ToString();
            _apigeeApiKey = context!.Properties["ApigeeApiKey"]?.ToString();
        }

        protected static HttpClient CreateHttpClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_applicationUrl)
            };

            client.DefaultRequestHeaders.Add("X-Api-Key", _apigeeApiKey);
            client.DefaultRequestHeaders.Add("X-Correlation-ID", Guid.NewGuid().ToString());

            return client;
        }

        protected static async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<T>(
                responseString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return responseObject;
        }
    }
}
