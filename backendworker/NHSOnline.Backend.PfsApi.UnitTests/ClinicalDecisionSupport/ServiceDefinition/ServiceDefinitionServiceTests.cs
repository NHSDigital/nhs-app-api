using System;
using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
using NHSOnline.Backend.Support.Sanitization;
using Task = System.Threading.Tasks.Task;

namespace NHSOnline.Backend.PfsApi.UnitTests.ClinicalDecisionSupport.ServiceDefinition
{
    [TestClass]
    public class ServiceDefinitionServiceTests
    {
        private Mock<IHtmlSanitizer> _mockHtmlSanitizer;
        private Mock<IFhirSanitizationHelper> _mockFhirSanitizationHelper;
        private Mock<ILogger<ServiceDefinitionService>> _mockLogger;
        private Mock<IOnlineConsultationsProviderHttpClient> _mockProviderHttpClient;
        
        private const string ServiceDefinitionId = "testId";
        private const string GuidanceResponseJsonContent = "{ \"resourceType\" : \"Bundle\" }";
        private const string ServiceDefinitionJsonContent = "{ \"resourceType\" : \"ServiceDefinition\", \"contained\": [ { \"resourceType\": \"Questionnaire\" } ] }";
        private const string BundleJsonContent = "{ \"resourceType\" : \"Bundle\", \"type\": \"searchset\", \"total\": 3 }";

        private ServiceDefinitionService _service;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _mockHtmlSanitizer = new Mock<IHtmlSanitizer>();
            _mockFhirSanitizationHelper = new Mock<IFhirSanitizationHelper>();
            _mockLogger = new Mock<ILogger<ServiceDefinitionService>>();
            _mockProviderHttpClient = new Mock<IOnlineConsultationsProviderHttpClient>();
            
            _service = new ServiceDefinitionService(
                _mockLogger.Object,
                _mockHtmlSanitizer.Object,
                _mockFhirSanitizationHelper.Object);
        }
        
