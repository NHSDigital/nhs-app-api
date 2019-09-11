using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
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
using UnitTestHelper;
using Address = Hl7.Fhir.Model.Address;
using Constants = NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Constants;
using Task = System.Threading.Tasks.Task;

namespace NHSOnline.Backend.PfsApi.UnitTests.ClinicalDecisionSupport.ServiceDefinition
{
    [TestClass]
    public class ServiceDefinitionServiceTests
    {
        private IFixture _fixture;
        private Mock<IHtmlSanitizer> _mockHtmlSanitizer;
        private Mock<IFhirSanitizationHelper> _mockFhirSanitizationHelper;
        private Mock<ILogger<ServiceDefinitionService>> _mockLogger;
        private Mock<IOnlineConsultationsProviderHttpClient> _mockProviderHttpClient;
        private Mock<IMapper<DemographicsResponse, OlcDemographics>> _mockDemographicsOlcMapper;
        private Mock<IAuditor> _mockAuditor;
        private UserSession _userSession;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IDemographicsService> _mockDemographicsService;
        private Mock<ICreateFhirParameter> _mockCreateFhirParam;
        private Mock<IServiceDefinitionListBuilder> _mockServiceDefinitionListBuilder;
        
        private const string ServiceDefinitionId = "testId";
        private const string GuidanceResponseJsonContent = "{ \"resourceType\" : \"Bundle\" }";
        private const string ServiceDefinitionJsonContent = "{ \"resourceType\" : \"ServiceDefinition\", \"contained\": [ { \"resourceType\": \"Questionnaire\" } ], \"publisher\" : \"eConsultHealthLtd\"  }";
        private const string BundleJsonContent = "{ \"resourceType\" : \"Bundle\", \"type\": \"searchset\", \"total\": 3 }";
        private const string paramString =
            "{\"resourceType\":\"Parameters\",\"parameter\":[{\"name\":\"organization\",\"resource\":{\"resourceType\":\"Organization\",\"identifier\":{\"value\":\"111111\"}}},{\"name\":\"inputData\",\"resource\":{\"resourceType\":\"QuestionnaireResponse\",\"status\":\"completed\",\"item\":[{\"linkId\":\"GLO_PRE_DISCLAIMERS\",\"answer\":[{\"valueCoding\":{\"code\":\"GLO_PRE_DISCLAIMERS_1\"}},{\"valueCoding\":{\"code\":\"GLO_PRE_DISCLAIMERS_2\"}},{\"valueCoding\":{\"code\":\"GLO_PRE_DISCLAIMERS_DEMOGRAPHIC\"}}]}],\"questionnaire\":{\"reference\":\"Questionnaire/GLO_PRE_DISCLAIMERS\"}}}]}";

        private ServiceDefinitionService _service;
        private DemographicsResult _demographicsResult;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            _mockHtmlSanitizer = new Mock<IHtmlSanitizer>();
            _mockFhirSanitizationHelper = new Mock<IFhirSanitizationHelper>();
            _mockLogger = new Mock<ILogger<ServiceDefinitionService>>();
            _mockProviderHttpClient = new Mock<IOnlineConsultationsProviderHttpClient>();
            _mockDemographicsOlcMapper = new Mock<IMapper<DemographicsResponse, OlcDemographics>>();
            _mockAuditor = new Mock<IAuditor>();
            _mockCreateFhirParam = new Mock<ICreateFhirParameter>();
            
            _demographicsResult = new DemographicsResult.Success(_fixture.Create<DemographicsResponse>());
            
