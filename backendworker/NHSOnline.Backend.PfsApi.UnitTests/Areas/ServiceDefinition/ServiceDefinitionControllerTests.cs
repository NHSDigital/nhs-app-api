using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.ServiceDefinition;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
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
        private string _provider;
        private string _id;
        private Parameters _evaluateParameters;
        
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

            _serviceDefinitionController = _fixture.Create<ServiceDefinitionController>();

            _successResult = new ServiceDefinitionResult.Success("");
            _provider = "eConsult";
            _id = "testId";
            _evaluateParameters = new Parameters();
        }

        [TestMethod]
        public async Task GetServiceDefinitionById_WhenProviderClientNotFound_ReturnsBadRequest()
        {
            // Arrange
            SetupHttpClientPool(false);
            
            // Act
            var actualResponse = await _serviceDefinitionController.SearchServiceDefinitionsByQuery(_provider);

            // Assert
            VerifyGetFromClientPoolCalledWithProvider(_provider);
            var value = actualResponse.Should().BeAssignableTo<BadRequestResult>();
            value.Subject.StatusCode.Should().Be(400);
        }

        [TestMethod]
        public async Task SearchServiceDefinitionsByQuery_WhenCalledWithProviderParameter_ServiceGetServiceDefinitionById()
        {
            // Arrange
            SetupHttpClientPool(true);
            _mockServiceDefinitionService
                .Setup(s => s.SearchServiceDefinitionsByQuery(It.Is<IOnlineConsultationsProviderHttpClient>(
                    c => c == _mockProviderHttpClient.Object)))
                .Returns(Task.FromResult(_successResult));

            // Act
            var actualResponse = await _serviceDefinitionController.SearchServiceDefinitionsByQuery(_provider);

            // Assert
            VerifyGetFromClientPoolCalledWithProvider(_provider);
            var value = actualResponse.Should().BeAssignableTo<OkObjectResult>();
            value.Subject.StatusCode.Should().Be(200);
            _mockServiceDefinitionService.Verify(
                s => s.SearchServiceDefinitionsByQuery(It.IsAny<IOnlineConsultationsProviderHttpClient>()),
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
            var actualResponse = await _serviceDefinitionController.EvaluateServiceDefinition(_provider, id, _evaluateParameters);
            
            // Assert
            var value = actualResponse.Should().BeAssignableTo<BadRequestResult>();
            value.Subject.StatusCode.Should().Be(400);
            _mockServiceDefinitionService.Verify(
                s => s.EvaluateServiceDefinition(It.IsAny<IOnlineConsultationsProviderHttpClient>(), It.IsAny<string>(),
                    It.IsAny<Parameters>()), Times.Never);
        }

        [TestMethod]
        [DataRow(null)]
        public async Task EvaluateServiceDefinition_WhenCalledWithoutParametersBody_ReturnsBadRequest(Parameters evaluateParameters)
        {
            // Arrange
            SetupHttpClientPool(true);
            
            // Act
            var actualResponse = await _serviceDefinitionController.EvaluateServiceDefinition(_provider, _id, evaluateParameters);
            
            // Assert
            var value = actualResponse.Should().BeAssignableTo<BadRequestResult>();
            value.Subject.StatusCode.Should().Be(400);
            _mockServiceDefinitionService.Verify(
                s => s.EvaluateServiceDefinition(It.IsAny<IOnlineConsultationsProviderHttpClient>(), It.IsAny<string>(),
                    It.IsAny<Parameters>()), Times.Never);
        }

        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenProviderClientNotFound_ReturnsBadRequest()
        {
            // Arrange
            SetupHttpClientPool(false);
            
            // Act
            var actualResponse = await _serviceDefinitionController.EvaluateServiceDefinition(_provider, _id, _evaluateParameters);


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
                    It.Is<Parameters>(parameters => _evaluateParameters == parameters)))
                .Returns(Task.FromResult(_successResult));
            
            // Act
            var actualResponse = await _serviceDefinitionController.EvaluateServiceDefinition(_provider, _id, _evaluateParameters);


            // Assert
            VerifyGetFromClientPoolCalledWithProvider(_provider);
            var value = actualResponse.Should().BeAssignableTo<OkObjectResult>();
            value.Subject.StatusCode.Should().Be(200);
            _mockServiceDefinitionService.Verify(
                s => s.EvaluateServiceDefinition(It.Is<IOnlineConsultationsProviderHttpClient>(
                        c => c == _mockProviderHttpClient.Object),
                    It.Is<string>(sdId => _id.Equals(sdId, StringComparison.Ordinal)),
                    It.Is<Parameters>(parameters => _evaluateParameters == parameters)),
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