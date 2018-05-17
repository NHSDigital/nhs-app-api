using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.Bridges.Emis.Mappers;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.Router.Prescriptions;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis
{
    [TestClass]
    public class EmisPrescriptionServiceTests
    {
        private EmisPrescriptionService _systemUnderTest;
        private Mock<IEmisClient> _emisClient;
        private Mock<IEmisPrescriptionMapper> _emisPrescriptionMapper;
        private EmisUserSession _userSession;
        private static IFixture _fixture;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _emisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _emisPrescriptionMapper = _fixture.Freeze<Mock<IEmisPrescriptionMapper>>();
            _userSession = _fixture.Freeze<EmisUserSession>();
            _systemUnderTest = _fixture.Create<EmisPrescriptionService>();
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;

            var prescriptionsResponse = _fixture.Create<PrescriptionRequestsGetResponse>();

            _emisClient.Setup(x => x.PrescriptionsGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, date, toDate))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            // Act
            var result = await _systemUnderTest.Get(_userSession, date, toDate);

            // Assert
            _emisClient.Verify(x => x.PrescriptionsGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, date, toDate));
            result.Should().BeAssignableTo<GetPrescriptionsResult.SuccessfullyRetrieved>();
            ((GetPrescriptionsResult.SuccessfullyRetrieved)result).Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Get_PrescriptionsInResponseAreFilteredSoOnlyPrescriptionsWithRepeatCoursesWhichReturned_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;

            var repeatCourseGuid = Guid.NewGuid().ToString();
            var nonRepeatCourseGuid1 = Guid.NewGuid().ToString();
            var nonRepeatCourseGuid2 = Guid.NewGuid().ToString();

            var prescriptionsResponse = new PrescriptionRequestsGetResponse
            {
                PrescriptionRequests = new List<PrescriptionRequest>
                {
                    new PrescriptionRequest
                    {
                        RequestedMedicationCourses = new List<RequestedMedicationCourse>
                        {
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = repeatCourseGuid,
                            },
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = nonRepeatCourseGuid1,
                            }
                        }
                    },
                    new PrescriptionRequest
                    {
                        RequestedMedicationCourses = new List<RequestedMedicationCourse>
                        {
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = nonRepeatCourseGuid2,
                            },
                        }
                    }
                },
                MedicationCourses = new List<MedicationCourse>
                {
                    new MedicationCourse
                    {
                        MedicationCourseGuid = repeatCourseGuid,
                        PrescriptionType = PrescriptionType.Repeat,
                    },
                    new MedicationCourse
                    {
                        MedicationCourseGuid = nonRepeatCourseGuid1,
                        PrescriptionType = PrescriptionType.Acute,
                    },
                    new MedicationCourse
                    {
                        MedicationCourseGuid = nonRepeatCourseGuid2,
                        PrescriptionType = PrescriptionType.Automatic,
                    }
                }
            };

            _emisClient.Setup(x => x.PrescriptionsGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, date, toDate))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            var response = new PrescriptionListResponse();
            PrescriptionRequestsGetResponse capturedItemToMap = null;
            _emisPrescriptionMapper.Setup(x => x.Map(It.IsAny<PrescriptionRequestsGetResponse>())).Returns(response).Callback<PrescriptionRequestsGetResponse>((x) =>
            {
                capturedItemToMap = x;
            });

            // Act
            var result = await _systemUnderTest.Get(_userSession, date, toDate);

            // Assert
            _emisClient.Verify(x => x.PrescriptionsGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, date, toDate));
            result.Should().BeAssignableTo<GetPrescriptionsResult.SuccessfullyRetrieved>();
            ((GetPrescriptionsResult.SuccessfullyRetrieved)result).Response.Should().NotBeNull();

            var getPrescriptionsResult = (GetPrescriptionsResult.SuccessfullyRetrieved)result;
            getPrescriptionsResult.Response.Should().Be(response);

            capturedItemToMap.PrescriptionRequests.Should().HaveCount(1);
            capturedItemToMap.PrescriptionRequests.ElementAt(0).RequestedMedicationCourses.Should().HaveCount(1);
            capturedItemToMap.PrescriptionRequests.ElementAt(0).RequestedMedicationCourses.ElementAt(0).RequestedMedicationCourseGuid.Should().Be(repeatCourseGuid);
            capturedItemToMap.MedicationCourses.Should().HaveCount(1);
            capturedItemToMap.MedicationCourses.ElementAt(0).MedicationCourseGuid.Should().Be(repeatCourseGuid);
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenErrorReceivedFromEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;
            
            _emisClient.Setup(x => x.PrescriptionsGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, date, toDate))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>(HttpStatusCode.InternalServerError)
                        { ErrorResponse = errorResponse }));

            // Act
            var result = await _systemUnderTest.Get(_userSession, date, toDate);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.Unsuccessful>();
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _emisClient.Setup(x => x.PrescriptionsGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, date, toDate))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get(_userSession, date, toDate);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.Unsuccessful>();
            _emisClient.Verify();
        }
    }
}