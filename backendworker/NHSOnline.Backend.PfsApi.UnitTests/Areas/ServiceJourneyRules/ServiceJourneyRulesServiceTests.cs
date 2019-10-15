using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.ServiceJourneyRules
{
   [TestClass]
   public class ServiceJourneyRulesServiceTests
   {
       private const string DefaultOdsCode = "A12345";

       private IFixture _fixture;
       private ServiceJourneyRulesService _systemUnderTest;
       private Mock<IServiceJourneyRulesClient> _serviceJourneyRulesClient;

       [TestInitialize]
       public void TestInitialize()
       {
           _fixture = new Fixture().Customize(new AutoMoqCustomization());

           _serviceJourneyRulesClient = _fixture.Freeze<Mock<IServiceJourneyRulesClient>>();
           _systemUnderTest = _fixture.Create<ServiceJourneyRulesService>();
       }

       [TestMethod]
       [DataRow(true)]
       [DataRow(false)]
       public async Task GetServiceJourneyRulesForOdsCode_ValidRequest_ReturnsSuccessResult(bool hasLinkedAccounts)
       {
           // Arrange
           _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(DefaultOdsCode))
               .ReturnsAsync(new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.Created)
               {
                   Body = _fixture.Create<ServiceJourneyRulesResponse>()
               });

           // Act
           var result = await _systemUnderTest.GetServiceJourneyRulesForOdsCode(DefaultOdsCode, hasLinkedAccounts);

           // Assert
           result.Should().BeAssignableTo<ServiceJourneyRulesConfigResult.Success>();
           var serviceJourneyRulesResponse = ((ServiceJourneyRulesConfigResult.Success) result).Response;
           
           Assert.AreEqual(hasLinkedAccounts, serviceJourneyRulesResponse.Journeys.HasLinkedAccounts);
       }
       
       [TestMethod]
       public async Task GetServiceJourneyRulesForOdsCode_ValidRequest_ReturnsNotFoundIfBodyNull()
       {
           // Arrange
           _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(DefaultOdsCode))
               .ReturnsAsync(
                   new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.Created));

           // Act
           var result = await _systemUnderTest.GetServiceJourneyRulesForOdsCode(DefaultOdsCode);

           // Assert
           result.Should().BeAssignableTo<ServiceJourneyRulesConfigResult.NotFound>();
       }

       [TestMethod]
       public async Task GetServiceJourneyRulesForOdsCode_GetRequestReturnsNotFound_ReturnsNotFound()
       {
           // Arrange
           _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(DefaultOdsCode))
               .ReturnsAsync(new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.NotFound));

           // Act
           var result = await _systemUnderTest.GetServiceJourneyRulesForOdsCode(DefaultOdsCode);

           // Assert
           result.Should().BeAssignableTo<ServiceJourneyRulesConfigResult.NotFound>();
       }

       [TestMethod] 
       public async Task GetServiceJourneyRulesForOdsCode_InvalidRequest_ReturnsInternalServerError()
       {
           // Arrange
           _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(DefaultOdsCode))
               .Throws<HttpRequestException>()
               .Verifiable();

           // Act
           var result = await _systemUnderTest.GetServiceJourneyRulesForOdsCode(DefaultOdsCode);

           // Assert
           result.Should().BeAssignableTo<ServiceJourneyRulesConfigResult.InternalServerError>();
       }
   }
}