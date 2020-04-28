 using System;
 using System.Collections.Generic;
 using System.Linq.Expressions;
 using System.Net;
 using System.Net.Http;
 using System.Text;
 using System.Threading.Tasks;
 using AutoFixture;
 using AutoFixture.AutoMoq;
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
         private IFixture _fixture;
         private Mock<IHtmlSanitizer> _mockHtmlSanitizer;
         private Mock<IFhirSanitizationHelper> _mockFhirSanitizationHelper;
         private Mock<ILogger<ServiceDefinitionService>> _mockLogger;
         private Mock<IMapper<DemographicsResponse, OlcDemographics>> _mockDemographicsOlcMapper;
         private Mock<IAuditor> _mockAuditor;
         private P9UserSession _userSession;
         private Mock<IGpSystem> _mockGpSystem;
         private Mock<IGpSystemFactory> _mockGpSystemFactory;
         private Mock<IDemographicsService> _mockDemographicsService;
         private Mock<IFhirParameterHelpers> _mockFhirParameterHelpers;
         private Mock<IEvaluateServiceDefinitionQuery> _mockEvaluateServiceDefinitionQuery;
         private Mock<IServiceDefinitionIsValidQuery> _mockServiceDefinitionIsValidQuery;
         private Mock<IGuidCreator> _mockGuidCreator;

         private const string Provider = "stubs";
         private const string ProviderName = "OLC Stubs";
         private const string ServiceDefinitionId = "testId";
         private const string ServiceDefinitionDescription = "test admin help";

         private const string GuidanceResponseJsonContent = "{ \"resourceType\" : \"Bundle\", \"status\": \"success\" }";
         private const string BundleJsonContent = "{ \"resourceType\" : \"Bundle\", \"type\": \"searchset\", \"total\": 3 }";
         private const string ParamString =
             "{\"resourceType\":\"Parameters\",\"parameter\":[{\"name\":\"organization\",\"resource\":{\"resourceType\":\"Organization\",\"identifier\":{\"value\":\"111111\"}}},{\"name\":\"sessionId\",\"valueString\":\"9102fb79-bc0e-465d-b2de-2a724ec876dc\"},{\"name\":\"inputData\",\"resource\":{\"resourceType\":\"QuestionnaireResponse\",\"status\":\"completed\",\"item\":[{\"linkId\":\"GLO_PRE_DISCLAIMERS\",\"answer\":[{\"valueCoding\":{\"code\":\"GLO_PRE_DISCLAIMERS_1\"}},{\"valueCoding\":{\"code\":\"GLO_PRE_DISCLAIMERS_2\"}},{\"valueCoding\":{\"code\":\"GLO_PRE_DISCLAIMERS_DEMOGRAPHIC\"}}]}],\"questionnaire\":{\"reference\":\"Questionnaire/GLO_PRE_DISCLAIMERS\"}}}]}";
         private const string SessionEndGuidanceResponse = "{ \"resourceType\": \"GuidanceResponse\", \"contained\": [ { \"resourceType\": \"Parameters\", \"id\": \"outputParams\", \"parameter\": [ { \"name\": \"sessionId\", \"valueString\": \"test\" } ] }, { \"resourceType\": \"OperationOutcome\", \"id\": \"outcome1\", \"issue\": [ { \"severity\": \"error\", \"code\": \"not-found\", \"details\": { \"coding\": [ { \"system\": \"https://test\", \"code\": \"SESSION_ENDED\", \"display\": \"sessionId test has already ended\" } ] } } ] } ], \"module\": { \"reference\": \"https://test/ServiceDefinition/GEC_ADM\" }, \"status\": \"failure\", \"occurrenceDateTime\": \"2019-12-02T13:38:09.403\", \"evaluationMessage\": [ { \"reference\": \"#outcome1\" } ], \"outputParameters\": { \"reference\": \"#outputParams\" } }";
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
             _fixture = new Fixture()
                 .Customize(new AutoMoqCustomization());
             _mockHtmlSanitizer = new Mock<IHtmlSanitizer>();
             _mockFhirSanitizationHelper = new Mock<IFhirSanitizationHelper>();
             _mockLogger = new Mock<ILogger<ServiceDefinitionService>>();
             _mockDemographicsOlcMapper = new Mock<IMapper<DemographicsResponse, OlcDemographics>>();
             _mockAuditor = new Mock<IAuditor>();
             _mockFhirParameterHelpers = new Mock<IFhirParameterHelpers>();
             _mockEvaluateServiceDefinitionQuery = new Mock<IEvaluateServiceDefinitionQuery>();
             _mockServiceDefinitionIsValidQuery = new Mock<IServiceDefinitionIsValidQuery>();
             _mockGuidCreator = new Mock<IGuidCreator>();
             _serializer = new FhirJsonSerializer();

             _demographicsResult = new DemographicsResult.Success(_fixture.Create<DemographicsResponse>());

             _fixture.Customize<P9UserSession>(c => c
                 .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

             _userSession = _fixture.Create<P9UserSession>();
             _mockGuidCreator.Setup(c => c.CreateGuid()).Returns(_requestId);

             _mockDemographicsService = _fixture.Freeze<Mock<IDemographicsService>>();

             _mockDemographicsService
                 .Setup(x => x.GetDemographics(
                     It.Is<GpLinkedAccountModel>(
                         d => d.GpUserSession == _userSession.GpUserSession
                              && d.PatientId == _userSession.GpUserSession.Id)))
                 .Returns(Task.FromResult(_demographicsResult));

             _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
             _mockGpSystem
                 .Setup(x => x.GetDemographicsService())
                 .Returns(_mockDemographicsService.Object);

             _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
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
                 _mockHtmlSanitizer.Object,
                 _mockFhirSanitizationHelper.Object,
                 _mockDemographicsOlcMapper.Object,
                 _mockAuditor.Object,
                 _mockGpSystemFactory.Object,
                 _mockFhirParameterHelpers.Object,
                 providersSettings,
                 _mockEvaluateServiceDefinitionQuery.Object,
                 _mockServiceDefinitionIsValidQuery.Object,
                 _mockGuidCreator.Object
             );
         }

         [TestMethod]
         public async Task GetServiceDefinitionById_WhenQueryThrowsException_ReturnsBadRequest()
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
             var response = await _service.GetServiceDefinitionById(Provider, ServiceDefinitionId, ServiceDefinitionDescription, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.BadRequest>();
         }

         [TestMethod]
         public async Task GetServiceDefinitionById_WhenQueryReturnsUnsuccessfulStatusCode_ReturnsBadGateway()
         {
             // Arrange
             MockCreateInitialServiceDefinitionEvaluateParameters();
             MockHttpResponseMessage(HttpStatusCode.BadRequest);

             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<bool>(),
                     null))
                 .ReturnsAsync(_httpResponse);

             // Act
             var response = await _service.GetServiceDefinitionById(Provider, ServiceDefinitionId, ServiceDefinitionDescription, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
         }

         [TestMethod]
         public async Task GetServiceDefinitionById_WhenQueryReturnsNullContent_ReturnsBadGateway()
         {
             // Arrange
             MockCreateInitialServiceDefinitionEvaluateParameters();
             MockHttpResponseMessage(HttpStatusCode.OK);

             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<bool>(),
                     null))
                 .ReturnsAsync(_httpResponse);

             // Act
             var response = await _service.GetServiceDefinitionById(Provider, ServiceDefinitionId, ServiceDefinitionDescription, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
         }

         [TestMethod]
         [DataRow("")]
         [DataRow("  ")]
         [DataRow(BundleJsonContent)]
         public async Task GetServiceDefinitionById_WhenQueryReturnsNonServiceDefinitionContent_ReturnsBadGateway(string content)
         {
             // Arrange
             MockCreateInitialServiceDefinitionEvaluateParameters();
             MockHttpResponseMessage(HttpStatusCode.OK, content);

             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<bool>(),
                     null))
                 .ReturnsAsync(_httpResponse);

             // Act
             var response = await _service.GetServiceDefinitionById(Provider, ServiceDefinitionId, ServiceDefinitionDescription, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.BadGateway>();
             _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeServiceDefinition(
                 It.IsAny<Hl7.Fhir.Model.ServiceDefinition>(), It.IsAny<IHtmlSanitizer>()), Times.Never);
         }

         [TestMethod]
         public async Task GetServiceDefinitionById_WhenResponseParsedSuccessfully_SanitizesResponseAndReturnsSuccess()
         {
             // Arrange
             MockCreateInitialServiceDefinitionEvaluateParameters();
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
             var response = await _service.GetServiceDefinitionById(Provider, ServiceDefinitionId, ServiceDefinitionDescription, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.Success>();
             _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeGuidanceResponse(
                 It.IsAny<GuidanceResponse>(), It.IsAny<IHtmlSanitizer>()), Times.Once);
         }

         [TestMethod]
         public void GetProviderName_ReturnsSuccess()
         {
             // Act
             var response = _service.GetProviderName(Provider);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.Success>();
             response.Should().BeAssignableTo<ServiceDefinitionResult.Success>()
                 .Subject.Response.Equals(ProviderName, StringComparison.Ordinal);
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

             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<bool>(),
                     null))
                 .ReturnsAsync(_httpResponse);

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

             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<bool>(),
                     null))
                 .ReturnsAsync(_httpResponse);

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

             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<bool>(),
                     null))
                 .ReturnsAsync(_httpResponse);

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

             _mockEvaluateServiceDefinitionQuery
                 .Setup(a => a.EvaluateServiceDefinition(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<bool>(),
                     null))
                 .ReturnsAsync(_httpResponse);

             // Act
             var response = await _service.EvaluateServiceDefinition(Provider, ServiceDefinitionId, ServiceDefinitionDescription, new Parameters(), false, false, _userSession);

             // Assert
             response.Should().BeAssignableTo<ServiceDefinitionResult.Success>();
             _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeGuidanceResponse(
                 It.IsAny<GuidanceResponse>(), It.IsAny<IHtmlSanitizer>()), Times.Once);
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
                     "9102fb79-bc0e-465d-b2de-2a724ec876dc"))
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
                 It.IsAny<IMapper<DemographicsResponse, OlcDemographics>>(),
                 It.IsAny<DemographicsResult.Success>())).Returns(new Patient
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

             var fhirParser = new FhirJsonParser();
             var paramsParsed = fhirParser.Parse(ParamString);

             var bodyParameters = (Parameters) paramsParsed;

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
             _mockLogger.VerifyLogger(LogLevel.Information, $"Ending consultation for {ServiceDefinitionDescription}. ODSCode: {_userSession.GpUserSession.OdsCode}", Times.Once());
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
             MockHttpResponseMessage(HttpStatusCode.OK, null);

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
                 .Setup(h => h.CreateInitialServiceDefinitionEvaluateParameters(_userSession.GpUserSession.OdsCode))
                 .Returns(new NhsAppFhir.Parameters());
         }

         private void MockCreateServiceDefinitionIsValidParameters()
         {
             var parameters = new Parameters();

             _serviceDefinitionIsValidParameters = _serializer.SerializeToString(parameters);
             _mockFhirParameterHelpers
                 .Setup(h => h.CreateServiceDefinitionIsValidParameters(_userSession.GpUserSession.OdsCode, _requestId.ToString()))
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