using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis
{
    [TestClass]
    public class EmisLoggingExtensionsTests
    {
        private IFixture _fixture;
        private static Regex _guidRegex;
        private static Mock<ILogger<EmisSessionService>> _logger;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger =  _fixture.Freeze<Mock<ILogger<EmisSessionService>>>();
            _guidRegex = new Regex(Constants.Regex.GuidRegex, RegexOptions.IgnoreCase);
            _sampleSuccessStatusCodes = new List<HttpStatusCode>()
            {
                HttpStatusCode.OK
            };
        }

        [TestMethod]
        public void CensorResponse_PatientSensitiveData_attributes()
        {
            // Arrange
            var demographicsResponse = _fixture.Create<DemographicsGetResponse>();
            var demoResponse = new EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.Conflict, RequestsForSuccessOutcome.DemographicsGet, _sampleSuccessStatusCodes)
            {
                Body = demographicsResponse
            };
            
            // Act
            var response = EmisLoggingExtensions.CensorResponse(_logger.Object, demoResponse);
            
            // Assert
            response
                .Descendants()
                .OfType<JProperty>()
                .Where(attr => PatientFields.PatientSensitiveFields.Contains(attr.Name))
                .Should().BeEmpty();
        }
        
        [TestMethod]
        public void CensorResponse_PatientSensitiveData_attributeValue()
        {
            // Arrange
            const string userIdentityGuid = "User Identity 'efa22060-9221-43a6-a0f0-6c0350b8f44d'";
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = userIdentityGuid;
            var sampleResponse = new EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode
                .InternalServerError, RequestsForSuccessOutcome.DemographicsGet, _sampleSuccessStatusCodes) {ExceptionErrorResponse = errorResponse};
            
            // Act
            var response = EmisLoggingExtensions.CensorResponse(_logger.Object, sampleResponse);
            
            // Assert
            response.Descendants()
                .OfType<JProperty>()
                .Where(ContainsGuid)
                .Should().BeEmpty();
        }
        
        private static bool ContainsGuid(JProperty attribute)
        {
            var guidMatch = _guidRegex.Match(attribute.Value.ToString());
            return guidMatch.Success;
        }
    }
}
