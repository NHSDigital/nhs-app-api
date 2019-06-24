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
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Prescriptions;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Prescriptions
{
    [TestClass]
    public class MicrotestPrescriptionServiceTests
    {
        private MicrotestPrescriptionService _systemUnderTest;
        private Mock<IMicrotestClient> _microtestClient;
        private Mock<IMicrotestPrescriptionMapper> _microtestPrescriptionMapper;
        private MicrotestConfigurationSettings _settings;
        private MicrotestUserSession _microtestUserSession;
        private IFixture _fixture;
        private RepeatPrescriptionRequest _repeatPrescriptionRequest;

        private const int PrescriptionsMaxCoursesSoftLimit = 100;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _microtestUserSession = _fixture.Freeze<MicrotestUserSession>();
            _microtestClient = _fixture.Freeze<Mock<IMicrotestClient>>();
            _microtestPrescriptionMapper = _fixture.Freeze<Mock<IMicrotestPrescriptionMapper>>();

            _settings = new MicrotestConfigurationSettings(
                null, string.Empty, string.Empty, string.Empty, PrescriptionsMaxCoursesSoftLimit, 0);

            _repeatPrescriptionRequest = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string>
                {
                    "766ecd82-3008-4454-95a5-98c423ce0527",
                    "166ecd82-3008-4454-95a5-98c423ce0527"
                },
                SpecialRequest = _fixture.Create<string>(),
            };
            _fixture.Inject(_settings);
            _systemUnderTest = _fixture.Create<MicrotestPrescriptionService>();
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromMicrotest()
        {
            // Arrange
            DateTimeOffset? fromDate = DateTimeOffset.Now;
            DateTimeOffset? toDate = DateTimeOffset.Now;

            var prescriptionsResponse = _fixture.Create<PrescriptionHistoryGetResponse>();

            _microtestClient.Setup(x => x.PrescriptionHistoryGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber, fromDate))
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<PrescriptionHistoryGetResponse>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                    }));

            var mappingResult = _fixture.Create<PrescriptionListResponse>();

            _microtestPrescriptionMapper.Setup(x => x.Map(prescriptionsResponse)).Returns(mappingResult);

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_microtestUserSession, fromDate, toDate);

            // Assert
            _microtestClient.Verify(x => x.PrescriptionHistoryGet(
                _microtestUserSession.OdsCode, _microtestUserSession.NhsNumber, fromDate)
            );
            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>();
            ((GetPrescriptionsResult.Success)result).Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task
            Get_PrescriptionsInResponseAreFilteredSoOnlyPrescriptionsWithRepeatCoursesWhichReturned_WhenSuccessfulResponseFromMicrotest()
        {
            // Arrange
            DateTimeOffset? fromDate = DateTimeOffset.Now;
            DateTimeOffset? toDate = DateTimeOffset.Now;

            var repeatCourseGuid = Guid.NewGuid().ToString();
            var nonRepeatCourseGuid1 = Guid.NewGuid().ToString();
            var nonRepeatCourseGuid2 = Guid.NewGuid().ToString();

            var prescriptionsResponse = new PrescriptionHistoryGetResponse
            {
                Courses = new List<PrescriptionCourse>
                {
                    new PrescriptionCourse
                    {
                        Id = repeatCourseGuid,
                        Type = PrescriptionType.Repeat,
                    },
                    new PrescriptionCourse
                    {
                        Id = nonRepeatCourseGuid1,
                        Type = "Other",
                    },
                    new PrescriptionCourse
                    {
                        Id = nonRepeatCourseGuid2,
                        Type = "Other",
                    },
                },
            };

            _microtestClient.Setup(x => x.PrescriptionHistoryGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber, fromDate))
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<PrescriptionHistoryGetResponse>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                    }));

            var response = new PrescriptionListResponse();
            PrescriptionHistoryGetResponse capturedItemToMap = null;
            _microtestPrescriptionMapper.Setup(x => x.Map(It.IsAny<PrescriptionHistoryGetResponse>())).Returns(response)
                .Callback<PrescriptionHistoryGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_microtestUserSession, fromDate, toDate);

            // Assert
            _microtestClient.Verify(x => x.PrescriptionHistoryGet(
                _microtestUserSession.OdsCode, _microtestUserSession.NhsNumber, fromDate)
            );
            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>();
            ((GetPrescriptionsResult.Success)result).Response.Should().NotBeNull();

            var getPrescriptionsResult = (GetPrescriptionsResult.Success)result;
            getPrescriptionsResult.Response.Should().Be(response);
            
            capturedItemToMap.Courses.Should().HaveCount(1);
            capturedItemToMap.Courses.ElementAt(0).Id.Should().Be(repeatCourseGuid);
        }

        [TestMethod]
        public async Task
            Get_PrescriptionsInResponseAreOrderedByDateRequestedDescending_WhenSuccessfulResponseFromMicrotest()
        {
            // Arrange
            DateTimeOffset? fromDate = DateTimeOffset.Now;
            DateTimeOffset? toDate = DateTimeOffset.Now;

            var repeatCourseGuidExpectedFirst = Guid.NewGuid().ToString();
            var repeatCourseGuidExpectedSecond = Guid.NewGuid().ToString();
            var repeatCourseGuidExpectedThird = Guid.NewGuid().ToString();

            var prescriptionsResponse = new PrescriptionHistoryGetResponse
            {
                Courses = new List<PrescriptionCourse>
                {
                    new PrescriptionCourse
                    {
                        Id = repeatCourseGuidExpectedThird,
                        Type = PrescriptionType.Repeat,
                        OrderDate = new DateTimeOffset(new DateTime(2000, 1, 1)),
                    },
                    new PrescriptionCourse
                    {
                        Id = repeatCourseGuidExpectedFirst,
                        Type = PrescriptionType.Repeat,
                        OrderDate = new DateTimeOffset(new DateTime(2000, 1, 3)),
                    },
                    new PrescriptionCourse
                    {
                        Id = repeatCourseGuidExpectedSecond,
                        Type = PrescriptionType.Repeat,
                        OrderDate = new DateTimeOffset(new DateTime(2000, 1, 2)),
                    }
                }
            };

            _microtestClient.Setup(x => x.PrescriptionHistoryGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber, fromDate))
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<PrescriptionHistoryGetResponse>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                    }));

            var response = new PrescriptionListResponse();
            PrescriptionHistoryGetResponse capturedItemToMap = null;
            _microtestPrescriptionMapper.Setup(x => x.Map(It.IsAny<PrescriptionHistoryGetResponse>())).Returns(response)
                .Callback<PrescriptionHistoryGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_microtestUserSession, fromDate, toDate);

            // Assert
            _microtestClient.Verify(x => x.PrescriptionHistoryGet(
                _microtestUserSession.OdsCode, _microtestUserSession.NhsNumber, fromDate)
            );
            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>();
            ((GetPrescriptionsResult.Success)result).Response.Should().NotBeNull();

            var getPrescriptionsResult = (GetPrescriptionsResult.Success)result;
            getPrescriptionsResult.Response.Should().Be(response);

            capturedItemToMap.Courses.Should().HaveCount(3);

            capturedItemToMap.Courses.ElementAt(0).Id.Should().Be(repeatCourseGuidExpectedFirst);
            capturedItemToMap.Courses.ElementAt(1).Id.Should().Be(repeatCourseGuidExpectedSecond);
            capturedItemToMap.Courses.ElementAt(2).Id.Should().Be(repeatCourseGuidExpectedThird);
        }

        [DataTestMethod]
        [DataRow(PrescriptionsMaxCoursesSoftLimit + 1, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit - 1, PrescriptionsMaxCoursesSoftLimit - 1)]
        public async Task Get_PrescriptionsInResponseAreLimitedToMax_WhenSuccessfulResponseFromMicrotest(
            int numberOfCoursesToCreate, int expectedNumberOfPrescriptions)
        {
            // Arrange
            DateTimeOffset? fromDate = DateTimeOffset.Now;
            DateTimeOffset? toDate = DateTimeOffset.Now;

            var medicationCourses = new List<PrescriptionCourse>();

            for (int i = 0; i < numberOfCoursesToCreate; i++)
            {
                var courseGuid = Guid.NewGuid().ToString();

                medicationCourses.Add(new PrescriptionCourse
                {
                    Id = courseGuid,
                    OrderDate = new DateTimeOffset(new DateTime(2000, 1, 2)),
                    Type = PrescriptionType.Repeat,
                });
            }

            var prescriptionsResponse = new PrescriptionHistoryGetResponse
            {
                Courses = medicationCourses,
            };

            _microtestClient.Setup(x => x.PrescriptionHistoryGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber, fromDate))
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<PrescriptionHistoryGetResponse>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                    }));

            var response = new PrescriptionListResponse();
            PrescriptionHistoryGetResponse capturedItemToMap = null;
            _microtestPrescriptionMapper.Setup(x => x.Map(It.IsAny<PrescriptionHistoryGetResponse>())).Returns(response)
                .Callback<PrescriptionHistoryGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_microtestUserSession, fromDate, toDate);

            // Assert
            _microtestClient.Verify(x => x.PrescriptionHistoryGet(
                _microtestUserSession.OdsCode, _microtestUserSession.NhsNumber, fromDate)
            );
            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>();
            ((GetPrescriptionsResult.Success)result).Response.Should().NotBeNull();

            var getPrescriptionsResult = (GetPrescriptionsResult.Success)result;
            getPrescriptionsResult.Response.Should().Be(response);

            capturedItemToMap.Courses.Should().HaveCount(expectedNumberOfPrescriptions);
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavilable_WhenErrorReceivedFromMicrotest()
        {
            // Arrange
            DateTimeOffset? fromDate = DateTimeOffset.Now;
            DateTimeOffset? toDate = DateTimeOffset.Now;

            _microtestClient.Setup(x => x.PrescriptionHistoryGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber, fromDate))
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<PrescriptionHistoryGetResponse>(HttpStatusCode.InternalServerError)));

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_microtestUserSession, fromDate, toDate);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.BadGateway>();
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavailable_WhenHttpExceptionOccursCallingMicrotest()
        {
            // Arrange
            DateTimeOffset? fromDate = DateTimeOffset.Now;
            DateTimeOffset? toDate = DateTimeOffset.Now;

            _microtestClient.Setup(x => x.PrescriptionHistoryGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber, fromDate))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_microtestUserSession, fromDate, toDate);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.BadGateway>();
            _microtestClient.Verify();
        }

        [TestMethod]
        public async Task Get_ReturnsInternalServerError_WhenNullExceptionOccursCallingMicrotest()
        {
            // Arrange
            DateTimeOffset? fromDate = DateTimeOffset.Now;
            DateTimeOffset? toDate = DateTimeOffset.Now;

            _microtestClient.Setup(x => x.PrescriptionHistoryGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber, fromDate))
                 .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<PrescriptionHistoryGetResponse>(HttpStatusCode.OK)
                    {
                        Body = null,
                    }));

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_microtestUserSession, fromDate, toDate);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.InternalServerError>();
            _microtestClient.Verify();
        }

        [TestMethod]
        public async Task Post_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromMicrotest()
        {
            // Arrange
            PrescriptionRequestsPost capturedPostRequest = null;

            var expectedRequest = new PrescriptionRequestsPost
            {
                CourseIds = _repeatPrescriptionRequest.CourseIds,
                SpecialRequestMessage = _repeatPrescriptionRequest.SpecialRequest,
            };

            _microtestClient.Setup(x => x.PrescriptionsPost(
                _microtestUserSession.OdsCode,
                _microtestUserSession.NhsNumber,
                It.IsAny<PrescriptionRequestsPost>()))
                .Callback<string, string, PrescriptionRequestsPost>((ods, nhs, prp) => capturedPostRequest = prp)
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<string>(HttpStatusCode.OK)));

            // Act
            var result = await _systemUnderTest.OrderPrescription(_microtestUserSession, _repeatPrescriptionRequest);

            // Assert
            _microtestClient.Verify(x => x.PrescriptionsPost(_microtestUserSession.OdsCode,
                _microtestUserSession.NhsNumber, It.IsAny<PrescriptionRequestsPost>()));
            var successResult = result.Should().BeAssignableTo<OrderPrescriptionResult.Success>().Subject;
            successResult.Should().NotBeNull();
            expectedRequest.Should().BeEquivalentTo(capturedPostRequest);
        }

        [TestMethod]
        public async Task Post_ReturnsConflict_WhenErrorReceivedFromEmis()
        {
            // Arrange
            _microtestClient.Setup(x => x.PrescriptionsPost(
                _microtestUserSession.OdsCode,
                _microtestUserSession.NhsNumber,
                It.IsAny<PrescriptionRequestsPost>()))
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<string>(HttpStatusCode.InternalServerError)));

            // Act
            var result = await _systemUnderTest.OrderPrescription(_microtestUserSession, _repeatPrescriptionRequest);

            // Assert
            result.Should().BeAssignableTo<OrderPrescriptionResult.BadGateway>();
        }
    }
}