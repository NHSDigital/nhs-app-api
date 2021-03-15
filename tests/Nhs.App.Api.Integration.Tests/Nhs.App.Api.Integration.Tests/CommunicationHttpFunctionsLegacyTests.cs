using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nhs.App.Api.Integration.Tests
{
    [TestClass]
    public class CommunicationHttpFunctionsLegacyTests : CommunicationHttpFunctionBase
    {
        private static string _sendToNhsNumbers;
        private static string _sendToOdsCode;

        private const string ValidAppMessageJson =
            " \"appMessage\": { \"sender\":\"Hello Team E\", \"body\":\"This is a message body\" }";

        private const string ValidPushNotificationJson =
            " \"pushNotification\": { \"title\":\"Title\", \"body\":\"This is a notification body\", \"url\":\"https://www.nhs.uk\" }";

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            var testConfiguration = new TestConfiguration(context);
            TestClassSetup(testConfiguration);

            var sendToNhsNumbers= testConfiguration.SendToNhsNumbers
                .Select( x => $"\"{x}\"")
                .ToArray();

            _sendToNhsNumbers = $"[{string.Join(',', sendToNhsNumbers)}]";

            _sendToOdsCode = testConfiguration.SendToOdsCode == null ? "null" : $"\"{testConfiguration.SendToOdsCode}\"";
        }

        [TestMethod]
        public async Task CommunicationPost_ValidAppMessageByNhsNumbers_ReturnsCreatedStatusCode()
        {
            // Arrange
            var validPayload = BuildValidCommunicationPostBody(_sendToNhsNumbers, "null", ValidAppMessageJson);

            await CommunicationPost_ValidTest(validPayload);
        }

        [TestMethod]
        public async Task CommunicationPost_ValidAppMessageByOdsCode_ReturnsCreatedStatusCode()
        {
            // Arrange
            var validPayload = BuildValidCommunicationPostBody("null", _sendToOdsCode, ValidAppMessageJson);

            await CommunicationPost_ValidTest(validPayload);
        }

        [TestMethod]
        public async Task CommunicationPost_ValidPushNotificationByNhsNumbers_ReturnsCreatedStatusCode()
        {
            // Arrange
            var validPayload = BuildValidCommunicationPostBody(_sendToNhsNumbers, "null", ValidPushNotificationJson);

            await CommunicationPost_ValidTest(validPayload);
        }

        [TestMethod]
        public async Task CommunicationPost_ValidPushNotificationByOdsCode_ReturnsCreatedStatusCode()
        {
            // Arrange
            var validPayload = BuildValidCommunicationPostBody("null", _sendToOdsCode, ValidPushNotificationJson);

            await CommunicationPost_ValidTest(validPayload);
        }

        [DataTestMethod]
        [DataRow("{ \"channels\": { " + ValidAppMessageJson + " } }", DisplayName = "No recipients")]
        [DataRow("{ \"recipients\": { \"nhsNumbers\": [\"9487416153\"], \"odsCode\": null } }", DisplayName = "No channels")]
        [DataRow("{ \"recipients\": { \"nhsNumbers\": [\"9487416153\"], \"odsCode\": \"A12355\" }, \"channels\": { " + ValidAppMessageJson +" } }", DisplayName = "Both recipients")]
        [DataRow("{ \"recipients\": { \"nhsNumbers\": [\"9487416153\"], \"odsCode\": \"A12355\" }, \"channels\": {  \"appMessage\": { \"sender\":\"Hello Team E\", \"body\":\"Body with <b>html</b> tags\" } } }", DisplayName = "Unsafe content")]
        public async Task CommunicationPost_WithInvalidPayload_Returns400BadRequest(string invalidJson)
        {
            // Arrange
            using var httpClient = CreateLegacyHttpClient();

            var httpContent = new StringContent(invalidJson, Encoding.UTF8, "application/json");

            // Act
            var response = await httpClient.PostAsync("communication", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task CommunicationPost_InvalidApiKey_Returns401Unauthorized()
        {
            // Arrange
            using var httpClient = CreateLegacyHttpClient();

            var stringPayload = BuildValidCommunicationPostBody(_sendToNhsNumbers, "null", ValidAppMessageJson);
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Act
            var response = await httpClient.PostAsync("communication", httpContent, "invalid-key");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        private static async Task CommunicationPost_ValidTest(string validPayload)
        {
            using var httpClient = CreateLegacyHttpClient();

            var httpContent = new StringContent(validPayload, Encoding.UTF8, "application/json");

            // Act
            var response = await httpClient.PostAsync("communication", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var responseObject = await DeserializeResponseAsync<CommunicationPostResponse>(response);
            Guid.TryParse(responseObject.Id, out _).Should().BeTrue();
        }

        private static string BuildValidCommunicationPostBody(string nhsNumbersJson, string odsCodeJson, string channelsJson)
        {
            return $"{{ \"channels\": {{ {channelsJson} }}," +
                   $" \"recipients\": {{ \"nhsNumbers\": {nhsNumbersJson}, \"odsCode\": {odsCodeJson} }} }}";
        }
    }
}
