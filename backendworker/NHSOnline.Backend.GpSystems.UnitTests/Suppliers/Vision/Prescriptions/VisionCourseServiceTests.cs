using System;
using System.Collections.Generic;
using System.Globalization;
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
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Prescriptions
{
    [TestClass]
    public class VisionCourseServiceTests
    {
        private VisionCourseService _systemUnderTest;
        private Mock<IVisionClient> _visionClient;
        private Mock<IVisionPrescriptionMapper> _visionMapper;
        private VisionConfigurationSettings _settings;
        private VisionUserSession _visionUserSession;
        private IFixture _fixture;
        private VisionResponseEnvelope<EligibleRepeatsResponse> _eligibleRepeatsResponse;
        private const string ApplicationProviderId = "ApplicationProviderId";
        private const string RequestUserName = "username";
        private const string CertificatePassphrase = "CertificatePassphrase";
        private const string CertificatePath = "CertificatePath";
        private const string VisionSenderUserName = "visionuser";
        private const string VisionSenderFullName = "visionuser";
        private const string VisionSenderUserIdentity = "username";
        private const string VisionSenderUserRole = "admin";
        private static readonly Uri ApiUrl = new Uri("http://vision_base_url/", UriKind.Absolute);
        private const int PrescriptionsMaxCoursesSoftLimit = 100;
        private const int CoursesMaxCoursesLimit = 100;
        private const int VisionAppointmentSlotsRequestCount = 100;
        private const string Environment = "environment";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _visionUserSession = _fixture.Create<VisionUserSession>();
            _visionUserSession.IsRepeatPrescriptionsEnabled = true;

            _visionClient = _fixture.Freeze<Mock<IVisionClient>>();
            
            _visionMapper = _fixture.Freeze<Mock<IVisionPrescriptionMapper>>();

            _settings = new VisionConfigurationSettings(ApplicationProviderId, ApiUrl, 
                CertificatePath, CertificatePassphrase, RequestUserName, VisionSenderUserName, 
                VisionSenderFullName, VisionSenderUserIdentity, VisionSenderUserRole, VisionAppointmentSlotsRequestCount, 
                CoursesMaxCoursesLimit, PrescriptionsMaxCoursesSoftLimit, Environment);

            _fixture.Inject(_settings);
            _systemUnderTest = _fixture.Create<VisionCourseService>();

            _eligibleRepeatsResponse = new VisionResponseEnvelope<EligibleRepeatsResponse>
            {
                Body = new VisionResponseBody<EligibleRepeatsResponse>
                {
                    VisionResponse = new VisionResponse<EligibleRepeatsResponse>
                    {
                        ServiceContent = new EligibleRepeatsResponse
                        {
                            EligibleRepeats = new EligibleRepeats
                            {
                                Repeats = new List<Repeat>(_fixture.CreateMany<Repeat>()),
                                Settings = new CourseSettings
                                {
                                    AllowFreeText = true,
                                },
                            }
                        }
                    }
                }
            };
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessResponseForHappyPath_WhenSuccessfulResponseFromVision()
        {
            _visionClient.Setup(x => x.GetEligibleRepeats(_visionUserSession))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<EligibleRepeatsResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = _eligibleRepeatsResponse,
                    }));

            var mappedResponse = new CourseListResponse
            {
                Courses = new List<Course>
                {
                    _fixture.Create<Course>(),
                    _fixture.Create<Course>(),
                }
            };

            _visionMapper.Setup(x =>
                    x.Map(_eligibleRepeatsResponse.Body.VisionResponse.ServiceContent.EligibleRepeats))
                .Returns(mappedResponse);

            // Act
            var result = await _systemUnderTest.GetCourses(_visionUserSession);

            // Assert
            _visionClient.Verify(x => x.GetEligibleRepeats(_visionUserSession));
            result.Should().BeAssignableTo<GetCoursesResult.Success>();
            ((GetCoursesResult.Success) result).Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Get_UpdatesSessionWithCurrentValueForEligibleRepeatsAllowFreeTextValue_WhenSuccessfulResponseFromVision()
        {
            _visionUserSession.AllowFreeTextPrescriptions = false;
            _eligibleRepeatsResponse.Body.VisionResponse.ServiceContent.EligibleRepeats.Settings.AllowFreeText = true;

            _visionClient.Setup(x => x.GetEligibleRepeats(_visionUserSession))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<EligibleRepeatsResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = _eligibleRepeatsResponse,
                    }));

            var mappedResponse = new CourseListResponse();

            _visionMapper.Setup(x =>
                    x.Map(_eligibleRepeatsResponse.Body.VisionResponse.ServiceContent.EligibleRepeats))
                .Returns(mappedResponse);

            // Act
            var result = await _systemUnderTest.GetCourses(_visionUserSession);

            // Assert
            _visionClient.Verify(x => x.GetEligibleRepeats(_visionUserSession));
            Assert.IsTrue(_visionUserSession.AllowFreeTextPrescriptions); // should be updated
            result.Should().BeAssignableTo<GetCoursesResult.Success>();

            ((GetCoursesResult.Success)result).Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetCourses_ReturnsForbidden_WhenRepeatPrescriptionsIsDisabledInUserSession()
        {
            // Arrange
            _visionUserSession.IsRepeatPrescriptionsEnabled = false;
            
            // Act
            var result = await _systemUnderTest.GetCourses(_visionUserSession);

            // Assert
            _visionClient.VerifyNoOtherCalls();
            result.Should().BeOfType<GetCoursesResult.Forbidden>();
        }

        [DataTestMethod]
        [DataRow(CoursesMaxCoursesLimit + 1, CoursesMaxCoursesLimit)]
        [DataRow(CoursesMaxCoursesLimit, CoursesMaxCoursesLimit)]
        [DataRow(CoursesMaxCoursesLimit - 1, CoursesMaxCoursesLimit - 1)]
        public async Task Get_PrescriptionsInResponseAreLimitedToMax_WhenSuccessfulResponseFromVision(
            int numberOfCoursesToCreate, int expectedNumberOfPrescriptions)
        {
            // Arrange
            var repeats = new List<Repeat>();
            for (int i = 0;
                i < numberOfCoursesToCreate;
                i++)
            {
                repeats.Add(new Repeat
                {
                    Id = i.ToString(CultureInfo.InvariantCulture),
                    Drug = "Drug " + i,
                    Quantity = _fixture.Create<string>(),
                });
            }

            _eligibleRepeatsResponse.Body.VisionResponse.ServiceContent.EligibleRepeats.Repeats = repeats;

            _visionClient.Setup(x => x.GetEligibleRepeats(_visionUserSession)).Returns(
                Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<EligibleRepeatsResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = _eligibleRepeatsResponse,
                    }));

            var response = new CourseListResponse();
            EligibleRepeats capturedItemToMap = null;
            _visionMapper.Setup(x => x.Map(It.IsAny<EligibleRepeats>())).Returns(response)
                .Callback<EligibleRepeats>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetCourses(_visionUserSession);

            // Assert
            _visionClient.Verify(x => x.GetEligibleRepeats(_visionUserSession));
            result.Should().BeAssignableTo<GetCoursesResult.Success>();
            ((GetCoursesResult.Success) result).Response.Should().NotBeNull();
            var getCourseResult = (GetCoursesResult.Success) result;
            getCourseResult.Response.Should().Be(response);
            capturedItemToMap.Repeats.Should().HaveCount(expectedNumberOfPrescriptions);
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavilable_WhenErrorReceivedFromVision()
        {
            // Arrange
            _visionClient.Setup(x => x.GetEligibleRepeats(_visionUserSession))
                .Returns(
                    Task.FromResult(
                        new VisionPFSClient.VisionApiObjectResponse<EligibleRepeatsResponse>
                            (HttpStatusCode.InternalServerError)
                            {
                                RawResponse = _fixture.Create<VisionResponseEnvelope<EligibleRepeatsResponse>>()
                            }));

            // Act
            var result = await _systemUnderTest.GetCourses(_visionUserSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.BadGateway>();
        }

        [TestMethod]
        public async Task Get_ReturnsBadGateway_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            _visionClient.Setup(x => x.GetEligibleRepeats(_visionUserSession))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetCourses(_visionUserSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.BadGateway>();
            _visionClient.Verify();
        }

        [TestMethod]
        public async Task Get_ReturnsBadData_ServiceReturns_InternalServerError()
        {
            _visionClient.Setup(x => x.GetEligibleRepeats(_visionUserSession))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<EligibleRepeatsResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = _eligibleRepeatsResponse,
                    }));


            _visionMapper.Setup(x =>
                    x.Map(It.IsAny<EligibleRepeats>()))
                .Throws<ArgumentNullException>()
                .Verifiable();


            // Act
            var result = await _systemUnderTest.GetCourses(_visionUserSession);

            // Assert
            _visionClient.Verify(x => x.GetEligibleRepeats(_visionUserSession));
            result.Should().BeAssignableTo<GetCoursesResult.InternalServerError>();
        }
    }
}