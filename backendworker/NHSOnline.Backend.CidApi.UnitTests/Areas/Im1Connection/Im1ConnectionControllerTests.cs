using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.CidApi.Areas.Im1Connection;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.Support;
using PatientIm1ConnectionResponse = NHSOnline.Backend.CidApi.Areas.Im1Connection.Models.PatientIm1ConnectionResponse;

namespace NHSOnline.Backend.CidApi.UnitTests.Areas.Im1Connection
{
    [TestClass]
    public class Im1ConnectionControllerTests
    {
        private const string DefaultOdsCode = "A12345";
        private const string DefaultPatientIdentifier = "XX00000A";
        private const string DefaultConnectionToken = "b2ed6831-cdd4-4ef7-a9b4-0880c2a35d78";

        private Im1ConnectionController _systemUnderTest;
        private Mock<ILogger<Im1ConnectionController>> _logger;
        private Mock<IAuditor> _auditor;
        private Mock<ITokenValidationService> _tokenValidationService;
        private Mock<IRetrieveLinkageKeysService> _retrieveLinkageKeysService;
        private Mock<IOdsCodeMassager> _odsCodeMassager;
        private IFixture _fixture;
        private Mock<IGpSystemFactory> _gpSystemFactory;
        private Mock<Im1ConnectionErrorCodes> _im1ErrorCodes;
        private Mock<Im1ConnectionErrorCodes> _errorCodes;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();

            _logger = new Mock<ILogger<Im1ConnectionController>>();
            _auditor = new Mock<IAuditor>();
            _gpSystemFactory = new Mock<IGpSystemFactory>();

            _tokenValidationService = new Mock<ITokenValidationService>();
            _tokenValidationService.Setup(x => x.IsValidConnectionTokenFormat(It.IsAny<string>())).Returns(true);
            
            _retrieveLinkageKeysService = new Mock<IRetrieveLinkageKeysService>();

            _odsCodeMassager = new Mock<IOdsCodeMassager>();
            

            _im1ErrorCodes = new Mock<Im1ConnectionErrorCodes>();

            _errorCodes = new Mock<Im1ConnectionErrorCodes>();

            _systemUnderTest = CreateIm1ConnectionController();
        }

        [TestMethod]
        public void Constructor_NullOdsCodeLookup_Throws()
        {
            Action act = () => new Im1ConnectionController(null, 
                _logger.Object,
                _auditor.Object,
                _odsCodeMassager.Object, 
                _retrieveLinkageKeysService.Object,
                _im1ErrorCodes.Object, 
                _errorCodes.Object);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("gpSystemFactory");
        }