            _mockServiceDefinitionListBuilder = new Mock<IServiceDefinitionListBuilder>();
            
            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));
            _userSession = _fixture.Create<UserSession>();

            _mockDemographicsService = _fixture.Freeze<Mock<IDemographicsService>>();

            _mockDemographicsService
                .Setup(x => x.GetDemographics(_userSession.GpUserSession))
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
                Provider = "eConsult", ProviderName = "eConsult"
            };

            var providerSettingsList = new List<OnlineConsultationsProviderSettings> { providerSetting };
            providersSettings.Providers = providerSettingsList;

            _service = new ServiceDefinitionService(
                _mockLogger.Object,
                _mockServiceDefinitionListBuilder.Object,
                _mockHtmlSanitizer.Object,
                _mockFhirSanitizationHelper.Object,
                _mockDemographicsOlcMapper.Object,
                _mockAuditor.Object,
                _mockGpSystemFactory.Object,
                _mockCreateFhirParam.Object
                );
        }
        
        [TestMethod]
        public async Task GetServiceDefinitionById_WhenProviderClientThrowsException_ReturnsBadRequest()
        {
            // Arrange
            _mockProviderHttpClient
                .Setup(pc => pc.EvaluateServiceDefinition(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal)),
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .Throws<HttpRequestException>();

            // Act
            var response = await _service.GetServiceDefinitionById(_mockProviderHttpClient.Object, ServiceDefinitionId, "eConsult", _userSession);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.BadRequest>();
        }

        [TestMethod]
        public async Task GetServiceDefinitionById_WhenProviderClientReturnsUnsuccessfulStatusCode_ReturnsBadGateway()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest };

            _mockProviderHttpClient
                .Setup(pc => pc.EvaluateServiceDefinition(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal)),
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);

            // Act
            var response = await _service.GetServiceDefinitionById(_mockProviderHttpClient.Object, ServiceDefinitionId, "eConsult", _userSession);

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
                .Setup(pc => pc.EvaluateServiceDefinition(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal)),
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.GetServiceDefinitionById(_mockProviderHttpClient.Object, ServiceDefinitionId, "eConsult", _userSession);

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
                .Setup(pc => pc.EvaluateServiceDefinition(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal)),
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.GetServiceDefinitionById(_mockProviderHttpClient.Object, ServiceDefinitionId, "eConsult", _userSession);

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
                Content = new StringContent(GuidanceResponseJsonContent, Encoding.UTF8, Constants.ContentTypes.ApplicationJsonFhir)
            };

            _mockProviderHttpClient
                .Setup(pc => pc.EvaluateServiceDefinition(
                    It.Is<string>(sid => ServiceDefinitionId.Equals(sid, StringComparison.Ordinal)),
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.GetServiceDefinitionById(_mockProviderHttpClient.Object, ServiceDefinitionId, "eConsult", _userSession);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.Success>();
            _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeGuidanceResponse(
                It.IsAny<GuidanceResponse>(), It.IsAny<IHtmlSanitizer>()), Times.Once);
        }

        [TestMethod]
        public async Task GetServiceDefinitions_WhenProviderClientThrowsException_ReturnsBadRequest()
        {
            // Arrange
            _mockProviderHttpClient
                .Setup(pc => pc.SearchServiceDefinitionsByQuery())
                .Throws<HttpRequestException>();

            // Act
            var response = await _service.GetServiceDefinitions(_mockProviderHttpClient.Object);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionListResult.BadRequest>();
        }
        
        [TestMethod]
        public async Task GetServiceDefinitions_WhenProviderClientReturnsUnsuccessfulStatusCode_ReturnsBadGateway()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest };

            _mockProviderHttpClient
                .Setup(pc => pc.SearchServiceDefinitionsByQuery())
                .ReturnsAsync(httpResponse);

            // Act
            var response = await _service.GetServiceDefinitions(_mockProviderHttpClient.Object);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionListResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetServiceDefinitions_WhenProviderClientReturnsNullContent_ReturnsBadGateway()
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
            var response = await _service.GetServiceDefinitions(_mockProviderHttpClient.Object);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionListResult.BadGateway>();
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow(ServiceDefinitionJsonContent)]
        public async Task GetServiceDefinitions_WhenProviderClientReturnsNonBundleContent_ReturnsBadGateway(string content)
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
            var response = await _service.GetServiceDefinitions(_mockProviderHttpClient.Object);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionListResult.BadGateway>();
            _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeServiceDefinitionSearchBundle(
                It.IsAny<Bundle>(), It.IsAny<IHtmlSanitizer>()), Times.Never);
        }
        
        [TestMethod]
        public async Task GetServiceDefinitions_WhenResponseParsedSuccessfully_BuildsListAndReturnsSuccess()
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
            var response = await _service.GetServiceDefinitions(_mockProviderHttpClient.Object);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionListResult.Success>();

            _mockServiceDefinitionListBuilder.Verify(a => a.BuildServiceDefinitionList(It.IsAny<Bundle>()), Times.Once);
        }

        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenNullParametersProvided_ReturnsBadRequest()
        {
            // Act
            var response = await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, null, false, false, _userSession);

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
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .Throws<HttpRequestException>();

            // Act
            var response = await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, new Parameters(), false, false, _userSession);

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
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, new Parameters(), false, false, _userSession);

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
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, new Parameters(), false, false, _userSession);

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
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, new Parameters(), false, false, _userSession);

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
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);
            
            // Act
            var response = await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, new Parameters(), false, false, _userSession);
            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.Success>();
            _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeGuidanceResponse(
                It.IsAny<GuidanceResponse>(), It.IsAny<IHtmlSanitizer>()), Times.Once);
        }
        
        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenResponseParsedSuccessfullyAddPatient_SanitizesResponseAndReturnsSuccess()
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
                    It.Is<string>(body => body.Contains("\"resourceType\":\"Patient\"", StringComparison.Ordinal)),
                    It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);

            var olcDem = new OlcDemographics
            {
                NameFull = "Test Test",
                NhsNumber = "111 111 111",
                AddressFull = "Test",
                DateOfBirth = DateTime.UtcNow
            };

            _mockDemographicsOlcMapper.Setup(d => d.Map(It.IsAny<DemographicsResponse>())).Returns(olcDem);

            _mockCreateFhirParam.Setup(cfp => cfp.CreatePatientFhir(
                It.IsAny<IMapper<DemographicsResponse, OlcDemographics>>(),
                It.IsAny<DemographicsResult.Success>())).Returns(new Patient
            {
                Address = new List<Address>
                {
                    new Address
                    {
                        Text = olcDem.AddressFull
                    }
                },
                BirthDate = olcDem.DateOfBirth.ToString(),
                Name = new List<HumanName>
                {
                    new HumanName
                    {
                        Text = olcDem.NameFull
                    }
                },
                Identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        Value = olcDem.NhsNumber
                    }
                }
            });

            var fhirParser = new FhirJsonParser();
            var paramsParsed = fhirParser.Parse(paramString);
           
            var bodyParameters = (Parameters) paramsParsed;
            
            // Act
            var response = await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, bodyParameters, false, true ,_userSession);

            // Assert
            response.Should().BeAssignableTo<ServiceDefinitionResult.Success>();
            _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeGuidanceResponse(
                It.IsAny<GuidanceResponse>(), It.IsAny<IHtmlSanitizer>()), Times.Once);
        }
        
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task EvaluateServiceDefinition_WillPassAddJsDisabledParameterToProviderClient(bool addJsDisabledHeader)
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
                    It.IsAny<string>(),
                    It.Is<bool>(addHeader => addHeader == addJsDisabledHeader)))
                .ReturnsAsync(httpResponse);
            
            // Act
            await _service.EvaluateServiceDefinition(_mockProviderHttpClient.Object, ServiceDefinitionId, new Parameters(), addJsDisabledHeader, false, _userSession);

            // Assert
            _mockFhirSanitizationHelper.Verify(fsh => fsh.SanitizeGuidanceResponse(
                It.IsAny<GuidanceResponse>(), It.IsAny<IHtmlSanitizer>()), Times.Once);
        }
    }
}