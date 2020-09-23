 using System;
 using System.Collections.Generic;
 using System.Linq.Expressions;
 using System.Net;
 using System.Net.Http;
 using System.Text;
 using System.Threading.Tasks;
 using FluentAssertions;
 using Hl7.Fhir.Model;
 using Hl7.Fhir.Serialization;
 using Microsoft.Extensions.Logging;
 using Microsoft.VisualStudio.TestTools.UnitTesting;
 using Moq;
 using NHSOnline.Backend.Auditing;
 using NHSOnline.Backend.GpSystems;
 using NHSOnline.Backend.GpSystems.Demographics;
 using NHSOnline.Backend.GpSystems.Suppliers.Emis;
 using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
 using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models;
 using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition;
 using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
 using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Settings;
 using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
 using NHSOnline.Backend.Support;
 using NHSOnline.Backend.Support.Sanitization;
 using NHSOnline.Backend.Support.Session;
 using UnitTestHelper;
 using Address = Hl7.Fhir.Model.Address;
 using Constants = NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Constants;
 using Task = System.Threading.Tasks.Task;
 using NhsAppFhir = NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models.Fhir;

 namespace NHSOnline.Backend.PfsApi.UnitTests.ClinicalDecisionSupport.ServiceDefinition
 {
     [TestClass]
     public sealed class ServiceDefinitionServiceTests: IDisposable
     {
         private Mock<IHtmlSanitizer> _mockHtmlSanitizer;
         private Mock<IFhirSanitizationHelper> _mockFhirSanitizationHelper;
         private Mock<ILogger<ServiceDefinitionService>> _mockLogger;
         private Mock<ILogger<ServiceDefinitionQuerySender>> _mockSenderLogger;
         private Mock<IMapper<DemographicsResponse, OlcDemographics>> _mockDemographicsOlcMapper;
         private Mock<IAuditor> _mockAuditor;
         private Mock<IGpSystem> _mockGpSystem;
         private Mock<IGpSystemFactory> _mockGpSystemFactory;
         private Mock<IDemographicsService> _mockDemographicsService;
         private Mock<IFhirParameterHelpers> _mockFhirParameterHelpers;
         private Mock<IEvaluateServiceDefinitionQuery> _mockEvaluateServiceDefinitionQuery;
         private Mock<IServiceDefinitionIsValidQuery> _mockServiceDefinitionIsValidQuery;
         private Mock<IGuidCreator> _mockGuidCreator;

         private CitizenIdUserSession _cidUserSession;
         private P9UserSession _userSession;

         private const string OdsCode = "A29928";
         private const string Provider = "stubs";
         private const string ProviderName = "OLC Stubs";
         private const string ServiceDefinitionId = "testId";
         private const string ServiceDefinitionDescription = "test admin help";
         private const string SessionId = "9102fb79-bc0e-465d-b2de-2a724ec876dc";

         private const string GuidanceResponseJsonContent = "{ \"resourceType\" : \"Bundle\", \"status\": \"success\" }";
         private const string BundleJsonContent = "{ \"resourceType\" : \"Bundle\", \"type\": \"searchset\", \"total\": 3 }";
         private const string ParamString =
             "{\"resourceType\":\"Parameters\",\"parameter\":[{\"name\":\"organization\",\"resource\":{\"resourceType\":\"Organization\",\"identifier\":{\"value\":\"111111\"}}},{\"name\":\"sessionId\",\"valueString\":\"" + SessionId + "\"},{\"name\":\"inputData\",\"resource\":{\"resourceType\":\"QuestionnaireResponse\",\"status\":\"completed\",\"item\":[{\"linkId\":\"GLO_PRE_DISCLAIMERS\",\"answer\":[{\"valueCoding\":{\"code\":\"GLO_PRE_DISCLAIMERS_1\"}},{\"valueCoding\":{\"code\":\"GLO_PRE_DISCLAIMERS_2\"}},{\"valueCoding\":{\"code\":\"GLO_PRE_DISCLAIMERS_DEMOGRAPHIC\"}}]}],\"questionnaire\":{\"reference\":\"Questionnaire/GLO_PRE_DISCLAIMERS\"}}}]}";
         private const string SessionEndGuidanceResponse = "{ \"resourceType\": \"GuidanceResponse\", \"contained\": [ { \"resourceType\": \"Parameters\", \"id\": \"outputParams\", \"parameter\": [ { \"name\": \"sessionId\", \"valueString\": \"" + SessionId + "\" } ] }, { \"resourceType\": \"OperationOutcome\", \"id\": \"outcome1\", \"issue\": [ { \"severity\": \"error\", \"code\": \"not-found\", \"details\": { \"coding\": [ { \"system\": \"https://test\", \"code\": \"SESSION_ENDED\", \"display\": \"sessionId test has already ended\" } ] } } ] } ], \"module\": { \"reference\": \"https://test/ServiceDefinition/GEC_ADM\" }, \"status\": \"failure\", \"occurrenceDateTime\": \"2019-12-02T13:38:09.403\", \"evaluationMessage\": [ { \"reference\": \"#outcome1\" } ], \"outputParameters\": { \"reference\": \"#outputParams\" } }";
         private const string IsValidResponseFormat ="{\"resourceType\":\"Parameters\",\"parameter\":[{\"name\":\"{{name}}\",\"{{fhirType}}\":{{value}}}]}";
         private const string InvalidIsValidResponse ="{invalid: format}";

         private readonly Guid _requestId = Guid.NewGuid();
         private FhirJsonSerializer _serializer;
         private HttpResponseMessage _httpResponse;
         private DemographicsResult _demographicsResult;
         private string _serviceDefinitionIsValidParameters;

         private ServiceDefinitionService _service;

         [TestInitialize]
         public void TestInitialize()
         {
             _mockHtmlSanitizer = new Mock<IHtmlSanitizer>();
             _mockFhirSanitizationHelper = new Mock<IFhirSanitizationHelper>();
             _mockLogger = new Mock<ILogger<ServiceDefinitionService>>();
             _mockSenderLogger = new Mock<ILogger<ServiceDefinitionQuerySender>>();
             _mockDemographicsOlcMapper = new Mock<IMapper<DemographicsResponse, OlcDemographics>>();
             _mockAuditor = new Mock<IAuditor>();
             _mockFhirParameterHelpers = new Mock<IFhirParameterHelpers>();
             _mockEvaluateServiceDefinitionQuery = new Mock<IEvaluateServiceDefinitionQuery>();
             _mockServiceDefinitionIsValidQuery = new Mock<IServiceDefinitionIsValidQuery>();
             _mockGuidCreator = new Mock<IGuidCreator>();
             _serializer = new FhirJsonSerializer();

             _demographicsResult = new DemographicsResult.Success(new DemographicsResponse());

             _cidUserSession = new CitizenIdUserSession { OdsCode = OdsCode };
             _userSession = new P9UserSession("csrfToken", "nhsNumber", _cidUserSession, new EmisUserSession(), "im1ConnectionToken");
             _mockGuidCreator.Setup(c => c.CreateGuid()).Returns(_requestId);

             _mockDemographicsService = new Mock<IDemographicsService>();
             _mockDemographicsService
                 .Setup(x => x.GetDemographics(
                     It.Is<GpLinkedAccountModel>(
                         d => d.GpUserSession == _userSession.GpUserSession
                              && d.PatientId == _userSession.GpUserSession.Id)))
                 .Returns(Task.FromResult(_demographicsResult));

             _mockGpSystem = new Mock<IGpSystem>();
             _mockGpSystem
                 .Setup(x => x.GetDemographicsService())
                 .Returns(_mockDemographicsService.Object);

             _mockGpSystemFactory = new Mock<IGpSystemFactory>();
             _mockGpSystemFactory
                 .Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                 .Returns(_mockGpSystem.Object);

             var providersSettings = new OnlineConsultationsProvidersSettings();
             var providerSetting = new OnlineConsultationsProviderSettings
             {
                 Provider = Provider, ProviderName = ProviderName
             };

             var providerSettingsList = new List<OnlineConsultationsProviderSettings> { providerSetting };
             providersSettings.Providers = providerSettingsList;

             _service = new ServiceDefinitionService(
                 _mockLogger.Object,
                 _mockAuditor.Object,
                 _mockGpSystemFactory.Object,
                 _mockFhirParameterHelpers.Object,
                 providersSettings,
                 _mockGuidCreator.Object,
                 new ServiceDefinitionQuerySender(
                     _mockSenderLogger.Object,
                     _mockHtmlSanitizer.Object,
                     _mockFhirSanitizationHelper.Object,
                     _mockFhirParameterHelpers.Object,
                     _mockEvaluateServiceDefinitionQuery.Object,
                     _mockServiceDefinitionIsValidQuery.Object));
         }

         [TestMethod]
         public async Task GetServiceDefinition_WhenQueryThrowsException_ReturnsBadRequest()
         {
             // Arrange
             MockCreateInitialServiceDefinitionEvaluateParameters();
             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<bool>(),
                     null))
                 .Throws<HttpRequestException>();

             // Act
             var response = await _service.GetServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.BadRequest>();
         }

         [TestMethod]
         public async Task GetServiceDefinition_WhenQueryReturnsUnsuccessfulStatusCode_ReturnsBadGateway()
         {
             // Arrange
             MockCreateInitialServiceDefinitionEvaluateParameters();
             MockHttpResponseMessage(HttpStatusCode.BadRequest);
             MockEvaluateServiceDefinitionQueryEvaluateWithNullSessionId();

             // Act
             var response = await _service.GetServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
         }

         [TestMethod]
         public async Task GetServiceDefinition_WhenQueryReturnsNullContent_ReturnsBadGateway()
         {
             // Arrange
             MockCreateInitialServiceDefinitionEvaluateParameters();
             MockHttpResponseMessage(HttpStatusCode.OK);
             MockEvaluateServiceDefinitionQueryEvaluateWithNullSessionId();

             // Act
             var response = await _service.GetServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
         }

         [TestMethod]
         [DataRow("")]
         [DataRow("  ")]
         [DataRow(BundleJsonContent)]
         public async Task GetServiceDefinition_WhenQueryReturnsNonServiceDefinitionContent_ReturnsBadGateway(string content)
         {
             // Arrange
             MockCreateInitialServiceDefinitionEvaluateParameters();
             MockHttpResponseMessage(HttpStatusCode.OK, content);
             MockEvaluateServiceDefinitionQueryEvaluateWithNullSessionId();

             // Act
             var response = await _service.GetServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
             _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeServiceDefinition(
                 It.IsAny<Hl7.Fhir.Model.ServiceDefinition>(), It.IsAny<IHtmlSanitizer>()), Times.Never);
         }

         [TestMethod]
         public async Task GetServiceDefinition_WhenResponseParsedSuccessfully_SanitizesResponseAndReturnsSuccess()
         {
             // Arrange
             MockCreateInitialServiceDefinitionEvaluateParameters();
             MockHttpResponseMessage(HttpStatusCode.OK, GuidanceResponseJsonContent);
             MockEvaluateServiceDefinitionQueryEvaluateWithNullSessionId();

             // Act
             var response = await _service.GetServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.Success>();
             _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeGuidanceResponse(
                 It.IsAny<GuidanceResponse>(), It.IsAny<IHtmlSanitizer>()), Times.Once);
         }

         [TestMethod]
         public async Task GetServiceDefinition_WhenResponseHasSessionId_ThenStartingConsultationIsLogged()
         {
             // Arrange
             MockCreateInitialServiceDefinitionEvaluateParameters();
             MockHttpResponseMessage(HttpStatusCode.OK, SessionEndGuidanceResponse);
             MockEvaluateServiceDefinitionQueryEvaluateWithNullSessionId();

             _mockFhirParameterHelpers
                 .Setup(h => h.GetSessionIdFromParameters(It.IsAny<Parameters>()))
                 .Returns(SessionId);

             // Act
             await _service.GetServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, _userSession);

             // Assert
             _mockSenderLogger.VerifyLogger(
                 LogLevel.Information,
                 $"Starting online consultation for {ServiceDefinitionDescription}. ODSCode: {OdsCode}",
                 Times.Once());
         }

         [TestMethod]
         public async Task GetServiceDefinition_WhenResponseHasNoSessionId_ThenStartingConsultationIsNotLogged()
         {
             // Arrange
             MockCreateInitialServiceDefinitionEvaluateParameters();
             MockHttpResponseMessage(HttpStatusCode.OK, SessionEndGuidanceResponse);
             MockEvaluateServiceDefinitionQueryEvaluateWithNullSessionId();

             // Act
             await _service.GetServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, _userSession);

             // Assert
             _mockSenderLogger.VerifyLogger(
                 LogLevel.Information,
                 $"Starting online consultation for {ServiceDefinitionDescription}. ODSCode: {OdsCode}",
                 Times.Never());
         }

         [TestMethod]
         public void GetProviderName_ReturnsSuccess()
         {
             // Act
             var response = _service.GetProviderName(Provider);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.Success>();
             response.Should().BeAssignableTo<ServiceDefinitionResult.Success>().Subject.Response.Should().Be(ProviderName);
         }

         [TestMethod]
         public async Task EvaluateServiceDefinition_WhenNullParametersProvided_ReturnsBadRequest()
         {
             // Act
             var response = await _service.EvaluateServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, null, false, false, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.BadRequest>();
         }

         [TestMethod]
         public async Task EvaluateServiceDefinition_WhenQueryThrowsException_ReturnsBadRequest()
         {
             // Arrange
             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<bool>(),
                     null))
                 .Throws<HttpRequestException>();

             // Act
             var response = await _service.EvaluateServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, new Parameters(), false, false, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.BadRequest>();
         }

         [TestMethod]
         public async Task EvaluateServiceDefinition_WhenQueryReturnsUnsuccessfulStatusCode_ReturnsBadGateway()
         {
             // Arrange
             MockHttpResponseMessage(HttpStatusCode.BadRequest);
             MockEvaluateServiceDefinitionQueryEvaluateWithNullSessionId();

             // Act
             var response = await _service.EvaluateServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, new Parameters(), false, false, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
         }

         [TestMethod]
         public async Task EvaluateServiceDefinition_WhenQueryReturnsNullContent_ReturnsBadGateway()
         {
             // Arrange
             MockHttpResponseMessage(HttpStatusCode.OK);
             MockEvaluateServiceDefinitionQueryEvaluateWithNullSessionId();

             // Act
             var response = await _service.EvaluateServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, new Parameters(), false, false, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
         }

         [TestMethod]
         [DataRow("")]
         [DataRow("  ")]
         [DataRow(BundleJsonContent)]
         public async Task EvaluateServiceDefinition_WhenQueryReturnsNonGuidanceResponseContent_ReturnsBadGateway(string content)
         {
             // Arrange
             MockHttpResponseMessage(HttpStatusCode.OK, content);
             MockEvaluateServiceDefinitionQueryEvaluateWithNullSessionId();

             // Act
             var response = await _service.EvaluateServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, new Parameters(), false, false, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
             _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeGuidanceResponse(
                 It.IsAny<GuidanceResponse>(), It.IsAny<IHtmlSanitizer>()), Times.Never);
         }

         [TestMethod]
         public async Task EvaluateServiceDefinition_WhenResponseParsedSuccessfully_SanitizesResponseAndReturnsSuccess()
         {
             // Arrange
             MockHttpResponseMessage(HttpStatusCode.OK, GuidanceResponseJsonContent);
             MockEvaluateServiceDefinitionQueryEvaluateWithNullSessionId();

             // Act
             var response = await _service.EvaluateServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, new Parameters(), false, false, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.Success>();
             _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeGuidanceResponse(
                 It.IsAny<GuidanceResponse>(), It.IsAny<IHtmlSanitizer>()), Times.Once);
         }

         [TestMethod]
         public async Task EvaluateServiceDefinition_WhenRequestHasNoSessionIdAndResponseHasSessionId_ThenStartingConsultationIsLogged()
         {
             // Arrange
             MockHttpResponseMessage(HttpStatusCode.OK, GuidanceResponseJsonContent);
             MockEvaluateServiceDefinitionQueryEvaluateWithNullSessionId();

             var requestParameters = new Parameters();

             _mockFhirParameterHelpers
                 .Setup(h => h.GetSessionIdFromParameters(
                     It.Is<Parameters>(p => p != requestParameters)))
                 .Returns(SessionId);

             // Act
             await _service.EvaluateServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, requestParameters, false, false, _userSession);

             // Assert
             _mockSenderLogger.VerifyLogger(
                 LogLevel.Information,
                 $"Starting online consultation for {ServiceDefinitionDescription}. ODSCode: {OdsCode}",
                 Times.Once());
         }

         [TestMethod]
         public async Task EvaluateServiceDefinition_WhenRequestHasNoSessionIdAndResponseHasNoSessionId_ThenStartingConsultationIsNotLogged()
         {
             // Arrange
             MockHttpResponseMessage(HttpStatusCode.OK, GuidanceResponseJsonContent);
             MockEvaluateServiceDefinitionQueryEvaluateWithNullSessionId();

             // Act
             await _service.EvaluateServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, new Parameters(), false, false, _userSession);

             // Assert
             _mockSenderLogger.VerifyLogger(
                 LogLevel.Information,
                 $"Starting online consultation for {ServiceDefinitionDescription}. ODSCode: {OdsCode}",
                 Times.Never());
         }

         [TestMethod]
         public async Task EvaluateServiceDefinition_WhenResponseParsedSuccessfullyAddPatient_SanitizesResponseAndReturnsSuccess()
         {
             // Arrange
             MockHttpResponseMessage(HttpStatusCode.OK, GuidanceResponseJsonContent);

             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.Is<string>(body => body.Contains("\"resourceType\":\"Patient\"", StringComparison.Ordinal)),
                     It.IsAny<bool>(),
                     SessionId))
                 .ReturnsAsync(_httpResponse);

             var olcDemographics = new OlcDemographics
             {
                 NameFull = "Test Test",
                 NhsNumber = "111 111 111",
                 AddressFull = "Test",
                 DateOfBirth = DateTime.UtcNow
             };

             _mockDemographicsOlcMapper.Setup(d => d.Map(It.IsAny<DemographicsResponse>())).Returns(olcDemographics);

             _mockFhirParameterHelpers.Setup(h => h.CreateFhirPatient(
                 It.IsAny<P9UserSession>(), It.IsAny<string>())).Returns(new Patient
             {
                 Address = new List<Address>
                 {
                     new Address
                     {
                         Text = olcDemographics.AddressFull
                     }
                 },
                 BirthDate = olcDemographics.DateOfBirth.ToString(),
                 Name = new List<HumanName>
                 {
                     new HumanName
                     {
                         Text = olcDemographics.NameFull
                     }
                 },
                 Identifier = new List<Identifier>
                 {
                     new Identifier
                     {
                         Value = olcDemographics.NhsNumber
                     }
                 }
             });

             var bodyParameters = (Parameters) new FhirJsonParser().Parse(ParamString);

             _mockFhirParameterHelpers.Setup(h => h.GetSessionIdFromParameters(bodyParameters)).Returns(SessionId);

             // Act
             var response = await _service.EvaluateServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, bodyParameters, false, true ,_userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.Success>();
             _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeGuidanceResponse(
                 It.IsAny<GuidanceResponse>(), It.IsAny<IHtmlSanitizer>()), Times.Once);
         }

         [TestMethod]
         [DataRow(true)]
         [DataRow(false)]
         public async Task EvaluateServiceDefinition_WillPassAddJsDisabledParameterToQuery(bool addJsDisabledHeader)
         {
             // Arrange
             MockHttpResponseMessage(HttpStatusCode.OK, GuidanceResponseJsonContent);

             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.Is<bool>(addHeader => addHeader == addJsDisabledHeader),
                     null))
                 .ReturnsAsync(_httpResponse);

             // Act
             await _service.EvaluateServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, new Parameters(), addJsDisabledHeader, false, _userSession);

             // Assert
             _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeGuidanceResponse(
                 It.IsAny<GuidanceResponse>(), It.IsAny<IHtmlSanitizer>()), Times.Once);
         }

         [TestMethod]
         [DataRow(false)]
         public async Task GetServiceDefinitionResultByError_SessionEnd(bool addJsDisabledHeader)
         {
             // Arrange
             MockHttpResponseMessage(HttpStatusCode.OK, SessionEndGuidanceResponse);

             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.Is<bool>(addHeader => addHeader == addJsDisabledHeader),
                     null))
                 .ReturnsAsync(_httpResponse);

             // Act
             var response = await _service.EvaluateServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, new Parameters(), addJsDisabledHeader, false, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.CustomError>().Subject.ErrorCode.Should()
                 .Be(480);
         }

         [TestMethod]
         public async Task EvaluateServiceDefinition_WillLogEndingConsultationWithOdsCode()
         {
             // Arrange
             MockHttpResponseMessage(HttpStatusCode.OK, GuidanceResponseJsonContent);

             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<bool>(),
                     null))
                 .ReturnsAsync(_httpResponse);

             // Act
             await _service.EvaluateServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, new Parameters(), false, false, _userSession);

             // Assert
             _mockSenderLogger.VerifyLogger(LogLevel.Information, $"Ending consultation for {ServiceDefinitionDescription}. ODSCode: {_userSession.GpUserSession.OdsCode}", Times.Once());
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_WhenProviderReturnsSuccessfulResultWithValidTrue_ReturnsValidResult()
         {
             MockCreateServiceDefinitionIsValidParameters();
             MockHttpResponseMessage(HttpStatusCode.OK, GetIsValidResponseContent());

             Expression<Func<IServiceDefinitionIsValidQuery, Task<HttpResponseMessage>>> serviceDefinitionIsValidMatch =
                 q => q.ServiceDefinitionIsValid(Provider, _serviceDefinitionIsValidParameters);

             _mockServiceDefinitionIsValidQuery
                 .Setup(serviceDefinitionIsValidMatch)
                 .ReturnsAsync(_httpResponse);

             var response = await _service.GetServiceDefinitionIsValid(Provider, _userSession);

             response.Should().BeAssignableTo<ServiceDefinitionIsValidResult.Valid>();
             _mockServiceDefinitionIsValidQuery.Verify(serviceDefinitionIsValidMatch, Times.Once);
             _mockLogger.VerifyLogger(LogLevel.Information, $"$isValid requestId: {_requestId}", Times.Once());
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_WhenProviderReturnsSuccessfulResultWithValidFalse_ReturnsInvalidResult()
         {
             MockCreateServiceDefinitionIsValidParameters();
             MockHttpResponseMessage(HttpStatusCode.OK, GetIsValidResponseContent(value: "false"));

             Expression<Func<IServiceDefinitionIsValidQuery, Task<HttpResponseMessage>>> serviceDefinitionIsValidMatch =
                 q => q.ServiceDefinitionIsValid(Provider, _serviceDefinitionIsValidParameters);

             _mockServiceDefinitionIsValidQuery
                 .Setup(serviceDefinitionIsValidMatch)
                 .ReturnsAsync(_httpResponse);

             var response = await _service.GetServiceDefinitionIsValid(Provider, _userSession);

             response.Should().BeAssignableTo<ServiceDefinitionIsValidResult.Invalid>();
             _mockServiceDefinitionIsValidQuery.Verify(serviceDefinitionIsValidMatch, Times.Once);
             _mockLogger.VerifyLogger(LogLevel.Information, $"$isValid requestId: {_requestId}", Times.Once());
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_WhenResponseReturnTypeIsNotBoolean_ReturnsBadGateway()
         {
             MockCreateServiceDefinitionIsValidParameters();
             MockHttpResponseMessage(HttpStatusCode.OK, GetIsValidResponseContent(fhirType: "valueInteger", value: "1"));

             Expression<Func<IServiceDefinitionIsValidQuery, Task<HttpResponseMessage>>> serviceDefinitionIsValidMatch =
                 q => q.ServiceDefinitionIsValid(Provider, _serviceDefinitionIsValidParameters);

             _mockServiceDefinitionIsValidQuery
                 .Setup(serviceDefinitionIsValidMatch)
                 .ReturnsAsync(_httpResponse);

             var response = await _service.GetServiceDefinitionIsValid(Provider, _userSession);

             response.Should().BeAssignableTo<ServiceDefinitionIsValidResult.BadGateway>();
             _mockServiceDefinitionIsValidQuery.Verify(serviceDefinitionIsValidMatch, Times.Once);
             _mockLogger.VerifyLogger(LogLevel.Information, $"$isValid requestId: {_requestId}", Times.Once());
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_WhenResponseDoesNotContainReturnParameter_ReturnsBadGateway()
         {
             MockCreateServiceDefinitionIsValidParameters();
             MockHttpResponseMessage(HttpStatusCode.OK, GetIsValidResponseContent("somethingElse"));

             Expression<Func<IServiceDefinitionIsValidQuery, Task<HttpResponseMessage>>> serviceDefinitionIsValidMatch =
                 q => q.ServiceDefinitionIsValid(Provider, _serviceDefinitionIsValidParameters);

             _mockServiceDefinitionIsValidQuery
                 .Setup(serviceDefinitionIsValidMatch)
                 .ReturnsAsync(_httpResponse);

             var response = await _service.GetServiceDefinitionIsValid(Provider, _userSession);

             response.Should().BeAssignableTo<ServiceDefinitionIsValidResult.BadGateway>();
             _mockServiceDefinitionIsValidQuery.Verify(serviceDefinitionIsValidMatch, Times.Once);
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_WhenResponseIsNotValidParametersFormat_ReturnsBadGateway()
         {
             MockCreateServiceDefinitionIsValidParameters();
             MockHttpResponseMessage(HttpStatusCode.OK, InvalidIsValidResponse);

             Expression<Func<IServiceDefinitionIsValidQuery, Task<HttpResponseMessage>>> serviceDefinitionIsValidMatch =
                 q => q.ServiceDefinitionIsValid(Provider, _serviceDefinitionIsValidParameters);

             _mockServiceDefinitionIsValidQuery
                 .Setup(serviceDefinitionIsValidMatch)
                 .ReturnsAsync(_httpResponse);

             var response = await _service.GetServiceDefinitionIsValid(Provider, _userSession);

             response.Should().BeAssignableTo<ServiceDefinitionIsValidResult.BadGateway>();
             _mockServiceDefinitionIsValidQuery.Verify(serviceDefinitionIsValidMatch, Times.Once);
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_WhenResponseContentIsNull_ReturnsBadGateway()
         {
             MockCreateServiceDefinitionIsValidParameters();
             MockHttpResponseMessage(HttpStatusCode.OK);

             Expression<Func<IServiceDefinitionIsValidQuery, Task<HttpResponseMessage>>> serviceDefinitionIsValidMatch =
                 q => q.ServiceDefinitionIsValid(Provider, _serviceDefinitionIsValidParameters);

             _mockServiceDefinitionIsValidQuery
                 .Setup(serviceDefinitionIsValidMatch)
                 .ReturnsAsync(_httpResponse);

             var response = await _service.GetServiceDefinitionIsValid(Provider, _userSession);

             response.Should().BeAssignableTo<ServiceDefinitionIsValidResult.BadGateway>();
             _mockServiceDefinitionIsValidQuery.Verify(serviceDefinitionIsValidMatch, Times.Once);
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_WhenResponseHasUnsuccessfulStatusCode_ReturnsBadGateway()
         {
             MockCreateServiceDefinitionIsValidParameters();
             MockHttpResponseMessage(HttpStatusCode.InternalServerError, GetIsValidResponseContent());

             Expression<Func<IServiceDefinitionIsValidQuery, Task<HttpResponseMessage>>> serviceDefinitionIsValidMatch =
                 q => q.ServiceDefinitionIsValid(Provider, _serviceDefinitionIsValidParameters);

             _mockServiceDefinitionIsValidQuery
                 .Setup(serviceDefinitionIsValidMatch)
                 .ReturnsAsync(_httpResponse);

             var response = await _service.GetServiceDefinitionIsValid(Provider, _userSession);

             response.Should().BeAssignableTo<ServiceDefinitionIsValidResult.BadGateway>();
             _mockServiceDefinitionIsValidQuery.Verify(serviceDefinitionIsValidMatch, Times.Once);
         }

         [TestMethod]
         public async Task GetServiceDefinitionIsValid_WhenQueryThrowsHttpRequestException_ReturnsBadRequest()
         {
             MockCreateServiceDefinitionIsValidParameters();
             Expression<Func<IServiceDefinitionIsValidQuery, Task<HttpResponseMessage>>> serviceDefinitionIsValidMatch =
                 q => q.ServiceDefinitionIsValid(Provider, _serviceDefinitionIsValidParameters);

             _mockServiceDefinitionIsValidQuery
                 .Setup(serviceDefinitionIsValidMatch)
                 .Throws(new HttpRequestException());

             var response = await _service.GetServiceDefinitionIsValid(Provider, _userSession);

             response.Should().BeAssignableTo<ServiceDefinitionIsValidResult.BadRequest>();
             _mockServiceDefinitionIsValidQuery.Verify(serviceDefinitionIsValidMatch, Times.Once);
         }

         private void MockCreateInitialServiceDefinitionEvaluateParameters()
         {
             _mockFhirParameterHelpers
                 .Setup(h => h.CreateInitialServiceDefinitionEvaluateParameters(_userSession.OdsCode))
                 .Returns(new NhsAppFhir.Parameters());
         }

         private void MockCreateServiceDefinitionIsValidParameters()
         {
             var parameters = new Parameters();

             _serviceDefinitionIsValidParameters = _serializer.SerializeToString(parameters);
             _mockFhirParameterHelpers
                 .Setup(h => h.CreateServiceDefinitionIsValidParameters(_userSession.OdsCode, _requestId.ToString()))
                 .Returns(parameters);
         }

         private void MockHttpResponseMessage(HttpStatusCode statusCode, string content = null)
         {
             _httpResponse = new HttpResponseMessage
             {
                 StatusCode = statusCode,
                 Content = content is null
                     ? null
                     : new StringContent(content, Encoding.UTF8, Constants.ContentTypes.ApplicationJsonFhir)
             };
         }

         private void MockEvaluateServiceDefinitionQueryEvaluateWithNullSessionId()
         {
             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<bool>(),
                     null))
                 .ReturnsAsync(_httpResponse);
         }

         private static string GetIsValidResponseContent(
             string name = "return",
             string fhirType = "valueBoolean",
             string value = "true") =>
             IsValidResponseFormat
                 .Replace("{{name}}", name, StringComparison.Ordinal)
                 .Replace("{{fhirType}}", fhirType, StringComparison.Ordinal)
                 .Replace("{{value}}", value, StringComparison.Ordinal);

         public void Dispose() => _httpResponse?.Dispose();
     }
 }