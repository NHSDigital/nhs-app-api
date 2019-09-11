using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.PfsApi.Areas.ServiceDefinition;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.Support;
using UnitTestHelper;
using Task = System.Threading.Tasks.Task;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.ServiceDefinition
{
    [TestClass]
    public class ServiceDefinitionControllerTests
    {
        private IFixture _fixture;
        private ServiceDefinitionController _serviceDefinitionController;
        private Mock<IServiceDefinitionService> _mockServiceDefinitionService;
        
        private Mock<IOnlineConsultationsProviderHttpClientPool> _mockOnlineConsultationsProviderHttpClientPool;
        private Mock<IOnlineConsultationsProviderHttpClient> _mockProviderHttpClient;

        private ServiceDefinitionResult _successResult;
        private ServiceDefinitionListResult _getServiceDefinitionsSuccessResult;
        private string _provider;
        private string _id;
        private Parameters _evaluateParameters;
        private UserSession _userSession;
        private readonly DemographicsResult.Success _successResponse = new DemographicsResult.Success(new DemographicsResponse());
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockServiceDefinitionService = _fixture
                .Freeze<Mock<IServiceDefinitionService>>();
            
            _mockOnlineConsultationsProviderHttpClientPool = _fixture
                .Freeze<Mock<IOnlineConsultationsProviderHttpClientPool>>();
            _mockProviderHttpClient = _fixture
                .Freeze<Mock<IOnlineConsultationsProviderHttpClient>>();

            _successResult = new ServiceDefinitionResult.Success("");
            _getServiceDefinitionsSuccessResult = new ServiceDefinitionListResult.Success(new List<ServiceDefinitionCategory>());
            _provider = "eConsult";
            _id = "testId";
            _evaluateParameters = new Parameters();
            
            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

            _userSession = _fixture.Create<UserSession>();
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Items[Constants.HttpContextItems.UserSession]).Returns(_userSession);

            _serviceDefinitionController = _fixture.Create<ServiceDefinitionController>();
            
            _serviceDefinitionController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
            
            _successResponse.Response.Address = "Test";                                       
            _successResponse.Response.NhsNumber = "111 111 111";                              
            _successResponse.Response.DateOfBirth = DateTime.UtcNow;                          
            _successResponse.Response.Sex = "Male";
        }

        [TestMethod]
        public async Task GetServiceDefinitionById_WhenProviderClientNotFound_ReturnsBadRequest()
        {
            // Arrange
            SetupHttpClientPool(false);
            
            // Act
            var actualResponse = await _serviceDefinitionController.GetServiceDefinitions(_provider);

            // Assert
            VerifyGetFromClientPoolCalledWithProvider(_provider);
            var value = actualResponse.Should().BeAssignableTo<BadRequestResult>();
            value.Subject.StatusCode.Should().Be(400);
        }

        [TestMethod]
        public async Task GetServiceDefinitions_WhenCalledWithProviderParameter_ServiceGetServiceDefinitionById()
        {
            // Arrange
            SetupHttpClientPool(true);
            _mockServiceDefinitionService
                .Setup(s => s.GetServiceDefinitions(It.Is<IOnlineConsultationsProviderHttpClient>(
                    c => c == _mockProviderHttpClient.Object)))
                .Returns(Task.FromResult(_getServiceDefinitionsSuccessResult));

            // Act
            var actualResponse = await _serviceDefinitionController.GetServiceDefinitions(_provider);

            // Assert
            VerifyGetFromClientPoolCalledWithProvider(_provider);
            var value = actualResponse.Should().BeAssignableTo<OkObjectResult>();
            value.Subject.StatusCode.Should().Be(200);
            _mockServiceDefinitionService.Verify(
                s => s.GetServiceDefinitions(It.IsAny<IOnlineConsultationsProviderHttpClient>()),
                Times.Once);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task EvaluateServiceDefinition_WhenCalledWithoutIdInRoute_ReturnsBadRequest(string id)
        {
            // Arrange
            SetupHttpClientPool(true);
            
            // Act
            var actualResponse = await _serviceDefinitionController.EvaluateServiceDefinition(_provider, id, _evaluateParameters, false);
            
            // Assert
            var value = actualResponse.Should().BeAssignableTo<BadRequestResult>();
            value.Subject.StatusCode.Should().Be(400);
            _mockServiceDefinitionService.Verify(
                s => s.EvaluateServiceDefinition(It.IsAny<IOnlineConsultationsProviderHttpClient>(), It.IsAny<string>(),
                    It.IsAny<Parameters>(), It.IsAny<bool>(), It.IsAny<bool>(), _userSession), Times.Never);
        }

        [TestMethod]
        [DataRow(null)]
        public async Task EvaluateServiceDefinition_WhenCalledWithoutParametersBody_ReturnsBadRequest(Parameters evaluateParameters)
        {
            // Arrange
            SetupHttpClientPool(true);
            
            // Act
            var actualResponse = await _serviceDefinitionController.EvaluateServiceDefinition(_provider, _id, evaluateParameters, false);
            
            // Assert
            var value = actualResponse.Should().BeAssignableTo<BadRequestResult>();
            value.Subject.StatusCode.Should().Be(400);
            _mockServiceDefinitionService.Verify(
                s => s.EvaluateServiceDefinition(It.IsAny<IOnlineConsultationsProviderHttpClient>(), It.IsAny<string>(),
                    It.IsAny<Parameters>(), It.IsAny<bool>(), It.IsAny<bool>(),_userSession), Times.Never);
        }

        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenProviderClientNotFound_ReturnsBadRequest()
        {
            // Arrange
            SetupHttpClientPool(false);
            
            // Act
            var actualResponse = await _serviceDefinitionController.EvaluateServiceDefinition(_provider, _id, _evaluateParameters, false);

            // Assert
            VerifyGetFromClientPoolCalledWithProvider(_provider);
            var value = actualResponse.Should().BeAssignableTo<BadRequestResult>();
            value.Subject.StatusCode.Should().Be(400);
        }

        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenCalledWithIdParametersAndClientFound_ServiceEvaluateServiceDefinition()
        {
            
            // Arrange
            SetupHttpClientPool(true);
            _mockServiceDefinitionService
                .Setup(s => s.EvaluateServiceDefinition(It.Is<IOnlineConsultationsProviderHttpClient>(
                        c => c == _mockProviderHttpClient.Object),
                    It.Is<string>(id => _id.Equals(id, StringComparison.Ordinal)),
                    It.Is<Parameters>(parameters => _evaluateParameters == parameters),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<UserSession>()))
                .Returns(Task.FromResult(_successResult));

            // Act
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("NHSO-Javascript-Disabled", "false");
            _serviceDefinitionController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var actualResponse = await _serviceDefinitionController.EvaluateServiceDefinition(_provider, _id, _evaluateParameters, false);

            // Assert
            VerifyGetFromClientPoolCalledWithProvider(_provider);
            var value = actualResponse.Should().BeAssignableTo<OkObjectResult>();
            value.Subject.StatusCode.Should().Be(200);
            _mockServiceDefinitionService.Verify(
                s => s.EvaluateServiceDefinition(It.Is<IOnlineConsultationsProviderHttpClient>(
                        c => c == _mockProviderHttpClient.Object),
                    It.Is<string>(sdId => _id.Equals(sdId, StringComparison.Ordinal)),
                    It.Is<Parameters>(parameters => _evaluateParameters == parameters),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<UserSession>()),
                Times.Once);
        }

        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenRequestHasJSDisabledHeader_EvaluateCalledWithAddJSDisabledHeaderTrue()
        {
            // Arrange
            SetupHttpClientPool(true);
            _mockServiceDefinitionService
                .Setup(s => s.EvaluateServiceDefinition(It.Is<IOnlineConsultationsProviderHttpClient>(
                        c => c == _mockProviderHttpClient.Object),
                    It.Is<string>(id => _id.Equals(id, StringComparison.Ordinal)),
                    It.Is<Parameters>(parameters => _evaluateParameters == parameters),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<UserSession>()))
                .Returns(Task.FromResult(_successResult));
            
            // Act
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("NHSO-Javascript-Disabled", "true");
            _serviceDefinitionController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var actualResponse = await _serviceDefinitionController.EvaluateServiceDefinition(_provider, _id, _evaluateParameters, false);

            // Assert
            VerifyGetFromClientPoolCalledWithProvider(_provider);
            var value = actualResponse.Should().BeAssignableTo<OkObjectResult>();
            value.Subject.StatusCode.Should().Be(200);
            _mockServiceDefinitionService.Verify(
                s => s.EvaluateServiceDefinition(It.Is<IOnlineConsultationsProviderHttpClient>(
                        c => c == _mockProviderHttpClient.Object),
                    It.Is<string>(sdId => _id.Equals(sdId, StringComparison.Ordinal)),
                    It.Is<Parameters>(parameters => _evaluateParameters == parameters),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<UserSession>()),
                Times.Once);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task EvaluateServiceDefinition_WhenDemographicsConsentQueryStringPresent_ValueIsPassedToService(bool demographicsConsentGiven)
        {
            // Arrange
            SetupHttpClientPool(true);
            _mockServiceDefinitionService
                .Setup(s => s.EvaluateServiceDefinition(It.Is<IOnlineConsultationsProviderHttpClient>(
                        c => c == _mockProviderHttpClient.Object),
                    It.Is<string>(id => _id.Equals(id, StringComparison.Ordinal)),
                    It.Is<Parameters>(parameters => _evaluateParameters == parameters),
                    It.IsAny<bool>(),
                    It.Is<bool>(consent => consent == demographicsConsentGiven),
                    It.IsAny<UserSession>()))
                .Returns(Task.FromResult(_successResult));
            
            // Act
            var httpContext = new DefaultHttpContext();
            _serviceDefinitionController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            await _serviceDefinitionController.EvaluateServiceDefinition(_provider, _id, _evaluateParameters, demographicsConsentGiven);

            // Assert
            VerifyGetFromClientPoolCalledWithProvider(_provider);
            _mockServiceDefinitionService.Verify(
                s => s.EvaluateServiceDefinition(It.Is<IOnlineConsultationsProviderHttpClient>(
                        c => c == _mockProviderHttpClient.Object),
                    It.Is<string>(sdId => _id.Equals(sdId, StringComparison.Ordinal)),
                    It.Is<Parameters>(parameters => _evaluateParameters == parameters),
                    It.IsAny<bool>(),
                    It.Is<bool>(consent => consent == demographicsConsentGiven),
                    It.IsAny<UserSession>()),
                Times.Once);
        }

        private void SetupHttpClientPool(bool clientExists)
        {
            _mockOnlineConsultationsProviderHttpClientPool
                .Setup(p => p.GetClientByProviderName(It.IsAny<string>()))
                .Returns(clientExists ? _mockProviderHttpClient.Object : null);
        }

        private void VerifyGetFromClientPoolCalledWithProvider(string provider)
        {
            _mockOnlineConsultationsProviderHttpClientPool.Verify(
                pool => pool.GetClientByProviderName(It.Is<string>(providerName =>
                    provider.Equals(providerName, StringComparison.Ordinal))), Times.Once);
        }
    }
}