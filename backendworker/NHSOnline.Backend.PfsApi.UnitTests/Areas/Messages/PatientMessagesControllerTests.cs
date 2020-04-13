using System;
using System.Globalization;
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

        private const string GetMessagesRequestAuditType = "PatientPracticeMessages_View_Request";
        private const string GetMessagesResponseAuditType = "PatientPracticeMessages_View_Response";
        private const string GetMessagesRequestAuditMessage = "Viewing Patient to Practice Messages";

        private const string GetMessageRequestAuditType = "PatientPracticeMessageDetails_Get_Request";
        private const string GetMessageResponseAuditType = "PatientPracticeMessageDetails_Get_Response";
        private const string GetMessageRequestAuditMessage = "Getting Patient to Practice Message Details";

        private const string GetMessageRecipientsRequestAuditType = "PatientPracticeMessageRecipients_Get_Request";
        private const string GetMessageRecipientsResponseAuditType = "PatientPracticeMessageRecipients_Get_Response";
        private const string GetMessageRecipientsRequestAuditMessage = "Getting Patient to Practice Message Recipients";

        private const string UpdateMessageUnreadStatusRequestAuditType = "PatientPracticeMessageUnreadStatus_Update_Request";
        private const string UpdateMessageUnreadStatusResponseAuditType = "PatientPracticeMessageUnreadStatus_Update_Response";

        private const string CreateMessageRequestAuditType = "PatientPracticeMessage_Create_Request";
        private const string CreateMessageResponseAuditType = "PatientPracticeMessage_Create_Response";
        private const string CreateMessageRequestAuditMessage = "Creating a patient to practice message";

        private const string DeletePatientPracticeMessageRequest = "PatientPracticeMessage_Delete_Request";
        private const string DeletePatientPracticeMessageResponse = "PatientPracticeMessage_Delete_Response";

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
            MockAuditor(GetMessagesRequestAuditType, GetMessagesRequestAuditMessage);
            MockAuditor(GetMessagesResponseAuditType, "Patient messages successfully retrieved");

            // Act
            var result = await _systemUnderTest.GetMessages();

            // Assert
            _mockPatientMessagesService.Verify();
            VerifyMockAuditor();

            result
                .Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().Be(successResponse);
        }

        [TestMethod]
        public async Task GetMessageDetails_ReturnsSuccessResult_WhenServiceReturnsSuccessfulResponse()
        {
            // Arrange
            var successResponse = _fixture.Create<GetPatientMessageResponse>();
            var successResult = new GetPatientMessageResult.Success(successResponse);

            _mockPatientMessagesService
                .Setup(s => s.GetMessageDetails("1",
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession)))
                .Returns(Task.FromResult((GetPatientMessageResult) successResult));
            MockAuditor(GetMessageRequestAuditType, GetMessageRequestAuditMessage);
            MockAuditor(GetMessageResponseAuditType, "Patient message details successfully retrieved");

            // Act
            var result = await _systemUnderTest.GetMessageDetails("1");

            // Assert
            _mockPatientMessagesService.Verify();
            VerifyMockAuditor();

            result
                .Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().Be(successResponse);
        }


        [TestMethod]
        public async Task GetMessageRecipients_ReturnsSuccessResult_WhenServiceReturnsSuccessfulResponse()
        {
            // Arrange
            var successResponse = _fixture.Create<PatientPracticeMessageRecipients>();
            var successResult = new GetPatientMessageRecipientsResult.Success(successResponse);

            _mockPatientMessagesService
                .Setup(s => s.GetMessageRecipients(
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession)))
                .Returns(Task.FromResult((GetPatientMessageRecipientsResult) successResult));
            MockAuditor(GetMessageRecipientsRequestAuditType, GetMessageRecipientsRequestAuditMessage);
            MockAuditor(GetMessageRecipientsResponseAuditType, "Patient message recipients successfully retrieved");

            // Act
            var result = await _systemUnderTest.GetMessageRecipients();

            // Assert
            _mockPatientMessagesService.Verify();
            VerifyMockAuditor();

            result
                .Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().Be(successResponse);
        }

        [TestMethod]
        public async Task PostUpdateMessageReadStatus_ReturnsSuccessResult_WhenServiceReturnsSuccessfulResponse()
        {
            // Arrange
            var successResponse = _fixture.Create<PutPatientMessageUpdateStatusResponse>();
            var successResult = new PutPatientMessageReadStatusResult.Success(successResponse);

            var requestBody = new UpdateMessageReadStatusRequestBody
            {
                MessageId = 1,
                MessageReadState = "Read"
            };

            _mockPatientMessagesService
                .Setup(s => s.UpdateMessageMessageReadStatus(
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession),
                    It.Is<UpdateMessageReadStatusRequestBody>(p => p.MessageId == requestBody.MessageId &&
                                                                   p.MessageReadState.Equals(requestBody.MessageReadState, StringComparison.Ordinal))))
                .Returns(Task.FromResult((PutPatientMessageReadStatusResult) successResult));
            MockAuditor(UpdateMessageUnreadStatusRequestAuditType, "Updating unread status for message with id 1 to Read");
            MockAuditor(UpdateMessageUnreadStatusResponseAuditType, "Patient message read status successfully updated");

            // Act
            var result = await _systemUnderTest.PostUpdateMessageReadStatus(requestBody);

            // Assert
            _mockPatientMessagesService.Verify();
            VerifyMockAuditor();

            result
                .Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().Be(successResponse);
        }

        [TestMethod]
        public async Task DeleteMessage_ReturnsSuccessResult_WhenServiceReturnsSuccessfulResponse()
        {
            // Arrange
            var successResult = new DeletePatientMessageResult.Success();

            _mockPatientMessagesService
                .Setup(s => s.DeleteMessage(
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession),"1"))
                .Returns(Task.FromResult((DeletePatientMessageResult) successResult));
            MockAuditor(DeletePatientPracticeMessageRequest, "Deleting a patient to practice message with id 1");
            MockAuditor(DeletePatientPracticeMessageResponse, "Patient message successfully deleted");

            // Act
            var result = await _systemUnderTest.DeleteMessage("1");

            // Assert
            _mockPatientMessagesService.Verify();
            VerifyMockAuditor();

            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task SendMessage_ReturnsSuccessResult_WhenServiceReturnsSuccessfulResponse()
        {
            // Arrange
            var message = new CreatePatientMessage
            {
                Subject = "subject",
                MessageBody = "message",
                Recipient = "recipient 1"

            };
            var successResponse = _fixture.Create<PostPatientMessageResponse>();
            var successResult = new PostPatientMessageResult.Success(successResponse);

            _mockPatientMessagesService
                .Setup(s => s.SendMessage(
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession),
                    message))
                .Returns(Task.FromResult((PostPatientMessageResult) successResult));
            MockAuditor(CreateMessageRequestAuditType, CreateMessageRequestAuditMessage);
            MockAuditor(CreateMessageResponseAuditType, "Patient practice message successfully sent");

            // Act
            var result = await _systemUnderTest.SendMessage(message);

            // Assert
            _mockPatientMessagesService.Verify();
            VerifyMockAuditor();

            result
                .Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().Be(successResponse);
        }

        [DataTestMethod]
        [DataRow(typeof(DeletePatientMessageResult.Forbidden), StatusCodes.Status403Forbidden,
            "Error deleting patient message: Forbidden")]
        [DataRow(typeof(DeletePatientMessageResult.InternalServerError), StatusCodes.Status500InternalServerError,
            "Error deleting patient message: Internal Server Error")]
        [DataRow(typeof(DeletePatientMessageResult.BadGateway), StatusCodes.Status502BadGateway,
            "Error deleting patient message: Bad Gateway")]
        [DataRow(typeof(DeletePatientMessageResult.BadRequest), StatusCodes.Status400BadRequest,
            "Error deleting patient message: Bad Request")]
        public async Task DeleteMessage_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode,
            string expectedAuditResponseMessage)
        {
            // Arrange
            var serviceResult = (DeletePatientMessageResult) Activator.CreateInstance(serviceResultType);

            _mockPatientMessagesService
                .Setup(s => s.DeleteMessage(
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession), "1"))
                .Returns(Task.FromResult(serviceResult))
                .Verifiable();

            MockErrorReferenceGenerator(expectedStatusCode);
            MockAuditor(DeletePatientPracticeMessageRequest, "Deleting a patient to practice message with id 1");
            MockAuditor(DeletePatientPracticeMessageResponse, expectedAuditResponseMessage);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.DeleteMessage("1");

            // Assert
            _mockPatientMessagesService.Verify();
            _mockErrorReferenceGenerator.Verify();
            VerifyMockAuditor();

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
        public async Task GetMessages_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
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

            MockErrorReferenceGenerator(expectedStatusCode);
            MockAuditor(GetMessagesRequestAuditType, GetMessagesRequestAuditMessage);
            MockAuditor(GetMessagesResponseAuditType, expectedAuditResponseMessage);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.GetMessages();

            // Assert
            _mockPatientMessagesService.Verify();
            _mockErrorReferenceGenerator.Verify();
            VerifyMockAuditor();

            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(expectedStatusCode);
            objectResult.Value.Should().BeEquivalentTo(expectedValue);
        }

        [DataTestMethod]
        [DataRow(typeof(GetPatientMessageResult.Forbidden), StatusCodes.Status403Forbidden,
            "Error retrieving patient message details: Forbidden")]
        [DataRow(typeof(GetPatientMessageResult.InternalServerError), StatusCodes.Status500InternalServerError,
            "Error retrieving patient message details: Internal Server Error")]
        [DataRow(typeof(GetPatientMessageResult.BadGateway), StatusCodes.Status502BadGateway,
            "Error retrieving patient message details: Bad Gateway")]
        [DataRow(typeof(GetPatientMessageResult.BadRequest), StatusCodes.Status400BadRequest,
            "Error retrieving patient message details: Bad Request")]
        public async Task GetMessageDetails_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode,
            string expectedAuditResponseMessage)
        {
            // Arrange
            var serviceResult = (GetPatientMessageResult) Activator.CreateInstance(serviceResultType);

            _mockPatientMessagesService
                .Setup(s => s.GetMessageDetails("1",
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession)))
                .Returns(Task.FromResult(serviceResult))
                .Verifiable();

            MockErrorReferenceGenerator(expectedStatusCode);
            MockAuditor(GetMessageRequestAuditType, GetMessageRequestAuditMessage);
            MockAuditor(GetMessageResponseAuditType, expectedAuditResponseMessage);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.GetMessageDetails("1");

            // Assert
            _mockPatientMessagesService.Verify();
            _mockErrorReferenceGenerator.Verify();
            VerifyMockAuditor();

            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(expectedStatusCode);
            objectResult.Value.Should().BeEquivalentTo(expectedValue);
        }

        [DataTestMethod]
        [DataRow(typeof(GetPatientMessageRecipientsResult.Forbidden), StatusCodes.Status403Forbidden,
            "Error retrieving patient message recipients: Forbidden")]
        [DataRow(typeof(GetPatientMessageRecipientsResult.InternalServerError), StatusCodes.Status500InternalServerError,
            "Error retrieving patient message recipients: Internal Server Error")]
        [DataRow(typeof(GetPatientMessageRecipientsResult.BadGateway), StatusCodes.Status502BadGateway,
            "Error retrieving patient message recipients: Bad Gateway")]
        [DataRow(typeof(GetPatientMessageRecipientsResult.BadRequest), StatusCodes.Status400BadRequest,
            "Error retrieving patient message recipients: Bad Request")]
        public async Task GetMessageRecipients_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode,
            string expectedAuditResponseMessage)
        {
            // Arrange
            var serviceResult = (GetPatientMessageRecipientsResult) Activator.CreateInstance(serviceResultType);

            _mockPatientMessagesService
                .Setup(s => s.GetMessageRecipients(
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession)))
                .Returns(Task.FromResult(serviceResult))
                .Verifiable();

            MockErrorReferenceGenerator(expectedStatusCode);
            MockAuditor(GetMessageRecipientsRequestAuditType, GetMessageRecipientsRequestAuditMessage);
            MockAuditor(GetMessageRecipientsResponseAuditType, expectedAuditResponseMessage);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.GetMessageRecipients();

            // Assert
            _mockPatientMessagesService.Verify();
            _mockErrorReferenceGenerator.Verify();
            VerifyMockAuditor();

            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(expectedStatusCode);
            objectResult.Value.Should().BeEquivalentTo(expectedValue);
        }

        [DataTestMethod]
        [DataRow(typeof(PutPatientMessageReadStatusResult.Forbidden), StatusCodes.Status403Forbidden,
            "Error updating unread status for message with id {0}: Forbidden")]
        [DataRow(typeof(PutPatientMessageReadStatusResult.InternalServerError), StatusCodes.Status500InternalServerError,
            "Error updating unread status for message with id {0}: Internal Server Error")]
        [DataRow(typeof(PutPatientMessageReadStatusResult.BadGateway), StatusCodes.Status502BadGateway,
            "Error updating unread status for message with id {0}: Bad Gateway")]
        [DataRow(typeof(PutPatientMessageReadStatusResult.BadRequest), StatusCodes.Status400BadRequest,
            "Error updating unread status for message with id {0}: Bad Request")]
        public async Task UpdateMessageMessageReadStatus_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode,
            string expectedAuditResponseMessageFormat)
        {
            // Arrange
            var requestBody = new UpdateMessageReadStatusRequestBody
            {
                MessageId = 1,
                MessageReadState = "Read"
            };
            var serviceResult = (PutPatientMessageReadStatusResult) Activator.CreateInstance(serviceResultType);

            _mockPatientMessagesService
                .Setup(s => s.UpdateMessageMessageReadStatus(
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession),
                    It.Is<UpdateMessageReadStatusRequestBody>(
                        p => p.MessageId == requestBody.MessageId &&
                             p.MessageReadState.Equals(requestBody.MessageReadState, StringComparison.Ordinal))))
                .Returns(Task.FromResult(serviceResult))
                .Verifiable();

            MockErrorReferenceGenerator(expectedStatusCode);
            MockAuditor(UpdateMessageUnreadStatusRequestAuditType,
                "Updating unread status for message with id 1 to Read");
            MockAuditor(UpdateMessageUnreadStatusResponseAuditType,
                string.Format(CultureInfo.InvariantCulture,
                    expectedAuditResponseMessageFormat,
                    1));

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.PostUpdateMessageReadStatus(requestBody);

            // Assert
            _mockPatientMessagesService.Verify();
            _mockErrorReferenceGenerator.Verify();
            VerifyMockAuditor();

            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(expectedStatusCode);
            objectResult.Value.Should().BeEquivalentTo(expectedValue);
        }

        [DataTestMethod]
        [DataRow(typeof(PostPatientMessageResult.Forbidden), StatusCodes.Status403Forbidden,
            "Error sending patient practice message: Forbidden")]
        [DataRow(typeof(PostPatientMessageResult.InternalServerError), StatusCodes.Status500InternalServerError,
            "Error sending patient practice message: Internal Server Error")]
        [DataRow(typeof(PostPatientMessageResult.BadGateway), StatusCodes.Status502BadGateway,
            "Error sending patient practice message: Bad Gateway")]
        [DataRow(typeof(PostPatientMessageResult.BadRequest), StatusCodes.Status400BadRequest,
            "Error sending patient practice message: Bad Request")]
        public async Task SendMessage_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode,
            string expectedAuditResponseMessage)
        {
            // Arrange
            var message = new CreatePatientMessage
            {
                Subject = "subject",
                MessageBody = "message",
                Recipient = "recipient 1"
            };
            var serviceResult = (PostPatientMessageResult) Activator.CreateInstance(serviceResultType);

            MockErrorReferenceGenerator(expectedStatusCode);
            MockAuditor(CreateMessageRequestAuditType, CreateMessageRequestAuditMessage);
            MockAuditor(CreateMessageResponseAuditType, expectedAuditResponseMessage);

            _mockPatientMessagesService
                .Setup(s => s.SendMessage(
                    It.Is<GpUserSession>(g => g == _userSession.GpUserSession),
                    message))
                .Returns(Task.FromResult(serviceResult));

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.SendMessage(message);

            // Assert
            _mockPatientMessagesService.Verify();
            _mockErrorReferenceGenerator.Verify();
            VerifyMockAuditor();

            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(expectedStatusCode);
            objectResult.Value.Should().BeEquivalentTo(expectedValue);
        }

        private void MockAuditor(string operation, string details)
        {
            _mockAuditor
                .Setup(x => x.Audit(operation, details))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        private void MockErrorReferenceGenerator(int statusCode)
        {
            _mockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    ErrorCategory.PatientPracticeMessages, statusCode, _userSession.GpUserSession.Supplier))
                .Returns(_serviceDeskReference)
                .Verifiable();
        }

        private void VerifyMockAuditor()
        {
            _mockAuditor.Verify();
            _mockAuditor.Verify(
                a => a.Audit(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(2));
        }
    }
}
