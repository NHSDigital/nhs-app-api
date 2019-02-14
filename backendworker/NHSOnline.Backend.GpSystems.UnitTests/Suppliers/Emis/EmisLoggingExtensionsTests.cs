using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis
{
    [TestClass]
    public class EmisLoggingExtensionsTests
    {
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
        }

        [TestMethod]
        public void CensoryFilter_PatientSensitiveData_attributes()
        {
            var demographicsResponse = _fixture.Create<DemographicsGetResponse>();
            var demoResponse = new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.Conflict)
            {
                Body = demographicsResponse
            };
            var response = EmisLoggingExtensions.CensorResponse(demoResponse);
            var hasSensitiveFields = response
                .Descendants()
                .OfType<JProperty>()
                .Any(attr => PatientFields.PatientSensitiveFields.Contains(attr.Name));
            hasSensitiveFields.Should().BeFalse();
        }
        
        [TestMethod]
        public void CensoryFilter_PatientSensitiveData_attributeValue()
        {
            const string userIdentityGuid = "User Identity 'efa22060-9221-43a6-a0f0-6c0350b8f44d'";
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = userIdentityGuid;
            var sampleResponse = new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode
                        .InternalServerError) {ExceptionErrorResponse = errorResponse};
            var response = EmisLoggingExtensions.CensorResponse(sampleResponse);
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