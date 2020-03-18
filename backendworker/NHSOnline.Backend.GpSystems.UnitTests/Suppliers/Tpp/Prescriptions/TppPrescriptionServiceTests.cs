using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Prescriptions
{
    [TestClass]
    public class TppPrescriptionServiceTests
    {
        private TppPrescriptionService _systemUnderTest;
        private Mock<ITppClientRequest<TppUserSession, ListRepeatMedicationReply>> _listRepeatMedication;
        private Mock<ITppPrescriptionMapper> _tppPrescriptionMapper;
        private Mock<ILogger<TppPrescriptionService>> _logger;
        private TppConfigurationSettings _settings;
        private TppUserSession _tppUserSession;
        private IFixture _fixture;
        private Guid _patientId;
        private GpLinkedAccountModel _gpLinkedAccountModel;
        private const string ApplicationName = "appName";
        private const string ApplicationVersion = "13";
        private const string ApplicationProviderId = "providerId";
        private const string ApplicationDeviceType = "deviceType";
        private static readonly Uri ApiUrl = new Uri("http://tppapitest:60015/Test/");
        private Mock<ITppClientRequest<(TppUserSession, RequestMedication), RequestMedicationReply>> _orderPrescriptionsPost;
        private Mock<ITppClientRequest<(RequestSystmOnlineMessages, string), RequestSystmOnlineMessagesReply>> _requestSystmOnlineMessages;
        private const string ApiVersion = "12";
        private const string CertificatePath = "CertificatePath";
        private const string CertificatePassphrase = "CerticiatePassphrase";
        private const int PrescriptionsMaxCoursesSoftLimit = 100;
        private const int CoursesMaxCoursesLimit = 100;
        private const string Environment = "environment";
        private const string SupportsLinkedAccounts = "true";

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _tppUserSession = _fixture.Create<TppUserSession>();
            _gpLinkedAccountModel = new GpLinkedAccountModel(_tppUserSession, _patientId);

            _listRepeatMedication = _fixture.Freeze<Mock<ITppClientRequest<TppUserSession, ListRepeatMedicationReply>>>();
            _orderPrescriptionsPost =
                _fixture.Freeze<Mock<ITppClientRequest<(TppUserSession, RequestMedication), RequestMedicationReply>>>();
            _requestSystmOnlineMessages = _fixture.Freeze<Mock<ITppClientRequest<(RequestSystmOnlineMessages, string), RequestSystmOnlineMessagesReply>>>();
            _tppPrescriptionMapper = _fixture.Freeze<Mock<ITppPrescriptionMapper>>();
            _logger = _fixture.Freeze<Mock<ILogger<TppPrescriptionService>>>();

            _settings = new TppConfigurationSettings(ApiUrl, ApiVersion, ApplicationName, ApplicationVersion, ApplicationProviderId, ApplicationDeviceType,
                CertificatePath, CertificatePassphrase, PrescriptionsMaxCoursesSoftLimit, CoursesMaxCoursesLimit, Environment, SupportsLinkedAccounts);

            _fixture.Inject(_settings);
            _systemUnderTest = _fixture.Create<TppPrescriptionService>();
        }

        #region Get Prescriptions

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            var prescriptionsResponse = _fixture.Create<ListRepeatMedicationReply>();

            _listRepeatMedication.Setup(x => x.Post(_tppUserSession))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                    }));

            _requestSystmOnlineMessages.Setup(x => x.Post(It.IsAny<(RequestSystmOnlineMessages, string)>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<RequestSystmOnlineMessagesReply>(HttpStatusCode.OK)
                    {
                        Body = new RequestSystmOnlineMessagesReply()
                        {
                            Medication = "Bazz",
                            RequestMedicationConfirmation = "Bozz",
                        }
                    }));

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_gpLinkedAccountModel);

            // Assert
            _listRepeatMedication.Verify(x => x.Post(_tppUserSession));
            _requestSystmOnlineMessages.Verify(x => x.Post(It.IsAny<(RequestSystmOnlineMessages, string)>()));

            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }

        [DataTestMethod]
        [DataRow(PrescriptionsMaxCoursesSoftLimit + 1, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit - 1, PrescriptionsMaxCoursesSoftLimit - 1)]
        public async Task Get_PrescriptionsInResponseAreLimitedToMax_WhenSuccessfulResponseFromTpp(
            int numberOfCoursesToCreate, int expectedNumberOfPrescriptions)
        {
            // Arrange
            var medications = new List<Medication>();

            for (var i = 0; i < numberOfCoursesToCreate; i++)
            {
                medications.Add(new Medication
                {
                    DrugId = Guid.NewGuid().ToString(),
                    Drug = "Drug " + i,
                    Details = _fixture.Create<string>(),
                    Requestable = "y",
                    Type = "Repeat",
                });
            }

            var listRepeatMedicationReply = new ListRepeatMedicationReply()
            {
                Medications = medications
            };

            _listRepeatMedication.Setup(x => x.Post(_tppUserSession)).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = listRepeatMedicationReply,
                    }));

            _requestSystmOnlineMessages.Setup(x => x.Post(It.IsAny<(RequestSystmOnlineMessages, string)>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<RequestSystmOnlineMessagesReply>(HttpStatusCode.OK)
                    {
                        Body = new RequestSystmOnlineMessagesReply()
                        {
                            Medication = "Bazz",
                            RequestMedicationConfirmation = "Bozz",
                        }
                    }));

            var response = new PrescriptionListResponse();
            List<Medication> capturedItemToMap = null;
            _tppPrescriptionMapper.Setup(x => x.Map(It.IsAny<List<Medication>>())).Returns(response)
                .Callback<List<Medication>>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_gpLinkedAccountModel);

            // Assert
            _listRepeatMedication.Verify(x => x.Post(_tppUserSession));
            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>()
                .Subject.Response.Should().Be(response);

            _requestSystmOnlineMessages.Verify(x => x.Post(It.IsAny<(RequestSystmOnlineMessages, string)>()));
            capturedItemToMap.Should().HaveCount(expectedNumberOfPrescriptions);
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponse_WhenSuccessfulResponseToGetPrescriptions_ButUnsuccessfulCallToRequestSystmOnlineMessages()
        {
            // Arrange
            var prescriptionsResponse = _fixture.Create<ListRepeatMedicationReply>();

            _listRepeatMedication.Setup(x => x.Post(_tppUserSession))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                    }));

            _requestSystmOnlineMessages.Setup(x => x.Post(It.IsAny<(RequestSystmOnlineMessages, string)>()))
                .ThrowsAsync(new TimeoutException());

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_gpLinkedAccountModel);

            // Assert
            _listRepeatMedication.Verify(x => x.Post(_tppUserSession));
            _requestSystmOnlineMessages.Verify(x => x.Post(It.IsAny<(RequestSystmOnlineMessages, string)>()));

            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>()
                .Subject.Response.Should().NotBeNull();

            _logger.VerifyLogger(LogLevel.Error, "Exception has been thrown calling TPP.", Times.Once());
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavailable_WhenErrorReceivedFromTpp()
        {
            // Arrange
            _listRepeatMedication.Setup(x => x.Post(_tppUserSession))
                .Returns(
                    Task.FromResult(
                        new TppApiObjectResponse<ListRepeatMedicationReply>
                            (HttpStatusCode.InternalServerError)
                            {
                                ErrorResponse = _fixture.Create<Error>()
                            }));
            // Act
            var result = await _systemUnderTest.GetPrescriptions(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.BadGateway>();
        }

        [TestMethod]
        public async Task Get_ReturnsBadGateway_WhenHttpExceptionOccursCallingTpp()
        {
            // Arrange
            _listRepeatMedication.Setup(x => x.Post(_tppUserSession))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.BadGateway>();
            _listRepeatMedication.Verify();
        }

        [TestMethod]
        public async Task Get_ReturnsForbidden_WhenErrorReceivedNoAccessFromTpp()
        {
            // Arrange
            var expectedError = _fixture.Build<Error>()
                .With(x => x.ErrorCode, TppApiErrorCodes.NoAccess)
                .Create();

            _listRepeatMedication.Setup(x => x.Post(_tppUserSession))
                .Returns(
                    Task.FromResult(
                        new TppApiObjectResponse<ListRepeatMedicationReply>
                            (HttpStatusCode.InternalServerError)
                            {
                                ErrorResponse = expectedError
                            }));
            // Act
            var result = await _systemUnderTest.GetPrescriptions(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.Forbidden>();
        }

        #endregion

        #region Post Prescriptions

         [TestMethod]
        public async Task Post_ReturnsConflict_WhenAlreadyOrdered_IsUnavailable_ErrorReceivedFromTpp()
        {
            // Arrange
            var request = _fixture.Create<RepeatPrescriptionRequest>();

            var error = _fixture.Build<Error>()
                .With(x => x.UserFriendlyMessage, TppApiErrorMessages.Prescriptions_CourseAlreadyOrdered_IsUnavailable)
                .Create();

            _orderPrescriptionsPost.Setup(x => x.Post(It.Is<(TppUserSession, RequestMedication)>(
                tuple => tuple.Item1.Equals(_tppUserSession))))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<RequestMedicationReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = error,
                    }));

            // Act
            var result = await _systemUnderTest.OrderPrescription(_gpLinkedAccountModel, request);

            // Assert
            _orderPrescriptionsPost.Verify(x => x.Post(It.Is<(TppUserSession, RequestMedication)>(
                tuple => tuple.Item1.Equals(_tppUserSession))));
            result.Should().BeAssignableTo<OrderPrescriptionResult.CannotReorderPrescription>();
        }

        [TestMethod]
        public async Task Post_ReturnsForbidden_WhenNoAccess_ErrorReceivedFromTpp()
        {
            // Arrange
            var request = _fixture.Create<RepeatPrescriptionRequest>();

            var error = _fixture.Build<Error>()
                .With(x => x.ErrorCode, TppApiErrorCodes.NoAccess)
                .Create();

            _orderPrescriptionsPost.Setup(x => x.Post(It.Is<(TppUserSession, RequestMedication)>(
                    tuple => tuple.Item1.Equals(_tppUserSession))))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<RequestMedicationReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = error,
                    }));

            // Act
            var result = await _systemUnderTest.OrderPrescription(_gpLinkedAccountModel, request);

            // Assert
            _orderPrescriptionsPost.Verify(x => x.Post(It.Is<(TppUserSession, RequestMedication)>(
                tuple => tuple.Item1.Equals(_tppUserSession))));
            result.Should().BeAssignableTo<OrderPrescriptionResult.Forbidden>();
        }

        [DataTestMethod]
        [DataRow("Prescriptions_RequestNoteTooLarge", "Tpp request note too large")]
        [DataRow("Prescriptions_InvalidCourseIds", "Invalid tpp course id or ids")]
        [DataRow("Prescriptions_MustViewMedicationsListFirst", "Tpp must view medications before requesting")]
        public async Task Post_ReturnsBadRequest_WhenErrorReceivedFromTpp(string errorKey, string scenario)
        {
            // Arrange
            var request = _fixture.Create<RepeatPrescriptionRequest>();

            var error = _fixture.Build<Error>()
                .With(x => x.UserFriendlyMessage, TppApiErrorMessages.ResourceManager
                    .GetString(errorKey, CultureInfo.InvariantCulture))
                .Create();

            _orderPrescriptionsPost.Setup(x => x.Post(It.Is<(TppUserSession, RequestMedication)>(
                    tuple => tuple.Item1.Equals(_tppUserSession))))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<RequestMedicationReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = error,
                    }));

            // Act
            var result = await _systemUnderTest.OrderPrescription(_gpLinkedAccountModel, request);

            // Assert
            _orderPrescriptionsPost.Verify(x => x.Post(It.Is<(TppUserSession, RequestMedication)>(
                tuple => tuple.Item1.Equals(_tppUserSession))));
            result.Should().BeAssignableTo<OrderPrescriptionResult.BadRequest>(scenario);
        }

        [TestMethod]
        public async Task Post_ReturnsBadGateway_WhenGeneralErrorReceivedFromTpp()
        {
            // Arrange
            var request = _fixture.Create<RepeatPrescriptionRequest>();

            var error = _fixture.Build<Error>()
                .With(x => x.UserFriendlyMessage, "General error")
                .Create();

            _orderPrescriptionsPost.Setup(x => x.Post(It.Is<(TppUserSession, RequestMedication)>(
                    tuple => tuple.Item1.Equals(_tppUserSession))))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<RequestMedicationReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = error,
                    }));

            // Act
            var result = await _systemUnderTest.OrderPrescription(_gpLinkedAccountModel, request);

            // Assert
            _orderPrescriptionsPost.Verify(x => x.Post(It.Is<(TppUserSession, RequestMedication)>(
                tuple => tuple.Item1.Equals(_tppUserSession))));
            result.Should().BeAssignableTo<OrderPrescriptionResult.BadGateway>();
        }
        #endregion

        [TestMethod]
        public async Task OrderPrescription_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            var request = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                SpecialRequest = "Can I collect these before the weekend please?",
            };

            var prescriptionsResponse = _fixture.Create<RequestMedicationReply>();

            RequestMedication capturedRequestMedication = null;

            _orderPrescriptionsPost.Setup(x => x.Post(It.Is<(TppUserSession, RequestMedication)>(
                    tuple => tuple.Item1.Equals(_tppUserSession))))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<RequestMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                    }))
                    .Callback<(TppUserSession, RequestMedication)>(tuple =>
                    {
                        capturedRequestMedication = tuple.Item2;
                    });

            // Act
            var result = await _systemUnderTest.OrderPrescription(_gpLinkedAccountModel, request);

            // Assert
            _orderPrescriptionsPost.Verify(x => x.Post(It.Is<(TppUserSession, RequestMedication)>(
                tuple => tuple.Item1.Equals(_tppUserSession))));
            capturedRequestMedication.RequestType.Should().Be("RequestMedication");
            capturedRequestMedication.Medications.Should().HaveCount(request.CourseIds.Count());
            capturedRequestMedication.Medications.ElementAt(0).DrugId.Should().Be(request.CourseIds.ElementAt(0));
            capturedRequestMedication.Medications.ElementAt(1).DrugId.Should().Be(request.CourseIds.ElementAt(1));
            capturedRequestMedication.Medications.ForEach(x => x.Type.Should().Be("Repeat"));
            result.Should().BeAssignableTo<OrderPrescriptionResult.Success>();
        }

        [TestMethod]
        public async Task OrderPrescription_ReturnsUnsuccessfulResponseForSadPath_WhenErrorResponseFromTpp()
        {
            // Arrange
            var request = _fixture.Create<RepeatPrescriptionRequest>();

            var error = _fixture.Create<Error>();

            _orderPrescriptionsPost.Setup(x => x.Post(It.Is<(TppUserSession, RequestMedication)>(
                    tuple => tuple.Item1.Equals(_tppUserSession))))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<RequestMedicationReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = error,
                    }));

            // Act
            var result = await _systemUnderTest.OrderPrescription(_gpLinkedAccountModel, request);

            // Assert
            _orderPrescriptionsPost.Verify(x => x.Post(It.Is<(TppUserSession, RequestMedication)>(
                tuple => tuple.Item1.Equals(_tppUserSession))));
            result.Should().BeAssignableTo<OrderPrescriptionResult.BadGateway>();
        }
    }
}
