using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Nhs.App.Api.Integration.Tests.Services;
using Nhs.App.Api.Integration.Tests.Services.AccessTokenService;

namespace Nhs.App.Api.Integration.Tests
{
    public abstract class CommunicationHttpFunctionBase
    {
        private static TestConfiguration _testConfiguration;
        private static IAccessTokenCacheService _accessTokenCacheService;

        protected static void TestClassSetup(TestConfiguration testConfiguration)
        {
            _testConfiguration = testConfiguration;
            _accessTokenCacheService = new AccessTokenCacheService(testConfiguration);
        }

        protected static NhsAppApiJwtWrapperClient CreateHttpClient()
        {
            return new(_testConfiguration, _accessTokenCacheService);
        }

        protected static NhsAppApiApiKeyWrapperClient CreateLegacyHttpClient()
        {
            return new(_testConfiguration);
        }

        protected static async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<T>(
                responseString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return responseObject;
        }

        protected static async Task<OperationOutcome> ParseOperationOutcome(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var operationOutcome = new FhirJsonParser().Parse<OperationOutcome>(responseString);

            return operationOutcome;
        }
    }
}
