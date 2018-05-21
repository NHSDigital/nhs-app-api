using System;
using System.Collections.Generic;
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
        private IOptions<ConfigurationSettings> _options;
        private EmisUserSession _userSession;
        private IFixture _fixture;

        private const int PrescriptionsMaxCoursesSoftLimit = 100;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _emisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _emisPrescriptionMapper = _fixture.Freeze<Mock<IEmisPrescriptionMapper>>();
            _userSession = _fixture.Freeze<EmisUserSession>();
            _options = Options.Create(new ConfigurationSettings
            {
                PrescriptionsMaxCoursesSoftLimit = PrescriptionsMaxCoursesSoftLimit
            });
            _fixture.Inject(_options);
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
        public async Task Get_PrescriptionsInResponseAreOrderedByDateRequestedDescending_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;

            var repeatCourseGuidExpectedFirst = Guid.NewGuid().ToString();
            var repeatCourseGuidExpectedSecond = Guid.NewGuid().ToString();
            var repeatCourseGuidExpectedThird = Guid.NewGuid().ToString();

            var prescriptionsResponse = new PrescriptionRequestsGetResponse
            {
                PrescriptionRequests = new List<PrescriptionRequest>
                {
                    new PrescriptionRequest
                    {
                        DateRequested = new DateTimeOffset(new DateTime(2000, 1, 2)),
                        RequestedMedicationCourses = new List<RequestedMedicationCourse>
                        {
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = repeatCourseGuidExpectedSecond,
                            }
                        }
                    },
                    new PrescriptionRequest
                    {
                        DateRequested = new DateTimeOffset(new DateTime(2000, 1, 1)),
                        RequestedMedicationCourses = new List<RequestedMedicationCourse>
                        {
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = repeatCourseGuidExpectedThird,
                            },
                        }
                    },
                    new PrescriptionRequest
                    {
                        DateRequested = new DateTimeOffset(new DateTime(2000, 1, 3)),
                        RequestedMedicationCourses = new List<RequestedMedicationCourse>
                        {
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = repeatCourseGuidExpectedFirst,
                            },
                        }
                    },
                },
                MedicationCourses = new List<MedicationCourse>
                {
                    new MedicationCourse
                    {
                        MedicationCourseGuid = repeatCourseGuidExpectedThird,
                        PrescriptionType = PrescriptionType.Repeat,
                    },
                    new MedicationCourse
                    {
                        MedicationCourseGuid = repeatCourseGuidExpectedFirst,
                        PrescriptionType = PrescriptionType.Repeat,
                    },
                    new MedicationCourse
                    {
                        MedicationCourseGuid = repeatCourseGuidExpectedSecond,
                        PrescriptionType = PrescriptionType.Repeat,
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

            capturedItemToMap.PrescriptionRequests.Should().HaveCount(3);

            capturedItemToMap.PrescriptionRequests.ElementAt(0).RequestedMedicationCourses.Should().HaveCount(1);
            capturedItemToMap.PrescriptionRequests.ElementAt(0).RequestedMedicationCourses.ElementAt(0).RequestedMedicationCourseGuid.Should().Be(repeatCourseGuidExpectedFirst);

            capturedItemToMap.PrescriptionRequests.ElementAt(1).RequestedMedicationCourses.Should().HaveCount(1);
            capturedItemToMap.PrescriptionRequests.ElementAt(1).RequestedMedicationCourses.ElementAt(0).RequestedMedicationCourseGuid.Should().Be(repeatCourseGuidExpectedSecond);

            capturedItemToMap.PrescriptionRequests.ElementAt(2).RequestedMedicationCourses.Should().HaveCount(1);
            capturedItemToMap.PrescriptionRequests.ElementAt(2).RequestedMedicationCourses.ElementAt(0).RequestedMedicationCourseGuid.Should().Be(repeatCourseGuidExpectedThird);
        }

        [DataTestMethod]
        [DataRow(PrescriptionsMaxCoursesSoftLimit + 1, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit - 1, PrescriptionsMaxCoursesSoftLimit - 1)]
        public async Task Get_PrescriptionsInResponseAreLimitedToMax_WhenSuccessfulResponseFromEmis(int numberOfCoursesToCreate, int expectedNumberOfPrescriptions)
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;

            var prescriptionRequests = new List<PrescriptionRequest>();
            var medicationCourses = new List<MedicationCourse>();

            for (int i = 0; i < numberOfCoursesToCreate; i++)
            {
                var courseGuid = Guid.NewGuid().ToString();

                prescriptionRequests.Add(new PrescriptionRequest
                {
                    DateRequested = new DateTimeOffset(new DateTime(2000, 1, 2)),
                    RequestedMedicationCourses = new List<RequestedMedicationCourse>
                        {
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = courseGuid,
                            }
                        }
                });

                medicationCourses.Add(new MedicationCourse
                {
                    MedicationCourseGuid = courseGuid,
                    PrescriptionType = PrescriptionType.Repeat,
                });
            }

            var prescriptionsResponse = new PrescriptionRequestsGetResponse
            {
                PrescriptionRequests = prescriptionRequests,
                MedicationCourses = medicationCourses,
            };

            _emisClient.Setup(x => x.PrescriptionsGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, date, toDate))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null,
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

            capturedItemToMap.PrescriptionRequests.Should().HaveCount(expectedNumberOfPrescriptions);
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
        
        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenNullReferenceExceptionOccursCallingEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _emisClient.Setup(x => x.PrescriptionsGet(_userSession.UserPatientLinkToken, _userSession.SessionId, _userSession.EndUserSessionId, date, toDate))
                .Throws<NullReferenceException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Get(_userSession, date, toDate);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.SupplierBadData>();
            _emisClient.Verify();
        }
    }
}