 using System;
 using AutoFixture;
 using AutoFixture.AutoMoq;
 using FluentAssertions;
 using Hl7.Fhir.Model;
 using Microsoft.AspNetCore.Http;
 using Microsoft.AspNetCore.Mvc;
 using Microsoft.Extensions.Logging;
 using Microsoft.VisualStudio.TestTools.UnitTesting;
 using Moq;
 using NHSOnline.Backend.GpSystems.Suppliers.Emis;
 using NHSOnline.Backend.PfsApi.Areas.ServiceDefinition;
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
         private Mock<HttpContext> _mockHttpContext;
         private Mock<ILoggerFactory> _mockLoggerFactory;
         private Mock<ILogger<ServiceDefinitionController>> _mockLogger;

         private ServiceDefinitionResult _successResult;
         private Parameters _evaluateParameters;
         private UserSession _userSession;

         private const string Provider = "OLC Stubs";
         private const string ServiceDefinitionId = "NHS_ADMIN";

         [TestInitialize]
         public void TestInitialize()
         {
             _successResult = new ServiceDefinitionResult.Success("");
             _evaluateParameters = new Parameters();

             _fixture = new Fixture()
                 .Customize(new AutoMoqCustomization())
                 .Customize(new ApiControllerAutoFixtureCustomization());

             _fixture.Customize<UserSession>(c => c
                 .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));
             _userSession = _fixture.Create<UserSession>();

             _mockServiceDefinitionService = _fixture
                 .Freeze<Mock<IServiceDefinitionService>>();

             _mockHttpContext = _fixture.Create<Mock<HttpContext>>();
             _mockHttpContext.Setup(x => x.Items[Constants.HttpContextItems.UserSession]).Returns(_userSession);

             _mockLogger = _fixture.Freeze<Mock<ILogger<ServiceDefinitionController>>>();
             _mockLoggerFactory = _fixture.Freeze<Mock<ILoggerFactory>>();
             _mockLoggerFactory
                 .Setup(x => x.CreateLogger(It.IsAny<string>()))
                 .Returns(_mockLogger.Object);

             _serviceDefinitionController = _fixture.Create<ServiceDefinitionController>();
             _serviceDefinitionController.ControllerContext = new ControllerContext
             {
                 HttpContext = _mockHttpContext.Object
             };
         }

         [TestMethod]
         public async Task GetServiceDefinitionsById_WhenCalled_LogsStartingConsultation()
         {
             // Arrange
             _mockServiceDefinitionService
                 .Setup(s => s.GetServiceDefinitionById(Provider,
                     It.Is<string>(id => ServiceDefinitionId.Equals(id, StringComparison.Ordinal)),
                     It.IsAny<UserSession>()))
                 .Returns(Task.FromResult(_successResult));

             // Act
             var actualResponse =
                 await _serviceDefinitionController.GetServiceDefinitionsById(ServiceDefinitionId, Provider);

             // Assert
             actualResponse.Should().BeAssignableTo<OkObjectResult>()
                 .Subject.StatusCode.Should().Be(200);
             _mockLogger.VerifyLogger(LogLevel.Information,
                 $"Starting consultation with ServiceDefinition: {ServiceDefinitionId}. ODSCode: {_userSession.GpUserSession.OdsCode}",
                 Times.Once());
             _mockServiceDefinitionService.Verify(
                 s => s.GetServiceDefinitionById(Provider,
                     It.Is<string>(sdId => ServiceDefinitionId.Equals(sdId, StringComparison.Ordinal)),
                     It.IsAny<UserSession>()),
                 Times.Once);
         }

         [TestMethod]
         [DataRow(null)]
         public async Task EvaluateServiceDefinition_WhenCalledWithoutParametersBody_ReturnsBadRequest(
             Parameters evaluateParameters)
         {
             // Act
             var actualResponse =
                 await _serviceDefinitionController.EvaluateServiceDefinition(Provider, ServiceDefinitionId,
                     evaluateParameters, false);

             // Assert
             actualResponse.Should().BeAssignableTo<BadRequestResult>()
                 .Subject.StatusCode.Should().Be(400);
             _mockServiceDefinitionService.Verify(
                 s => s.EvaluateServiceDefinition(Provider,
                     It.IsAny<string>(),
                     It.IsAny<Parameters>(),
                     It.IsAny<bool>(),
                     It.IsAny<bool>(),
                     _userSession),
                 Times.Never);
         }

         [TestMethod]
         public async Task
             EvaluateServiceDefinition_WhenCalledWithIdParametersAndClientFound_ServiceEvaluateServiceDefinition()
         {
             // Arrange
             _mockServiceDefinitionService
                 .Setup(s => s.EvaluateServiceDefinition(Provider,
                     It.Is<string>(id => ServiceDefinitionId.Equals(id, StringComparison.Ordinal)),
                     It.Is<Parameters>(parameters => _evaluateParameters == parameters),
                     It.IsAny<bool>(),
                     It.IsAny<bool>(),
                     It.IsAny<UserSession>()))
                 .Returns(Task.FromResult(_successResult));

             // Act
             var actualResponse =
                 await _serviceDefinitionController.EvaluateServiceDefinition(Provider, ServiceDefinitionId,
                     _evaluateParameters, false);

             // Assert
             ShouldLogEvaluationWithOdsCode();
             actualResponse.Should().BeAssignableTo<OkObjectResult>()
                 .Subject.StatusCode.Should().Be(200);
             _mockServiceDefinitionService.Verify(
                 s => s.EvaluateServiceDefinition(Provider,
                     It.Is<string>(sdId => ServiceDefinitionId.Equals(sdId, StringComparison.Ordinal)),
                     It.Is<Parameters>(parameters => _evaluateParameters == parameters),
                     It.IsAny<bool>(),
                     It.IsAny<bool>(),
                     It.IsAny<UserSession>()),
                 Times.Once);
         }

         [TestMethod]
         public async Task
             EvaluateServiceDefinition_WhenRequestHasJSDisabledHeader_EvaluateCalledWithAddJSDisabledHeaderTrue()
         {
             // Arrange
             _mockServiceDefinitionService
                 .Setup(s => s.EvaluateServiceDefinition(Provider,
                     It.Is<string>(id => ServiceDefinitionId.Equals(id, StringComparison.Ordinal)),
                     It.Is<Parameters>(parameters => _evaluateParameters == parameters),
                     It.IsAny<bool>(),
                     It.IsAny<bool>(),
                     It.IsAny<UserSession>()))
                 .Returns(Task.FromResult(_successResult));

             // Act
             await _serviceDefinitionController.EvaluateServiceDefinition(Provider, ServiceDefinitionId,
                 _evaluateParameters, false);

             // Assert
             ShouldLogEvaluationWithOdsCode();
             _mockServiceDefinitionService.Verify(
                 s => s.EvaluateServiceDefinition(Provider,
                     It.Is<string>(sdId => ServiceDefinitionId.Equals(sdId, StringComparison.Ordinal)),
                     It.Is<Parameters>(parameters => _evaluateParameters == parameters),
                     It.IsAny<bool>(),
                     It.IsAny<bool>(),
                     It.IsAny<UserSession>()),
                 Times.Once);
         }

         [TestMethod]
         [DataRow(true)]
         [DataRow(false)]
         public async Task EvaluateServiceDefinition_WhenDemographicsConsentQueryStringPresent_ValueIsPassedToService(
             bool demographicsConsentGiven)
         {
             // Arrange
             _mockServiceDefinitionService
                 .Setup(s => s.EvaluateServiceDefinition(Provider,
                     It.Is<string>(id => ServiceDefinitionId.Equals(id, StringComparison.Ordinal)),
                     It.Is<Parameters>(parameters => _evaluateParameters == parameters),
                     It.IsAny<bool>(),
                     It.Is<bool>(consent => consent == demographicsConsentGiven),
                     It.IsAny<UserSession>()))
                 .Returns(Task.FromResult(_successResult));

             // Act
             await _serviceDefinitionController.EvaluateServiceDefinition(Provider, ServiceDefinitionId,
                 _evaluateParameters, demographicsConsentGiven);

             // Assert
             ShouldLogEvaluationWithOdsCode();
             _mockServiceDefinitionService.Verify(
                 s => s.EvaluateServiceDefinition(Provider,
                     It.Is<string>(sdId => ServiceDefinitionId.Equals(sdId, StringComparison.Ordinal)),
                     It.Is<Parameters>(parameters => _evaluateParameters == parameters),
                     It.IsAny<bool>(),
                     It.Is<bool>(consent => consent == demographicsConsentGiven),
                     It.IsAny<UserSession>()),
                 Times.Once);
         }

         private void ShouldLogEvaluationWithOdsCode()
         {
             _mockLogger.VerifyLogger(LogLevel.Information,
                 $"Evaluating ServiceDefinition: {ServiceDefinitionId}. ODSCode: {_userSession.GpUserSession.OdsCode}",
                 Times.Once());
         }
     }
 }