        [TestMethod]
        public async Task GetServiceDefinitionById_WhenProviderClientThrowsException_ReturnsBadRequest()
        {
            // Arrange
            _mockProviderHttpClient
                .Setup(pc => pc.GetServiceDefinitionById(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal))))
                .Throws<HttpRequestException>();

            // Act
            var response = await _service.GetServiceDefinitionById(_mockProviderHttpClient.Object, ServiceDefinitionId);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadRequest>();
        }

        [TestMethod]
        public async Task GetServiceDefinitionById_WhenProviderClientReturnsNotFound_ReturnsNotFound()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound };

            _mockProviderHttpClient
                .Setup(pc => pc.GetServiceDefinitionById(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal))))
                .ReturnsAsync(httpResponse);

            // Act
            var response = await _service.GetServiceDefinitionById(_mockProviderHttpClient.Object, ServiceDefinitionId);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.NotFound>();
        }

        [TestMethod]
        public async Task GetServiceDefinitionById_WhenProviderClientReturnsUnsuccessfulStatusCode_ReturnsBadGateway()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest };

            _mockProviderHttpClient
                .Setup(pc => pc.GetServiceDefinitionById(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal))))
                .ReturnsAsync(httpResponse);

            // Act
            var response = await _service.GetServiceDefinitionById(_mockProviderHttpClient.Object, ServiceDefinitionId);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetServiceDefinitionById_WhenProviderClientReturnsNullContent_ReturnsBadGateway()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = null
            };

            _mockProviderHttpClient
                .Setup(pc => pc.GetServiceDefinitionById(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal))))
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.GetServiceDefinitionById(_mockProviderHttpClient.Object, ServiceDefinitionId);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow(BundleJsonContent)]
        public async Task GetServiceDefinitionById_WhenProviderClientReturnsNonServiceDefinitionContent_ReturnsBadGateway(string content)
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(content, Encoding.UTF8, Constants.ContentTypes.ApplicationJsonFhir)
            };
            
            _mockProviderHttpClient
                .Setup(pc => pc.GetServiceDefinitionById(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal))))
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.GetServiceDefinitionById(_mockProviderHttpClient.Object, ServiceDefinitionId);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
            _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeServiceDefinition(
                It.IsAny<Hl7.Fhir.Model.ServiceDefinition>(), It.IsAny<IHtmlSanitizer>()), Times.Never);
        }

        [TestMethod]
        public async Task GetServiceDefinitionById_WhenResponseParsedSuccessfully_SanitizesResponseAndReturnsSuccess()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(ServiceDefinitionJsonContent, Encoding.UTF8, Constants.ContentTypes.ApplicationJsonFhir)
            };

            _mockProviderHttpClient
                .Setup(pc => pc.GetServiceDefinitionById(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal))))
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.GetServiceDefinitionById(_mockProviderHttpClient.Object, ServiceDefinitionId);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.Success>();
            _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeServiceDefinition(
                It.IsAny<Hl7.Fhir.Model.ServiceDefinition>(), It.IsAny<IHtmlSanitizer>()), Times.Once);
        }

        [TestMethod]
        public async Task SearchServiceDefinitionsByQuery_WhenProviderClientThrowsException_ReturnsBadRequest()
        {
            // Arrange
            _mockProviderHttpClient
                .Setup(pc => pc.SearchServiceDefinitionsByQuery())
                .Throws<HttpRequestException>();

            // Act
            var response = await _service.SearchServiceDefinitionsByQuery(_mockProviderHttpClient.Object);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadRequest>();
        }
        
        [TestMethod]
        public async Task SearchServiceDefinitionsByQuery_WhenProviderClientReturnsUnsuccessfulStatusCode_ReturnsBadGateway()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest };

            _mockProviderHttpClient
                .Setup(pc => pc.SearchServiceDefinitionsByQuery())
                .ReturnsAsync(httpResponse);

            // Act
            var response = await _service.SearchServiceDefinitionsByQuery(_mockProviderHttpClient.Object);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
        }

        [TestMethod]
        public async Task SearchServiceDefinitionsByQuery_WhenProviderClientReturnsNullContent_ReturnsBadGateway()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = null
            };

            _mockProviderHttpClient
                .Setup(pc => pc.SearchServiceDefinitionsByQuery())
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.SearchServiceDefinitionsByQuery(_mockProviderHttpClient.Object);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow(ServiceDefinitionJsonContent)]
        public async Task SearchServiceDefinitionsByQuery_WhenProviderClientReturnsNonBundleContent_ReturnsBadGateway(string content)
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(content, Encoding.UTF8, Constants.ContentTypes.ApplicationJsonFhir)
            };
            
            _mockProviderHttpClient
                .Setup(pc => pc.SearchServiceDefinitionsByQuery())
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.SearchServiceDefinitionsByQuery(_mockProviderHttpClient.Object);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
            _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeServiceDefinitionSearchBundle(
                It.IsAny<Bundle>(), It.IsAny<IHtmlSanitizer>()), Times.Never);
        }
        
        [TestMethod]
        public async Task SearchServiceDefinitionsByQuery_WhenResponseParsedSuccessfully_SanitizesResponseAndReturnsSuccess()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(BundleJsonContent, Encoding.UTF8, Constants.ContentTypes.ApplicationJsonFhir)
            };

            _mockProviderHttpClient
                .Setup(pc => pc.SearchServiceDefinitionsByQuery())
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.SearchServiceDefinitionsByQuery(_mockProviderHttpClient.Object);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.Success>();
            _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeServiceDefinitionSearchBundle(
                It.IsAny<Bundle>(), It.IsAny<IHtmlSanitizer>()), Times.Once);
        }

        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenNullParametersProvided_ReturnsBadRequest()
        {
            // Act
            var response = await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, null);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadRequest>();
        }

        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenProviderClientThrowsException_ReturnsBadRequest()
        {
            // Arrange
            _mockProviderHttpClient
                .Setup(pc => pc.EvaluateServiceDefinition(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal)),
                    It.IsAny<string>()))
                .Throws<HttpRequestException>();

            // Act
            var response = await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, new Parameters());

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadRequest>();
        }
        
        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenProviderClientReturnsUnsuccessfulStatusCode_ReturnsBadGateway()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest };

            _mockProviderHttpClient
                .Setup(pc => pc.EvaluateServiceDefinition(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal)),
                    It.IsAny<string>()))
                .ReturnsAsync(httpResponse);

            // Act
            var response = await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, new Parameters());

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
        }

        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenProviderClientReturnsNullContent_ReturnsBadGateway()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = null
            };

            _mockProviderHttpClient
                .Setup(pc => pc.EvaluateServiceDefinition(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal)),
                    It.IsAny<string>()))
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, new Parameters());

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow(BundleJsonContent)]
        public async Task EvaluateServiceDefinition_WhenProviderClientReturnsNonGuidanceResponseContent_ReturnsBadGateway(string content)
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(content, Encoding.UTF8, Constants.ContentTypes.ApplicationJsonFhir)
            };
            
            _mockProviderHttpClient
                .Setup(pc => pc.EvaluateServiceDefinition(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal)),
                    It.IsAny<string>()))
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, new Parameters());

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
            _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeGuidanceResponse(
                It.IsAny<GuidanceResponse>(), It.IsAny<IHtmlSanitizer>()), Times.Never);
        }
        
        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenResponseParsedSuccessfully_SanitizesResponseAndReturnsSuccess()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(GuidanceResponseJsonContent, Encoding.UTF8, Constants.ContentTypes.ApplicationJsonFhir)
            };

            _mockProviderHttpClient
                .Setup(pc => pc.EvaluateServiceDefinition(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal)),
                    It.IsAny<string>()))
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, new Parameters());

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.Success>();
            _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeGuidanceResponse(
                It.IsAny<GuidanceResponse>(), It.IsAny<IHtmlSanitizer>()), Times.Once);
        }
    }
}