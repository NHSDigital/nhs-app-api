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
using NHSOnline.Backend.ServiceJourneyRules.Common;
using NHSOnline.Backend.ServiceJourneyRules.Common.Models;
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
       public async Task GetServiceJourneyRulesForOdsCode_ValidRequest_ReturnsSuccessResult()
       {
           // Arrange
           _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(DefaultOdsCode))
               .ReturnsAsync(new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.Created)
               {
                   Body = _fixture.Create<ServiceJourneyRulesResponse>()
               });

           // Act
           var result = await _systemUnderTest.GetServiceJourneyRulesForOdsCode(DefaultOdsCode);

           // Assert
           result.Should().BeAssignableTo<ServiceJourneyRulesConfigResult.Success>();

       }
       
       [DataTestMethod]
       [DataRow(AppointmentsProvider.informatica, AppointmentsProvider.linkedAccount)]
       [DataRow(AppointmentsProvider.gpAtHand, AppointmentsProvider.linkedAccount)]
       [DataRow(AppointmentsProvider.im1, AppointmentsProvider.im1)]
       [DataRow(AppointmentsProvider.eConsult, AppointmentsProvider.im1)]
       public async Task GetServiceJourneyRulesForOdsCode_ForLinkedAccount_ValidRequest_ReturnsSuccessResult
           (AppointmentsProvider originalProvider, AppointmentsProvider expectedProvider)
       {
           // Arrange
           _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(DefaultOdsCode))
               .ReturnsAsync(new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.Created)
               {
                   Body = new ServiceJourneyRulesResponse
                   {
                       Journeys = new Journeys
                       {
                           Appointments = new ServiceJourneyRulesApi.Models.Appointments
                           {
                               Provider = originalProvider
                           },
                           NominatedPharmacy = true,
                           Messaging = true,
                           Notifications = true,
                       }
                   }
               });

           // Act
           var result = await _systemUnderTest.GetServiceJourneyRulesForLinkedAccount(DefaultOdsCode);

           // Assert
           result.Should().BeAssignableTo<ServiceJourneyRulesConfigResult.Success>();

           var response = result.Should().BeAssignableTo<ServiceJourneyRulesConfigResult.Success>().Subject.Response;
           Assert.AreEqual(false, response.Journeys.Messaging);
           Assert.AreEqual(false, response.Journeys.NominatedPharmacy);
           Assert.AreEqual(false, response.Journeys.Notifications);
           Assert.AreEqual(expectedProvider, response.Journeys.Appointments.Provider);
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