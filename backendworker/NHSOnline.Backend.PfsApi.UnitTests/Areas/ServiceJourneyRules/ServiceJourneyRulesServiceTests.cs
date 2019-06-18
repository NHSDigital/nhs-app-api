using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.DynamicProxy;
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
       public async Task IsJourneyEnabled_WhenProviderIsIm1_ReturnsTrue()
       {
           // Arrange
           var mockResponse =
               new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.Created)
               {
                   Body = new ServiceJourneyRulesResponse
                   {
                       Appointments = new ServiceJourneyRulesApi.Models.Appointments
                       {
                           Provider = AppointmentsProvider.im1
                       }
                   }
               };
           
           _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(DefaultOdsCode))
               .ReturnsAsync(mockResponse);

           // Act
           var result = await _systemUnderTest.IsJourneyEnabled(DefaultOdsCode);

           // Assert
           result.Should().BeTrue();
       }

       [TestMethod]
       public async Task IsJourneyEnabled_WhenThereIsNoProvider_ReturnsFalse()
       {
           // Arrange
           var mockResponse =
               new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.Created)
               {
                   Body = new ServiceJourneyRulesResponse
                   {
                       Appointments = new ServiceJourneyRulesApi.Models.Appointments
                       {
                           Provider = AppointmentsProvider.none
                       }
                   }
               };
           
           _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(DefaultOdsCode))
               .ReturnsAsync(mockResponse);

           // Act
           var result = await _systemUnderTest.IsJourneyEnabled(DefaultOdsCode);

           // Assert
           result.Should().BeFalse();
       }

       [TestMethod]
       public async Task GetServiceJourneyRulesForOdsCode_ValidRequest_ReturnsSuccessResult()
       {
           // Arrange
           _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(DefaultOdsCode))
               .ReturnsAsync(new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.Created)
               {
                   Body = new ServiceJourneyRulesResponse
                   {
                       Appointments = new ServiceJourneyRulesApi.Models.Appointments
                       {
                           Provider = AppointmentsProvider.im1
                       }
                   }
                   
               });

           // Act
           var result = await _systemUnderTest.GetServiceJourneyRulesForOdsCode(DefaultOdsCode);

           // Assert
           result.Should().BeAssignableTo<ServiceJourneyRulesConfigResult.Success>();
       }
       
       [TestMethod]
       public async Task IsJourneyEnabled_BodyIsNull_ReturnsFalse()
       {
           // Arrange
           var mockResponse =
               new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.Created);
           
           _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(DefaultOdsCode))
               .ReturnsAsync(mockResponse);

           // Act
           var result = await _systemUnderTest.IsJourneyEnabled(DefaultOdsCode);

           // Assert
           result.Should().BeFalse();
       }
       
       [TestMethod]
       [ExpectedException(typeof(InvalidProxyConstructorArgumentsException))]
       public async Task IsJourneyEnabled_ThrowsError()
       {
           // Arrange
           var mockResponse =
               new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.Created);

           _serviceJourneyRulesClient.Setup(x => x.GetServiceJourneyRules(DefaultOdsCode))
               .ReturnsAsync(mockResponse);

           // Act
           await _systemUnderTest.IsJourneyEnabled("odsCode");
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

       [TestMethod] public async Task GetServiceJourneyRulesForOdsCode_InvalidRequest_ReturnsInternalServerError()
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