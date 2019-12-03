using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.PfsApi.Areas.Messages;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Messages
{
    [TestClass]
    public class PatientMessagesControllerTests
    {
        private IFixture _fixture;

        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IPatientMessagesService> _mockPatientMessagesService;

        private Mock<IAuditor> _mockAuditor;
        private Mock<IErrorReferenceGenerator> _mockErrorReferenceGenerator;
        private string _serviceDeskReference;

        private Mock<HttpContext> _mockHttpContext;
        private PatientMessagesController _systemUnderTest;

        private UserSession _userSession;

        private const string RequestAuditType = "PracticePatientMessages_View_Request";
        private const string ResponseAuditType = "PracticePatientMessages_View_Response";
        private const string RequestAuditMessage = "Viewing Practice to Patient Messages";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockPatientMessagesService = _fixture.Freeze<Mock<IPatientMessagesService>>();
            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();

            _mockGpSystemFactory
                .Setup(f => f.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);
            _mockGpSystem
                .Setup(g => g.GetPatientMessagesService())
                .Returns(_mockPatientMessagesService.Object);

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();
            _mockErrorReferenceGenerator = _fixture.Freeze<Mock<IErrorReferenceGenerator>>();
            _serviceDeskReference = _fixture.Create<string>();

            _fixture
                .Customize<UserSession>(c => c
                    .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));
            _userSession = _fixture.Create<UserSession>();

            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpContext
                .Setup(x => x.Items[Constants.HttpContextItems.UserSession])
                .Returns(_userSession);

            _systemUnderTest = _fixture.Create<PatientMessagesController>();
            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            };
        }

        [TestMethod]
        public async Task GetMessages_ReturnsSuccessResult_WhenServiceReturnsSuccessfulResponse()
        {
            // Arrange
            var successResponse = _fixture.Create<GetPatientMessagesResponse>();
            var successResult = new GetPatientMessagesResult.Success(successResponse);

            _mockPatientMessagesService
                .Setup(s => s.GetMessages(
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession)))
                .Returns(Task.FromResult((GetPatientMessagesResult) successResult));

            // Act
            var result = await _systemUnderTest.GetMessages();

            // Assert
            result
                .Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().Be(successResponse);
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "Patient messages successfully retrieved"));
        }
        
        [TestMethod]
        public async Task GetMessageDetaisl_ReturnsSuccessResult_WhenServiceReturnsSuccessfulResponse()
        {
            // Arrange
            var successResponse = _fixture.Create<GetPatientMessageResponse>();
            var successResult = new GetPatientMessageResult.Success(successResponse);

            _mockPatientMessagesService
                .Setup(s => s.GetMessageDetails("1",
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession)))
                .Returns(Task.FromResult((GetPatientMessageResult) successResult));

            // Act
            var result = await _systemUnderTest.GetMessageDetails("1");

            // Assert
            result
                .Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().Be(successResponse);
        }
        
        [DataTestMethod]
        [DataRow(typeof(GetPatientMessageResult.Forbidden), StatusCodes.Status403Forbidden)]
        [DataRow(typeof(GetPatientMessageResult.InternalServerError), StatusCodes.Status500InternalServerError)]
        [DataRow(typeof(GetPatientMessageResult.BadGateway), StatusCodes.Status502BadGateway)]
        [DataRow(typeof(GetPatientMessageResult.BadRequest), StatusCodes.Status400BadRequest)]
        public async Task Get_ServiceReturnsErrorResultForGetMessageDetails_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode)
        {
            // Arrange
            var serviceResult = (GetPatientMessageResult) Activator.CreateInstance(serviceResultType);
            _mockPatientMessagesService
                .Setup(s => s.GetMessageDetails("1",
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession)))
                .Returns(Task.FromResult(serviceResult))
                .Verifiable();
            _mockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    ErrorCategory.PatientPracticeMessages,
                    expectedStatusCode,
                    _userSession.GpUserSession.Supplier))
                .Returns(_serviceDeskReference)
                .Verifiable();

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.GetMessageDetails("1");

            // Assert
            _mockPatientMessagesService.Verify();
            _mockErrorReferenceGenerator.Verify();

            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(expectedStatusCode);
            objectResult.Value.Should().BeEquivalentTo(expectedValue);
        }

        [DataTestMethod]
        [DataRow(typeof(GetPatientMessagesResult.Forbidden), StatusCodes.Status403Forbidden,
            "Error retrieving patient messages: Forbidden")]
        [DataRow(typeof(GetPatientMessagesResult.InternalServerError), StatusCodes.Status500InternalServerError,
            "Error retrieving patient messages: Internal Server Error")]
        [DataRow(typeof(GetPatientMessagesResult.BadGateway), StatusCodes.Status502BadGateway,
            "Error retrieving patient messages: Bad Gateway")]
        [DataRow(typeof(GetPatientMessagesResult.BadRequest), StatusCodes.Status400BadRequest,
            "Error retrieving patient messages: Bad Request")]
        public async Task Get_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode,
            string expectedAuditResponseMessage)
        {
            // Arrange
            var serviceResult = (GetPatientMessagesResult) Activator.CreateInstance(serviceResultType);
            _mockPatientMessagesService
                .Setup(s => s.GetMessages(
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession)))
                .Returns(Task.FromResult(serviceResult))
                .Verifiable();
            _mockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    ErrorCategory.PatientPracticeMessages,
                    expectedStatusCode,
                    _userSession.GpUserSession.Supplier))
                .Returns(_serviceDeskReference)
                .Verifiable();
            _mockAuditor
                .Setup(x => x.Audit(RequestAuditType, RequestAuditMessage))
                .Returns(Task.CompletedTask)
                .Verifiable();
            _mockAuditor
                .Setup(x => x.Audit(ResponseAuditType, expectedAuditResponseMessage))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.GetMessages();

            // Assert
            _mockPatientMessagesService.Verify();
            _mockErrorReferenceGenerator.Verify();
            _mockAuditor.Verify();

            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(expectedStatusCode);
            objectResult.Value.Should().BeEquivalentTo(expectedValue);
        }
    }
}
