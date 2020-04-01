 using System;
 using System.Linq.Expressions;
 using System.Threading.Tasks;
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
         private Mock<ILogger<ServiceDefinitionController>> _mockLogger;

         private Parameters _evaluateParameters;
         private UserSession _userSession;

         private const string Provider = "OLC Stubs";
         private const string ServiceDefinitionId = "NHS_ADMIN";

         [TestInitialize]
         public void TestInitialize()
         {
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
             Expression<Func<IServiceDefinitionService, Task<ServiceDefinitionResult>>> expectedGetByIdMatch =
                 s => s.GetServiceDefinitionById(Provider, ServiceDefinitionId, _userSession);
             
             _mockServiceDefinitionService
                 .Setup(expectedGetByIdMatch)
                 .ReturnsAsync(new ServiceDefinitionResult.Success(""));

             // Act
             var response =
                 await _serviceDefinitionController.GetServiceDefinitionsById(ServiceDefinitionId, Provider);

             // Assert
             var result = response.Should().BeAssignableTo<OkObjectResult>().Subject;
             result.StatusCode.Should().Be(200);
             result.Value.Should().Be("");

             _mockServiceDefinitionService.Verify(expectedGetByIdMatch, Times.Once);
             _mockLogger.VerifyLogger(
                 LogLevel.Information,
                 $"Starting consultation with ServiceDefinition: {ServiceDefinitionId}. ODSCode: {_userSession.GpUserSession.OdsCode}",
                 Times.Once());
         }

         [TestMethod]
         [DataRow(null)]
         public async Task EvaluateServiceDefinition_WhenCalledWithoutParametersBody_ReturnsBadRequest(
             Parameters evaluateParameters)
         {
             // Arrange
             Expression<Func<IServiceDefinitionService, Task<ServiceDefinitionResult>>> expectedEvaluateMatch =
                 s => s.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<Parameters>(),
                     It.IsAny<bool>(),
                     It.IsAny<bool>(),
                     It.IsAny<UserSession>());

             // Act
             var response =
                 await _serviceDefinitionController.EvaluateServiceDefinition(Provider, ServiceDefinitionId,
                     evaluateParameters, false);

             // Assert
             response.Should().BeAssignableTo<BadRequestResult>()
                 .Subject.StatusCode.Should().Be(400);
             _mockServiceDefinitionService.Verify(expectedEvaluateMatch, Times.Never);
         }

         [TestMethod]
         public async Task
             EvaluateServiceDefinition_WhenCalledWithIdParametersAndClientFound_ServiceEvaluateServiceDefinition()
         {
             // Arrange
             Expression<Func<IServiceDefinitionService, Task<ServiceDefinitionResult>>> expectedEvaluateMatch =
                 s => s.EvaluateServiceDefinition(
                     Provider,
                     ServiceDefinitionId,
                     _evaluateParameters ,
                     false,
                     false,
                     _userSession);
             
             _mockServiceDefinitionService
                 .Setup(expectedEvaluateMatch)
                 .ReturnsAsync(new ServiceDefinitionResult.Success(""));

             // Act
             var response =
                 await _serviceDefinitionController.EvaluateServiceDefinition(Provider, ServiceDefinitionId,
                     _evaluateParameters, false);

             // Assert
             var result = response.Should().BeAssignableTo<OkObjectResult>().Subject;
             result.StatusCode.Should().Be(200);
             result.Value.Should().Be("");

             _mockServiceDefinitionService.Verify(expectedEvaluateMatch, Times.Once);
             ShouldLogEvaluationWithOdsCode();
         }

         [TestMethod]
         public async Task
             EvaluateServiceDefinition_WhenRequestHasJSDisabledHeader_EvaluateCalledWithAddJSDisabledHeaderTrue()
         {
             // Arrange
             Expression<Func<IServiceDefinitionService, Task<ServiceDefinitionResult>>> expectedEvaluateMatch =
                 s => s.EvaluateServiceDefinition(
                     Provider,
                     ServiceDefinitionId,
                     _evaluateParameters,
                     true,
                     false,
                     _userSession);

             _mockServiceDefinitionService
                 .Setup(expectedEvaluateMatch)
                 .ReturnsAsync(new ServiceDefinitionResult.Success(""));

             _mockHttpContext
                 .Setup(x => x.Request.Headers[Constants.HttpHeaders.JavascriptDisabled])
                 .Returns("true");
             
             // Act
             await _serviceDefinitionController.EvaluateServiceDefinition(Provider, ServiceDefinitionId,
                 _evaluateParameters, false);

             // Assert
             _mockServiceDefinitionService.Verify(expectedEvaluateMatch, Times.Once);
             ShouldLogEvaluationWithOdsCode();
         }

         [TestMethod]
         [DataRow(true)]
         [DataRow(false)]
         public async Task EvaluateServiceDefinition_WhenDemographicsConsentQueryStringPresent_ValueIsPassedToService(
             bool demographicsConsentGiven)
         {
             // Arrange
             Expression<Func<IServiceDefinitionService, Task<ServiceDefinitionResult>>> expectedEvaluateMatch =
                 s => s.EvaluateServiceDefinition(Provider,
                     ServiceDefinitionId,
                     _evaluateParameters,
                     false,
                     demographicsConsentGiven,
                     _userSession);

             _mockServiceDefinitionService
                 .Setup(expectedEvaluateMatch)
                 .ReturnsAsync(new ServiceDefinitionResult.Success(""));

             // Act
             await _serviceDefinitionController.EvaluateServiceDefinition(Provider, ServiceDefinitionId,
                 _evaluateParameters, demographicsConsentGiven);

             // Assert
             _mockServiceDefinitionService.Verify(expectedEvaluateMatch, Times.Once);
             ShouldLogEvaluationWithOdsCode();
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_ReturnsNoContent_WhenServiceReturnsSuccessWithValidTrue()
         {
             // Arrange
             var expectedIsValidResult = new ServiceDefinitionIsValidResult.Valid();
             
             Expression<Func<IServiceDefinitionService, Task<ServiceDefinitionIsValidResult>>> expectedEvaluateMatch =
                 s => s.GetServiceDefinitionIsValid(Provider, _userSession);

             _mockServiceDefinitionService
                 .Setup(expectedEvaluateMatch)
                 .ReturnsAsync(expectedIsValidResult);

             // Act
             var response = await _serviceDefinitionController.GetServiceDefinitionIsValid(Provider);

             // Assert
             response.Should().BeAssignableTo<StatusCodeResult>().Subject
                 .StatusCode.Should().Be(204);

             _mockServiceDefinitionService.Verify(expectedEvaluateMatch, Times.Once);
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_ReturnsCustom580_WhenServiceReturnsSuccessWithValidFalse()
         {
             // Arrange
             var expectedIsValidResult = new ServiceDefinitionIsValidResult.Invalid();
             
             Expression<Func<IServiceDefinitionService, Task<ServiceDefinitionIsValidResult>>> expectedEvaluateMatch =
                 s => s.GetServiceDefinitionIsValid(Provider, _userSession);

             _mockServiceDefinitionService
                 .Setup(expectedEvaluateMatch)
                 .ReturnsAsync(expectedIsValidResult);

             // Act
             var response = await _serviceDefinitionController.GetServiceDefinitionIsValid(Provider);

             // Assert
             response.Should().BeAssignableTo<StatusCodeResult>().Subject
                 .StatusCode.Should().Be(580);

             _mockServiceDefinitionService.Verify(expectedEvaluateMatch, Times.Once);
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_ReturnsBadRequest_WhenServiceReturnsBadRequest()
         {
             // Arrange
             Expression<Func<IServiceDefinitionService, Task<ServiceDefinitionIsValidResult>>> expectedEvaluateMatch =
                 s => s.GetServiceDefinitionIsValid(Provider, _userSession);

             _mockServiceDefinitionService
                 .Setup(expectedEvaluateMatch)
                 .ReturnsAsync(new ServiceDefinitionIsValidResult.BadRequest());

             // Act
             var response = await _serviceDefinitionController.GetServiceDefinitionIsValid(Provider);

             // Assert
             var result = response.Should().BeAssignableTo<StatusCodeResult>().Subject;
             result.StatusCode.Should().Be(400);

             _mockServiceDefinitionService.Verify(expectedEvaluateMatch, Times.Once);
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_ReturnsBadGateway_WhenServiceReturnsBadGateway()
         {
             // Arrange
             Expression<Func<IServiceDefinitionService, Task<ServiceDefinitionIsValidResult>>> expectedEvaluateMatch =
                 s => s.GetServiceDefinitionIsValid(Provider, _userSession);

             _mockServiceDefinitionService
                 .Setup(expectedEvaluateMatch)
                 .ReturnsAsync(new ServiceDefinitionIsValidResult.BadGateway());

             // Act
             var response = await _serviceDefinitionController.GetServiceDefinitionIsValid(Provider);

             // Assert
             var result = response.Should().BeAssignableTo<StatusCodeResult>().Subject;
             result.StatusCode.Should().Be(502);

             _mockServiceDefinitionService.Verify(expectedEvaluateMatch, Times.Once);
         }
         
         private void ShouldLogEvaluationWithOdsCode()
         {
             _mockLogger.VerifyLogger(LogLevel.Information,
                 $"Evaluating ServiceDefinition: {ServiceDefinitionId}. ODSCode: {_userSession.GpUserSession.OdsCode}",
                 Times.Once());
         }
     }
 }