        [TestMethod]
        public async Task GetIm1ConnectionV1_ReturnsABadRequestResult_WhenTheConnectionTokenIsNull()
        {
            var result = await _systemUnderTest.Get(null, DefaultOdsCode);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
            _auditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetIm1ConnectionV1_ReturnsABadRequestResult_WhenTheConnectionTokenIsEmpty()
        {
            var result = await _systemUnderTest.Get(string.Empty, DefaultOdsCode);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
            _auditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetIm1ConnectionV1_ReturnsABadRequestResult_WhenTheOdsCodeIsNull()
        {
            var result = await _systemUnderTest.Get(DefaultConnectionToken, null);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
            _auditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetIm1ConnectionV1_ReturnsABadRequestResult_WhenTheOdsCodeIsEmpty()
        {
            var result = await _systemUnderTest.Get(DefaultConnectionToken, string.Empty);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
            _auditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetIm1ConnectionV1_ReturnsTheSuccessResponse_WhenServiceIsSuccessfullyCalled()
        {
            // Arrange
            const string odsCode = DefaultOdsCode;
            const string patientIdentifier = DefaultConnectionToken;

            _odsCodeMassager.Setup(x => x.CheckOdsCode(odsCode)).Returns(odsCode);

            var expectedNhsNumbers = new[]
            {
                new PatientNhsNumber { NhsNumber = "123ABC" },
                new PatientNhsNumber { NhsNumber = "456DEF" }
            };
            
            var verifyResponse = new GpSystems.Im1Connection.Models.PatientIm1ConnectionResponse
            {
                ConnectionToken = DefaultConnectionToken,
                NhsNumbers = expectedNhsNumbers
            };

            var expectedResponse = new PatientIm1ConnectionResponse
            {
                ConnectionToken = DefaultConnectionToken,
                NhsNumbers = expectedNhsNumbers
            };

            var im1ConnectionService = MockIm1ConnectionService(patientIdentifier, odsCode,
                new Im1ConnectionVerifyResult.Success(verifyResponse));
            
            var gpSystemMock = MockGpSystem(im1ConnectionService);
            _gpSystemFactory
                .Setup(x => x.LookupGpSystem(odsCode))
                .ReturnsAsync(Option.Some(gpSystemMock.Object));
            
            _systemUnderTest = CreateIm1ConnectionController();

            // Act
            var result = await _systemUnderTest.Get(DefaultConnectionToken, odsCode);

            // Assert
            var resultValue = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = resultValue.Should().BeAssignableTo<PatientIm1ConnectionResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);

            _auditor.Verify(x => x.AuditRegistrationEvent(expectedNhsNumbers[0].NhsNumber, gpSystemMock.Object.Supplier,
                AuditingOperations.Im1ConnectionVerifyResponse, It.IsAny<string>(), It.IsAny<object[]>()));
        }

        [TestMethod]
        public async Task GetIm1ConnectionV1_UnknownOdsCode_Returns501NotImplemented()
        {
            // Arrange
            _odsCodeMassager
                .Setup(x => x.CheckOdsCode(DefaultOdsCode))
                .Returns(DefaultOdsCode);

            _systemUnderTest = CreateIm1ConnectionController();

            // Act
            var result = await _systemUnderTest.Get(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status501NotImplemented);
            _auditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Post_UnknownOdsCode_Returns501NotImplemented()
        {
            // Arrange
            var request = _fixture.Create<PatientIm1ConnectionRequest>();
            request.OdsCode = DefaultOdsCode;
          
            _odsCodeMassager
                .Setup(x => x.CheckOdsCode(DefaultOdsCode))
                .Returns(DefaultOdsCode);

            _systemUnderTest = CreateIm1ConnectionController();

            // Act
            var result = await _systemUnderTest.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>();
            objectResult.Subject.StatusCode.Should().Be(StatusCodes.Status501NotImplemented);
            _auditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Post_ReturnsTheSuccessResponse_WhenServiceIsSuccessfullyCalled()
        {
            // Arrange
            const string odsCode = DefaultOdsCode;
            const string patientIdentifier = DefaultConnectionToken;

            var model = _fixture.Create<PatientIm1ConnectionRequest>();
            model.OdsCode = odsCode;

            var expectedNhsNumbers = new[]
            {
                new PatientNhsNumber { NhsNumber = "123ABC" },
                new PatientNhsNumber { NhsNumber = "456DEF" }
            };

            var registerResponse = new GpSystems.Im1Connection.Models.PatientIm1ConnectionResponse
            {
                ConnectionToken = DefaultConnectionToken,
                NhsNumbers = expectedNhsNumbers,
                OdsCode = DefaultOdsCode,
                AccountId = model.AccountId,
                LinkageKey = model.LinkageKey
            };

            var expectedResponse = new PatientIm1ConnectionResponse
            {
                ConnectionToken = DefaultConnectionToken,
                NhsNumbers = expectedNhsNumbers
            };

            _odsCodeMassager.Setup(x => x.CheckOdsCode(odsCode)).Returns(odsCode);

            var im1ConnectionService = MockIm1ConnectionService(patientIdentifier, odsCode,
                new Im1ConnectionVerifyResult.Success(registerResponse));
            
            var gpSystemMock = MockGpSystem(im1ConnectionService);
            
            _gpSystemFactory
                .Setup(x => x.LookupGpSystem(odsCode))
                .ReturnsAsync(Option.Some(gpSystemMock.Object));
            
            im1ConnectionService
                .Setup(x => x.Register(model))
                .ReturnsAsync(new Im1ConnectionRegisterResult.Success(registerResponse));
            
            _systemUnderTest = CreateIm1ConnectionController();

            // Act
            var result = await _systemUnderTest.Post(model);

            // Assert
            var resultValue = result.Should().BeAssignableTo<CreatedResult>().Subject.Value;
            var actualResponse = resultValue.Should().BeAssignableTo<PatientIm1ConnectionResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);

            _auditor.Verify(x => x.AuditRegistrationEvent(expectedNhsNumbers[0].NhsNumber, gpSystemMock.Object.Supplier,
                AuditingOperations.Im1ConnectionRegisterResponse, It.IsAny<string>(), It.IsAny<object[]>()));
        }

        [TestMethod]
        public async Task PostV2_CreateLinkageRequestInvalid_ReturnsBadRequest()
        {
            var model = ValidIm1RegistrationRequest();
            model.OdsCode = null;

            // Act
            var result = await _systemUnderTest.Post(model);

            //Assert
            AssertErrorWithStatusCode(
                result, 
                StatusCodes.Status400BadRequest,
                Im1ConnectionErrorCodes.ExternalCode.InvalidDetails,
                "Invalid Details. Invalid parameters: OdsCode");
        }

        [TestMethod]
        public async Task PostV2_NoGpSystem_Returns501()
        {
            var model = ValidIm1RegistrationRequest();
            
            _odsCodeMassager
                .Setup(x => x.CheckOdsCode(model.OdsCode))
                .Returns(model.OdsCode);
            
            _gpSystemFactory.Setup(x => x.LookupGpSystem(DefaultOdsCode)).ReturnsAsync(Option.None<IGpSystem>());

            // Act
            var result = await _systemUnderTest.Post(model);

            //Assert
            AssertErrorWithStatusCode(
                result,
                StatusCodes.Status501NotImplemented,
                Im1ConnectionErrorCodes.ExternalCode.InvalidDetails,
                "Invalid Details. Invalid parameters: OdsCode");
        }

        [TestMethod]
        public async Task PostV2_NoLinkageNeeded_ReturnsSuccess()
        {
            var model = ValidIm1RegistrationRequest();
            
            _odsCodeMassager
                .Setup(x => x.CheckOdsCode(model.OdsCode))
                .Returns(model.OdsCode);
            
            var expectedResponse = new GpSystems.Im1Connection.Models.PatientIm1ConnectionResponse
            {
                ConnectionToken = DefaultConnectionToken,
                NhsNumbers = new []{new PatientNhsNumber { NhsNumber = "1112223333" }}
            };
            var mockIm1ConnectionService = new Mock<IIm1ConnectionService>();
            mockIm1ConnectionService
                .Setup(x => x.Register(It.IsAny<PatientIm1ConnectionRequest>()))
                .ReturnsAsync(new Im1ConnectionRegisterResult.Success(expectedResponse));
            
            var gpSystemMock = MockGpSystem(mockIm1ConnectionService);
            _gpSystemFactory
                .Setup(x => x.LookupGpSystem(DefaultOdsCode))
                .ReturnsAsync(Option.Some(gpSystemMock.Object));

            // Act
            var result = await _systemUnderTest.Post(model);

            //Assert
            var resultValue = result.Should().BeAssignableTo<CreatedResult>().Subject.Value;
            var actualResponse = resultValue.Should().BeAssignableTo<GpSystems.Im1Connection.Models.PatientIm1ConnectionResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task PostV2_NoLinkageNeeded_WhereRegisterReturnsError_ReturnsError()
        {
            var model = ValidIm1RegistrationRequest();
            
            _odsCodeMassager
                .Setup(x => x.CheckOdsCode(model.OdsCode))
                .Returns(model.OdsCode);
            
            var mockIm1ConnectionService = new Mock<IIm1ConnectionService>();
            mockIm1ConnectionService
                .Setup(x => x.Register(It.IsAny<PatientIm1ConnectionRequest>()))
                .ReturnsAsync(new Im1ConnectionRegisterResult.ErrorCase(Im1ConnectionErrorCodes.InternalCode.UnknownError));
            
            var gpSystemMock = MockGpSystem(mockIm1ConnectionService);
            _gpSystemFactory
                .Setup(x => x.LookupGpSystem(DefaultOdsCode))
                .ReturnsAsync(Option.Some(gpSystemMock.Object));

            // Act
            var result = await _systemUnderTest.Post(model);

            //Assert
            AssertErrorWithStatusCode(
                result,
                StatusCodes.Status500InternalServerError,
                Im1ConnectionErrorCodes.ExternalCode.UnknownError,
                "Unknown Error");
        }

        [TestMethod]
        public async Task PostV2_LinkageNeeded_WhereLinkageReturnsError_ReturnsError()
        {
            var model = ValidIm1RegistrationRequest();
            model.LinkageKey = null;
            
            _odsCodeMassager
                .Setup(x => x.CheckOdsCode(model.OdsCode))
                .Returns(model.OdsCode);

            var mockIm1ConnectionService = new Mock<IIm1ConnectionService>();
            var gpSystemMock = MockGpSystem(mockIm1ConnectionService);
            _gpSystemFactory
                .Setup(x => x.LookupGpSystem(DefaultOdsCode))
                .ReturnsAsync(Option.Some(gpSystemMock.Object));

            _retrieveLinkageKeysService.Setup(x =>
                x.RetrieveLinkageKey(
                    It.IsAny<RetrieveLinkageKeysRequest>(),
                    It.IsAny<IGpSystem>())).ReturnsAsync(
                new LinkageResult.UnmappedErrorWithStatusCode(HttpStatusCode.BadRequest));

            mockIm1ConnectionService
                .Setup(x => x.Register(It.IsAny<PatientIm1ConnectionRequest>()))
                .ReturnsAsync(new Im1ConnectionRegisterResult.ErrorCase(Im1ConnectionErrorCodes.InternalCode.UnknownError));
            // Act
            var result = await _systemUnderTest.Post(model);

            //Assert
            AssertErrorWithStatusCode(
                result,
                StatusCodes.Status502BadGateway,
                Im1ConnectionErrorCodes.ExternalCode.UnknownError,
                "Unknown Error");
        }

        [TestMethod]
        public async Task PostV2_LinkageNeeded_WhereLinkageReturnsSuccessAndIm1ReturnsError_ReturnsError()
        {
            var model = ValidIm1RegistrationRequest();
            model.LinkageKey = null;
            
            _odsCodeMassager
                .Setup(x => x.CheckOdsCode(model.OdsCode))
                .Returns(model.OdsCode);

            var mockIm1ConnectionService = new Mock<IIm1ConnectionService>();
            var gpSystemMock = MockGpSystem(mockIm1ConnectionService);
            _gpSystemFactory
                .Setup(x => x.LookupGpSystem(DefaultOdsCode))
                .ReturnsAsync(Option.Some(gpSystemMock.Object));

            var linkageResponse = _fixture.Create<LinkageResponse>();
            _retrieveLinkageKeysService.Setup(x =>
                x.RetrieveLinkageKey(
                    It.IsAny<RetrieveLinkageKeysRequest>(),
                    It.IsAny<IGpSystem>())).ReturnsAsync(
                new LinkageResult.SuccessfullyCreated(linkageResponse));

            mockIm1ConnectionService
                .Setup(x => x.Register(It.IsAny<PatientIm1ConnectionRequest>()))
                .ReturnsAsync(new Im1ConnectionRegisterResult.ErrorCase(Im1ConnectionErrorCodes.InternalCode.UnknownError));

            // Act
            var result = await _systemUnderTest.Post(model);

            //Assert
            AssertErrorWithStatusCode(
                result,
                StatusCodes.Status500InternalServerError,
                Im1ConnectionErrorCodes.ExternalCode.UnknownError,
                "Unknown Error");
        }

        [TestMethod]
        public async Task PostV2_LinkageNeeded_WhereLinkageReturnsSuccessAndIm1ReturnsSuccess_ReturnsSuccess()
        {
            var model = ValidIm1RegistrationRequest();
            model.LinkageKey = null;
            
            _odsCodeMassager
                .Setup(x => x.CheckOdsCode(model.OdsCode))
                .Returns(model.OdsCode);

            var mockIm1ConnectionService = new Mock<IIm1ConnectionService>();
            var gpSystemMock = MockGpSystem(mockIm1ConnectionService);
            _gpSystemFactory
                .Setup(x => x.LookupGpSystem(DefaultOdsCode))
                .ReturnsAsync(Option.Some(gpSystemMock.Object));

            var linkageResponse = _fixture.Create<LinkageResponse>();
            _retrieveLinkageKeysService.Setup(x =>
                x.RetrieveLinkageKey(
                    It.IsAny<RetrieveLinkageKeysRequest>(),
                    It.IsAny<IGpSystem>())).ReturnsAsync(
                new LinkageResult.SuccessfullyCreated(linkageResponse));

            var connectionResponse = _fixture.Create<GpSystems.Im1Connection.Models.PatientIm1ConnectionResponse>();
            mockIm1ConnectionService
                .Setup(x => x.Register(It.IsAny<PatientIm1ConnectionRequest>()))
                .ReturnsAsync(new Im1ConnectionRegisterResult.Success(connectionResponse));

            // Act
            var result = await _systemUnderTest.Post(model);

            //Assert
            var resultValue = result.Should().BeAssignableTo<CreatedResult>().Subject.Value;
            var actualResponse = resultValue.Should().BeAssignableTo<GpSystems.Im1Connection.Models.PatientIm1ConnectionResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(connectionResponse);
        }

        private Im1RegistrationRequest ValidIm1RegistrationRequest()
        {
            return new Im1RegistrationRequest()
            {
                AccountId = _fixture.Create<string>(),
                DateOfBirth = _fixture.Create<DateTime>(),
                EmailAddress = _fixture.Create<string>(),
                IdentityToken = _fixture.Create<string>(),
                LinkageKey = _fixture.Create<string>(),
                NhsNumber = "1112223333",
                OdsCode = DefaultOdsCode,
                Surname = _fixture.Create<string>()
            };
        }

        private Im1ConnectionController CreateIm1ConnectionController()
        {
            // Dummy context...
            var dummyHttpContext = new DefaultHttpContext();
            dummyHttpContext.Request.Host = new HostString("some.host.name");
            dummyHttpContext.Request.PathBase = "/test/";
            dummyHttpContext.Request.Path = "/test.html";
            dummyHttpContext.Request.QueryString = new QueryString("?test=test");

            var dummyControllerContext = new ControllerContext(new ActionContext(dummyHttpContext, new RouteData(),
                new ControllerActionDescriptor()));

            return new Im1ConnectionController(_gpSystemFactory.Object,
                _logger.Object,
                _auditor.Object, 
                _odsCodeMassager.Object,
                _retrieveLinkageKeysService.Object,
                _im1ErrorCodes.Object,
                _errorCodes.Object)
            {
                ControllerContext = dummyControllerContext
            };
        }

        private Mock<IGpSystem> MockGpSystem(
            Mock<IIm1ConnectionService> nhsNumberProvider = null,
            ITokenValidationService tokenValidationService = null
        )
        {
            nhsNumberProvider = nhsNumberProvider ?? MockIm1ConnectionService();
            tokenValidationService = tokenValidationService ?? _tokenValidationService.Object;

            var mockGpSystem = new Mock<IGpSystem>();
            mockGpSystem.Setup(x => x.GetIm1ConnectionService()).Returns(nhsNumberProvider.Object);
            mockGpSystem.Setup(x => x.GetTokenValidationService()).Returns(tokenValidationService);

            return mockGpSystem;
        }

        private static Mock<IIm1ConnectionService> MockIm1ConnectionService(
            string patientIdentifier = DefaultPatientIdentifier, string odsCode = DefaultOdsCode,
            Im1ConnectionVerifyResult expectedResponse = null)
        {
            var mockIm1ConnectionService = new Mock<IIm1ConnectionService>();
            mockIm1ConnectionService.Setup(x => x.Verify(patientIdentifier, odsCode))
                .ReturnsAsync(expectedResponse);

            return mockIm1ConnectionService;
        }

        private void AssertErrorWithStatusCode(IActionResult result, int statusCode, Im1ConnectionErrorCodes.ExternalCode errorCode, string message)
        {
            var objectResult = result.Should().BeAssignableTo<ObjectResult>();
            objectResult.Subject.StatusCode.Should().Be(statusCode);
            var resultValue = objectResult.Subject.Value;

            var errorResult = resultValue.Should().BeAssignableTo<Im1ErrorResponse>().Subject;
            errorResult.ErrorCode.Should().Be((int)errorCode);
            errorResult.ErrorMessage.Should().Be(message);
        }
    }
}