using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Worker.IntegrationTests.Worker.Models.Patient;
using NHSOnline.Backend.Worker.IntegrationTests.Worker.Models.User;

namespace NHSOnline.Backend.Worker.IntegrationTests.Worker
{
    public class WorkerClient
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly HttpClient _client;

        public WorkerClient()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(Configuration.BackendBaseUrl)
            };
        }

        public async Task<Im1ConnectionResponse> GetIm1Connection(string connectionToken, string odsCode)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, WorkerPaths.PatientIm1Connection);
            request.Headers.Add(WorkerHeaders.ConnectionToken, connectionToken);
            request.Headers.Add(WorkerHeaders.OdsCode, odsCode);
            var response = await SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<Im1ConnectionResponse>(json, _jsonSerializerSettings);
            return res;
        }

        public async Task<Im1ConnectionResponse> PostIm1Connection(Im1ConnectionRequest requestBody)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, WorkerPaths.PatientIm1Connection)
            {
                Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")

            };

            var response = await SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Im1ConnectionResponse>(json, _jsonSerializerSettings);
        }

        public async Task<SessionResponseTestObject> PostSession(UserSessionRequest requestBody)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, WorkerPaths.PatientSessionConnection)
            {
                Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
            };

            var response = await SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            var sessionResponseTestObject = new SessionResponseTestObject
            {
                UserSessionResponse = JsonConvert.DeserializeObject<UserSessionResponse>(json, WorkerClient._jsonSerializerSettings)
            };

            if (response.Headers.TryGetValues("Set-Cookie", out var values))
            {
                sessionResponseTestObject.Cookie = values.First();
            }

            return sessionResponseTestObject;
        }


        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                // Exception is thrown here to ensure that the tests fail at the appropriate location and not further down the line
                // when values are not as expected.  This makes it easier to debug.
                throw new NhsoHttpException(request, response);
            }

            return response;
        }
    }
}
