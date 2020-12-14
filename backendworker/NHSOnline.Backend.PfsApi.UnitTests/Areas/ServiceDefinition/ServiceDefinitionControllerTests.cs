 using System;
 using System.Linq.Expressions;
 using System.Threading.Tasks;
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
 using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
 using NHSOnline.Backend.Support;
 using NHSOnline.Backend.Support.Session;
 using UnitTestHelper;
 using Task = System.Threading.Tasks.Task;
 using ServiceDefinitionTypeEnum = NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models.ServiceDefinitionType;

 namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.ServiceDefinition
 {
     [TestClass]
     public sealed class ServiceDefinitionControllerTests: IDisposable
     {
         private ServiceDefinitionController _systemUnderTest;

         private Parameters _evaluateParameters;
         private P9UserSession _userSession;

         private Mock<IServiceDefinitionService> _mockServiceDefinitionService;
         private Mock<HttpContext> _mockHttpContext;
         private Mock<ILogger<ServiceDefinitionController>> _mockLogger;
         private Mock<IFhirParameterHelpers> _mockFhirParameterHelpers;

         private const string Provider = "olcStubs";
         private const string ProviderName = "Olc Stubs";
         private const string ServiceDefinitionId = "NHS_ADMIN";
         private const string ServiceDefinitionType = "AdminHelp";
         private const string ServiceDefinitionDescriptionV1 = "an unknown journey";
         private const string ServiceDefinitionDescriptionV2 = "admin help";

         [TestInitialize]
         public void TestInitialize()
         {
             _userSession = new P9UserSession("csrfToken", "nhsNumber", new CitizenIdUserSession(), new EmisUserSession(), "im1token");
             _evaluateParameters = new Parameters();

             _mockServiceDefinitionService = new Mock<IServiceDefinitionService>();
             _mockFhirParameterHelpers = new Mock<IFhirParameterHelpers>();
             _mockHttpContext = new Mock<HttpContext>();
             _mockLogger = new Mock<ILogger<ServiceDefinitionController>>();

             var mockRequest = new Mock<HttpRequest>();

             mockRequest.Setup(x => x.Headers).Returns(new Mock<IHeaderDictionary>().Object);
             _mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);

             _systemUnderTest = new ServiceDefinitionController(
                 _mockServiceDefinitionService.Object,
                 _mockFhirParameterHelpers.Object,
                 _mockLogger.Object)
             {
                 ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object }
             };
         }

         [TestMethod]
         public async Task GetServiceDefinition_WhenServiceGetServiceDefinitionReturnsSuccessfulResult_ThenResultIsReturnedAndStatusCodeIs200()
         {
             // Arrange
             MockServiceGetServiceDefinition(new ServiceDefinitionResult.Success("this is a v2 service definition"),
                 _userSession, description: ServiceDefinitionDescriptionV2);
             var serviceDefinitionMetaData = new ServiceDefinitionMetaData
             {
                 Id = ServiceDefinitionId,
                 Type = ServiceDefinitionTypeEnum.AdminHelp
             };

             // Act
             var response = await _systemUnderTest.GetServiceDefinition(serviceDefinitionMetaData,
                 Provider, _userSession);

             // Assert
             var result = response.Should().BeAssignableTo<OkObjectResult>().Subject;
             result.StatusCode.Should().Be(200);
             result.Value.Should().Be("this is a v2 service definition");
         }

         [TestMethod]
         public async Task GetServiceDefinition_WhenServiceGetServiceDefinitionV2ReturnsSuccessfulResult_ThenResultIsReturnedAndStatusCodeIs200()
         {

            // Arrange
            MockServiceGetServiceDefinition(new ServiceDefinitionResult.Success("this is a v2 service definition"),
                _userSession, description: ServiceDefinitionDescriptionV2, version: "2");
            var serviceDefinitionMetaData = new ServiceDefinitionMetaData
            {
                Id = ServiceDefinitionId,
                Type = ServiceDefinitionTypeEnum.AdminHelp
            };

            // Act
            var response = await _systemUnderTest.GetServiceDefinitionV2(serviceDefinitionMetaData,
                Provider, _userSession);

            // Assert
            var result = response.Should().BeAssignableTo<OkObjectResult>().Subject;
            result.StatusCode.Should().Be(200);
            result.Value.Should().Be("this is a v2 service definition");
         }

         [TestMethod]
         public async Task EvaluateServiceDefinition_WhenBodyIsNull_ThenBadRequestIsReturned()
         {
             // Act
             var response = await _systemUnderTest.EvaluateServiceDefinition(
                 Provider,
                 null,
                 false,
                 _userSession);

             // Assert
             response.Should().BeAssignableTo<BadRequestResult>()
                 .Subject.StatusCode.Should().Be(400);
         }

         [TestMethod]
         public async Task EvaluateServiceDefinition_WhenCalled_ThenEvaluatingConsultationIsLogged()
         {
             // Arrange
             AddServiceDefinitionMetaDataToParameters();
             MockServiceDefinitionParametersHelper();

             MockServiceEvaluateServiceDefinition(new ServiceDefinitionResult.Success("this is a v2 evaluate response"),
                 _userSession, _evaluateParameters, description: ServiceDefinitionDescriptionV2);

             // Act
             var response = await _systemUnderTest.EvaluateServiceDefinition(
                 Provider,
                 _evaluateParameters,
                 false,
                 _userSession);

             // Assert
             VerifyEvaluationWithOdsCodeIsLogged(ServiceDefinitionDescriptionV2);
         }

         [TestMethod]
         public async Task EvaluateServiceDefinitionV2_WhenCalled_ThenEvaluatingConsultationIsLogged()
         {
             // Arrange
             AddServiceDefinitionMetaDataToParameters();
             MockServiceDefinitionParametersHelper();

             MockServiceEvaluateServiceDefinition(new ServiceDefinitionResult.Success("this is a v2 evaluate response"),
                 _userSession, _evaluateParameters, description: ServiceDefinitionDescriptionV2, version: "2");

             // Act
             var response = await _systemUnderTest.EvaluateServiceDefinitionV2(
                 Provider,
                 _evaluateParameters,
                 false,
                 _userSession);

             // Assert
             VerifyEvaluationWithOdsCodeIsLogged(ServiceDefinitionDescriptionV2);
         }

         [TestMethod]
         public async Task EvaluateServiceDefinition_WhenServiceEvaluateReturnsSuccessfulResult_ThenResultIsReturnedAndStatusCodeIs200()
         {
             // Arrange
             AddServiceDefinitionMetaDataToParameters();
             MockServiceDefinitionParametersHelper();

             MockServiceEvaluateServiceDefinition(new ServiceDefinitionResult.Success("this is another v2 evaluate response"),
                 _userSession, _evaluateParameters, description: ServiceDefinitionDescriptionV2);

             // Act
             var response = await _systemUnderTest.EvaluateServiceDefinition(
                 Provider,
                 _evaluateParameters,
                 false,
                 _userSession);

             // Assert
             var result = response.Should().BeAssignableTo<OkObjectResult>().Subject;
             result.StatusCode.Should().Be(200);
             result.Value.Should().Be("this is another v2 evaluate response");
         }

         [TestMethod]
         public async Task EvaluateServiceDefinition_WhenRequestHasJSDisabledHeader_ThenServiceEvaluateIsCalledWithAddJSDisabledHeaderTrue()
         {
             // Arrange
             AddServiceDefinitionMetaDataToParameters();
             MockServiceDefinitionParametersHelper();

             MockServiceEvaluateServiceDefinition(new ServiceDefinitionResult.Success("this is yet another evaluate response"),
                 _userSession, _evaluateParameters, description: ServiceDefinitionDescriptionV2, jsDisabled: true);

             // Act
             _mockHttpContext
                 .Setup(x => x.Request.Headers[Constants.HttpHeaders.JavascriptDisabled])
                 .Returns("true");

             await _systemUnderTest.EvaluateServiceDefinition(Provider, _evaluateParameters,
                 false, _userSession);

             // Assert
             _mockServiceDefinitionService.Verify();
         }

         [TestMethod]
         [DataRow(true)]
         [DataRow(false)]
         public async Task EvaluateServiceDefinition_WhenDemographicsConsentQueryIsSet_ThenValueIsPassedToServiceEvaluate(
             bool demographicsConsentGiven)
         {
             // Arrange
             AddServiceDefinitionMetaDataToParameters();
             MockServiceDefinitionParametersHelper();

             MockServiceEvaluateServiceDefinition(new ServiceDefinitionResult.Success("this is yet again another evaluate response"),
                 _userSession, _evaluateParameters, description: ServiceDefinitionDescriptionV2, demographicsConsentGiven: demographicsConsentGiven);

             // Act
             await _systemUnderTest.EvaluateServiceDefinition(Provider, _evaluateParameters,
                 demographicsConsentGiven, _userSession);

             // Assert
             _mockServiceDefinitionService.Verify();
         }

         [DataTestMethod]
         public async Task GetServiceDefinitionIsValid_WhenServiceIsValidReturnsSuccessWithValidTrue_Then204IsReturned()
         {
             // Arrange
             MockServiceGetServiceDefinitionIsValid(new ServiceDefinitionIsValidResult.Valid(), _userSession);

             // Act
             var response = await _systemUnderTest.GetServiceDefinitionIsValid(Provider, _userSession);

             // Assert
             response.Should().BeAssignableTo<StatusCodeResult>().Subject
                 .StatusCode.Should().Be(204);
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_WhenServiceIsValidReturnsSuccessWithValidFalse_Then580IsReturned()
         {
             // Arrange
             MockServiceGetServiceDefinitionIsValid(new ServiceDefinitionIsValidResult.Invalid(), _userSession);

             // Act
             var response = await _systemUnderTest.GetServiceDefinitionIsValid(Provider, _userSession);

             // Assert
             response.Should().BeAssignableTo<StatusCodeResult>().Subject
                 .StatusCode.Should().Be(580);
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_WhenServiceIsValidReturnsBadRequest_ThenBadRequestIsReturned()
         {
             // Arrange
             MockServiceGetServiceDefinitionIsValid(new ServiceDefinitionIsValidResult.BadRequest(), _userSession);

             // Act
             var response = await _systemUnderTest.GetServiceDefinitionIsValid(Provider, _userSession);

             // Assert
             response.Should().BeAssignableTo<StatusCodeResult>().Subject
                 .StatusCode.Should().Be(400);
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_WhenServiceIsValidReturnsBadGateway_ThenBadGatewayIsReturned()
         {
             // Arrange
             MockServiceGetServiceDefinitionIsValid(new ServiceDefinitionIsValidResult.BadGateway(), _userSession);

             // Act
             var response = await _systemUnderTest.GetServiceDefinitionIsValid(Provider, _userSession);

             // Assert
             response.Should().BeAssignableTo<StatusCodeResult>().Subject
                 .StatusCode.Should().Be(502);
         }

         [TestMethod]
         public void GetProviderDetails_WhenServiceReturnsProviderDetails_ThenProviderDetailsAreReturned()
         {
             // Arrange
             MockServiceGetProviderName(new ServiceDefinitionResult.Success(ProviderName));

             // Act
             var response = _systemUnderTest.GetProviderDetails(Provider);

             // Assert
             var result = response.Should().BeAssignableTo<OkObjectResult>().Subject;
             result.StatusCode.Should().Be(200);
             result.Value.Should().Be(ProviderName);
         }

         private void MockServiceGetProviderName(ServiceDefinitionResult result, string provider = Provider)
         {
             Expression<Func<IServiceDefinitionService, ServiceDefinitionResult>> expectedEvaluateMatch =
                 s => s.GetProviderName(provider);

             _mockServiceDefinitionService
                 .Setup(expectedEvaluateMatch)
                 .Returns(result)
                 .Verifiable();
         }

         private void MockServiceGetServiceDefinition(ServiceDefinitionResult result, P9UserSession userSession, string provider = Provider, string serviceDefinitionId = ServiceDefinitionId,
             string description = ServiceDefinitionDescriptionV1, string version = "1")
         {
             Expression<Func<IServiceDefinitionService, Task<ServiceDefinitionResult>>> expectedGetByIdMatch =
                 s => s.GetServiceDefinition(provider, serviceDefinitionId, description, userSession, version);

             _mockServiceDefinitionService
                 .Setup(expectedGetByIdMatch)
                 .ReturnsAsync(result)
                 .Verifiable();
         }

         private void MockServiceEvaluateServiceDefinition(ServiceDefinitionResult result, P9UserSession userSession, Parameters parameters,
             string provider = Provider, string serviceDefinitionId = ServiceDefinitionId, string description = ServiceDefinitionDescriptionV1,
             bool jsDisabled = false, bool demographicsConsentGiven = false, string version ="1")
         {
             Expression<Func<IServiceDefinitionService, Task<ServiceDefinitionResult>>> expectedEvaluateMatch =
                 s => s.EvaluateServiceDefinition(
                     provider,
                     serviceDefinitionId,
                     description,
                     parameters,
                     jsDisabled,
                     demographicsConsentGiven,
                     userSession,
                     version);

             _mockServiceDefinitionService
                 .Setup(expectedEvaluateMatch)
                 .ReturnsAsync(result)
                 .Verifiable();
         }

         private void MockServiceGetServiceDefinitionIsValid(ServiceDefinitionIsValidResult result, P9UserSession userSession,
             string provider = Provider)
         {
             _mockServiceDefinitionService
                 .Setup(s => s.GetServiceDefinitionIsValid(provider, userSession))
                 .ReturnsAsync(result)
                 .Verifiable();
         }

         private void MockServiceDefinitionParametersHelper()
         {
             var metaData = new ServiceDefinitionMetaData
             {
                 Id = ServiceDefinitionId,
                 Type = ServiceDefinitionTypeEnum.AdminHelp
             };

             _mockFhirParameterHelpers
                 .Setup(h => h.RemoveServiceDefinitionMetadataFromParameters(_evaluateParameters, out metaData))
                 .Returns(_evaluateParameters)
                 .Verifiable();
         }

         private void VerifyEvaluationWithOdsCodeIsLogged(string description)
         {
             _mockLogger.VerifyLogger(LogLevel.Information,
                 $"Evaluating for {description}. ODSCode: {_userSession.GpUserSession.OdsCode}",
                 Times.Once());
         }

         private void AddServiceDefinitionMetaDataToParameters()
         {
             _evaluateParameters
                 .Add("nhsAppServiceDefinitionId", new FhirString(ServiceDefinitionId))
                 .Add("nhsAppServiceDefinitionType", new FhirString(ServiceDefinitionType));
         }

         public void Dispose() => _systemUnderTest?.Dispose();
     }
 }