using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision
{

    [TestClass]
    public class VisionLoggingExtensionsTests
    {
        private Mock<ILogger<VisionSessionService>> _logger;
        private IFixture _fixture;
        private static Regex _guidRegex;
        private static TestContext _context;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _context = testContext;
            _guidRegex = new Regex(Constants.Regex.GuidRegex, RegexOptions.IgnoreCase);
        }

        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _logger =  _fixture.Freeze<Mock<ILogger<VisionSessionService>>>();
        }

        [TestMethod]
        public void Vision_LoggingExtension_Validate_SuccessfulErrorLog()
        {
            var expectedResponse = _fixture.Create<VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse>>();

            _logger.Object.LogVisionErrorResponse(expectedResponse);

            _logger.Verify(
                m => m.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<FormattedLogValues>(v => v.ToString().Contains("Name", StringComparison.InvariantCulture)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()
                )
            );

        }

        [TestMethod]
        public void Vision_LoggingExtension_Validate_GuidsFiltered()
        {
            const string userIdentityGuid = "Failed for user 'efa22060-9221-43a6-a0f0-6c0350b8f44d'";

            var errorResponse = VisionApiObjectResponseBuilder
                .BuildUnsuccessfulResponseWithErrorCode<BookedAppointmentsResponse>("-35");

            errorResponse.RawResponse.Body.VisionResponse.ServiceHeader.Outcome.Error.Description = userIdentityGuid;
            var sampleResponse = new VisionPFSClient.VisionApiObjectResponse<BookedAppointmentsResponse>(HttpStatusCode
                .InternalServerError) {RawResponse = errorResponse.RawResponse};

            var response = VisionLoggingExtensions.CensorResponse(sampleResponse);

            var hasGuids = response.Descendants()
                .OfType<JProperty>()
                .Where(ContainsGuid)
                .Any();
            hasGuids.Should().BeFalse();
        }

        private static bool ContainsGuid(JProperty attribute)
        {
            var guidMatch = _guidRegex.Match(attribute.Value.ToString());
            return guidMatch.Success;
        }
    }
}
