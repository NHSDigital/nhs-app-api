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
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Prescriptions
{
    [TestClass]
    public class TppPrescriptionServiceTests
    {
        private TppPrescriptionService _systemUnderTest;
        private Mock<ITppClient> _tppClient;
        private Mock<ITppPrescriptionMapper> _tppPrescriptionMapper;
        private IOptions<ConfigurationSettings> _options;
        private TppUserSession _tppUserSession;
        private IFixture _fixture;

        private const int PrescriptionsMaxCoursesSoftLimit = 100;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _tppUserSession = _fixture.Create<TppUserSession>();

            _tppClient = _fixture.Freeze<Mock<ITppClient>>();
            _tppPrescriptionMapper = _fixture.Freeze<Mock<ITppPrescriptionMapper>>();
                        
            _options = Options.Create(new ConfigurationSettings
            {
                PrescriptionsMaxCoursesSoftLimit = PrescriptionsMaxCoursesSoftLimit
            });
            _fixture.Inject(_options);
            _systemUnderTest = _fixture.Create<TppPrescriptionService>();
        }

        #region Get Prescriptions

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            var prescriptionsResponse = _fixture.Create<ListRepeatMedicationReply>();

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_tppUserSession))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                    }));

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_tppUserSession);

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_tppUserSession));
            result.Should().BeAssignableTo<PrescriptionResult.SuccessfulGet>();
            ((PrescriptionResult.SuccessfulGet) result).Response.Should().NotBeNull();
        }

        [DataTestMethod]
        [DataRow(PrescriptionsMaxCoursesSoftLimit + 1, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit - 1, PrescriptionsMaxCoursesSoftLimit - 1)]
        public async Task Get_PrescriptionsInResponseAreLimitedToMax_WhenSuccessfulResponseFromEmis(
            int numberOfCoursesToCreate, int expectedNumberOfPrescriptions)
        {
            // Arrange
            var medications = new List<Medication>();

            for (int i = 0; i < numberOfCoursesToCreate; i++)
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

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_tppUserSession)).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = listRepeatMedicationReply,
                    }));

            var response = new PrescriptionListResponse();
            List<Medication> capturedItemToMap = null;
            _tppPrescriptionMapper.Setup(x => x.Map(It.IsAny<List<Medication>>())).Returns(response)
                .Callback<List<Medication>>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_tppUserSession);

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_tppUserSession));
            result.Should().BeAssignableTo<PrescriptionResult.SuccessfulGet>();
            ((PrescriptionResult.SuccessfulGet) result).Response.Should().NotBeNull();

            var getPrescriptionsResult = (PrescriptionResult.SuccessfulGet) result;
            getPrescriptionsResult.Response.Should().Be(response);

            capturedItemToMap.Should().HaveCount(expectedNumberOfPrescriptions);
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavilable_WhenErrorReceivedFromEmis()
        {
            // Arrange
            _tppClient.Setup(x => x.ListRepeatMedicationPost(_tppUserSession))
                .Returns(
                    Task.FromResult(
                        new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>
                            (HttpStatusCode.InternalServerError)
                            {
                                ErrorResponse = _fixture.Create<Error>()
                            }));
            // Act
            var result = await _systemUnderTest.GetPrescriptions(_tppUserSession);

            // Assert
            result.Should().BeAssignableTo<PrescriptionResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavailable_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            _tppClient.Setup(x => x.ListRepeatMedicationPost(_tppUserSession))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_tppUserSession);

            // Assert
            result.Should().BeAssignableTo<PrescriptionResult.SupplierSystemUnavailable>();
            _tppClient.Verify();
        }
        
        [TestMethod]
        public async Task Get_ReturnsForbidden_WhenErrorReceivedNoAccessFromTpp()
        {
            var expectedError = _fixture.Create<Error>();

            // Tpp forbidden error code 
            expectedError.ErrorCode = TppApiErrorCodes.NoAccess;
            
            // Arrange
            _tppClient.Setup(x => x.ListRepeatMedicationPost(_tppUserSession))
                .Returns(
                    Task.FromResult(
                        new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>
                            (HttpStatusCode.InternalServerError)
                            {
                                ErrorResponse = expectedError
                            }));
            // Act
            var result = await _systemUnderTest.GetPrescriptions(_tppUserSession);

            // Assert
            result.Should().BeAssignableTo<PrescriptionResult.SupplierNotEnabled>();
        }

        #endregion
        
        #region Post Prescriptions
        
         [TestMethod]
        public async Task Post_ReturnsConflict_WhenAlreadyOrdered_IsUnavailable_ErrorReceivedFromTpp()
        {
            // Arrange
            var request = _fixture.Create<RepeatPrescriptionRequest>();

            var error = _fixture.Create<Error>();
            error.UserFriendlyMessage = TppApiErrorMessages.Prescriptions_CourseAlreadyOrdered_IsUnavailable;

            _tppClient.Setup(x => x.OrderPrescriptionsPost(_tppUserSession, It.IsAny<RequestMedication>()))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<RequestMedicationReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = error,
                    }));

            // Act
            var result = await _systemUnderTest.OrderPrescription(_tppUserSession, request);

            // Assert
            _tppClient.Verify(x => x.OrderPrescriptionsPost(_tppUserSession, It.IsAny<RequestMedication>()));
            result.Should().BeAssignableTo<PrescriptionResult.CannotReorderPrescription>();
        }

        [TestMethod]
        public async Task Post_ReturnsForbidden_WhenNoAccess_ErrorReceivedFromTpp()
        {
            // Arrange
            var request = _fixture.Create<RepeatPrescriptionRequest>();

            var error = _fixture.Create<Error>();
            error.ErrorCode = TppApiErrorCodes.NoAccess;

            _tppClient.Setup(x => x.OrderPrescriptionsPost(_tppUserSession, It.IsAny<RequestMedication>()))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<RequestMedicationReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = error,
                    }));

            // Act
            var result = await _systemUnderTest.OrderPrescription(_tppUserSession, request);

            // Assert
            _tppClient.Verify(x => x.OrderPrescriptionsPost(_tppUserSession, It.IsAny<RequestMedication>()));
            result.Should().BeAssignableTo<PrescriptionResult.SupplierNotEnabled>();
        }

        [DataTestMethod]
        [DataRow("Prescriptions_RequestNoteTooLarge", "Tpp request note too large")]
        [DataRow("Prescriptions_InvalidCourseIds", "Invalid tpp course id or ids")]
        [DataRow("Prescriptions_MustViewMedicationsListFirst", "Tpp must view medications before requesting")]
        public async Task Post_ReturnsBadRequest_WhenErrorReceivedFromTpp(string errorKey, string scenario)
        {
            // Arrange
            var request = _fixture.Create<RepeatPrescriptionRequest>();

            var error = _fixture.Create<Error>();
            error.UserFriendlyMessage = TppApiErrorMessages.ResourceManager
                .GetString(errorKey, CultureInfo.InvariantCulture);

            _tppClient.Setup(x => x.OrderPrescriptionsPost(_tppUserSession, It.IsAny<RequestMedication>()))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<RequestMedicationReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = error,
                    }));

            // Act
            var result = await _systemUnderTest.OrderPrescription(_tppUserSession, request);

            // Assert
            _tppClient.Verify(x => x.OrderPrescriptionsPost(_tppUserSession, It.IsAny<RequestMedication>()));
            result.Should().BeAssignableTo<PrescriptionResult.BadRequest>(scenario);
        }

        [TestMethod]
        public async Task Post_ReturnsBadGateway_WhenGeneralErrorReceivedFromTpp()
        {
            // Arrange
            var request = _fixture.Create<RepeatPrescriptionRequest>();

            var error = _fixture.Create<Error>();
            error.UserFriendlyMessage = "General error";

            _tppClient.Setup(x => x.OrderPrescriptionsPost(_tppUserSession, It.IsAny<RequestMedication>()))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<RequestMedicationReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = error,
                    }));

            // Act
            var result = await _systemUnderTest.OrderPrescription(_tppUserSession, request);

            // Assert
            _tppClient.Verify(x => x.OrderPrescriptionsPost(_tppUserSession, It.IsAny<RequestMedication>()));
            result.Should().BeAssignableTo<PrescriptionResult.SupplierSystemUnavailable>();
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

            _tppClient.Setup(x => x.OrderPrescriptionsPost(_tppUserSession, It.IsAny<RequestMedication>()))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<RequestMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                    }))
                    .Callback<TppUserSession, RequestMedication>((tppUserSession, requestMedication) =>
                    {
                        capturedRequestMedication = requestMedication;
                    });

            // Act
            var result = await _systemUnderTest.OrderPrescription(_tppUserSession, request);

            // Assert
            _tppClient.Verify(x => x.OrderPrescriptionsPost(_tppUserSession, It.IsAny<RequestMedication>()));
            capturedRequestMedication.RequestType.Should().Be("RequestMedication");
            capturedRequestMedication.Medications.Should().HaveCount(request.CourseIds.Count());
            capturedRequestMedication.Medications.ElementAt(0).DrugId.Should().Be(request.CourseIds.ElementAt(0));
            capturedRequestMedication.Medications.ElementAt(1).DrugId.Should().Be(request.CourseIds.ElementAt(1));
            capturedRequestMedication.Medications.ForEach(x => x.Type.Should().Be("Repeat"));
            result.Should().BeAssignableTo<PrescriptionResult.SuccessfulPost>();
        }

        [TestMethod]
        public async Task OrderPrescription_ReturnsUnsuccessfulResponseForSadPath_WhenErrorResponseFromTpp()
        {
            // Arrange
            var request = _fixture.Create<RepeatPrescriptionRequest>();

            var error = _fixture.Create<Error>();

            _tppClient.Setup(x => x.OrderPrescriptionsPost(_tppUserSession, It.IsAny<RequestMedication>()))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<RequestMedicationReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = error,
                    }));

            // Act
            var result = await _systemUnderTest.OrderPrescription(_tppUserSession, request);

            // Assert
            _tppClient.Verify(x => x.OrderPrescriptionsPost(_tppUserSession, It.IsAny<RequestMedication>()));
            result.Should().BeAssignableTo<PrescriptionResult.SupplierSystemUnavailable>();
        }

    }
}
