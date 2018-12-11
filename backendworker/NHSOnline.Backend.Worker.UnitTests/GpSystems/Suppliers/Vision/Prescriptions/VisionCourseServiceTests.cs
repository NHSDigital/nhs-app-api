using System;
using System.Collections.Generic;
using System.Globalization;
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
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Courses;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Prescriptions
{
    [TestClass]
    public class VisionCourseServiceTests
    {
        private VisionCourseService _systemUnderTest;
        private Mock<IVisionClient> _visionClient;
        private Mock<IVisionPrescriptionMapper> _visionMapper;
        private IOptions<ConfigurationSettings> _options;
        private Mock<ISessionCacheService> _sessionCacheService;
        private VisionUserSession _userSession;
        private IFixture _fixture;
        private VisionResponseEnvelope<EligibleRepeatsResponse> _eligibleRepeatsResponse;

        private const int CoursesMaxCoursesLimit = 100;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _visionClient = _fixture.Freeze<Mock<IVisionClient>>();
            _userSession = _fixture.Freeze<VisionUserSession>();
            _userSession.IsRepeatPrescriptionsEnabled = true;
            _visionMapper = _fixture.Freeze<Mock<IVisionPrescriptionMapper>>();
            _sessionCacheService = _fixture.Freeze<Mock<ISessionCacheService>>();
            _options = Options.Create(new ConfigurationSettings
            {
                CoursesMaxCoursesLimit = CoursesMaxCoursesLimit
            });
            _fixture.Inject(_options);
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
                                Repeats = new List<Repeat>
                                {
                                    _fixture.Create<Repeat>(),
                                    _fixture.Create<Repeat>(),
                                },
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
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromVision()
        {
            _visionClient.Setup(x => x.GetEligibleRepeats(_userSession))
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
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _visionClient.Verify(x => x.GetEligibleRepeats(_userSession));
            result.Should().BeAssignableTo<GetCoursesResult.SuccessfullyRetrieved>();
            ((GetCoursesResult.SuccessfullyRetrieved) result).Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Get_UpdatesSessionWithCurrentValueForEligibleRepeatsAllowFreeTextValue_WhenSuccessfulResponseFromVision()
        {
            _userSession.AllowFreeTextPrescriptions = false;
            _eligibleRepeatsResponse.Body.VisionResponse.ServiceContent.EligibleRepeats.Settings.AllowFreeText = true;

            _visionClient.Setup(x => x.GetEligibleRepeats(_userSession))
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
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _visionClient.Verify(x => x.GetEligibleRepeats(_userSession));
            Assert.IsTrue(_userSession.AllowFreeTextPrescriptions); // should be updated
            _sessionCacheService.Verify(x => x.UpdateUserSession(It.Is<VisionUserSession>(vus => vus.AllowFreeTextPrescriptions)));
            _sessionCacheService.Verify(x => x.UpdateUserSession(_userSession));
            result.Should().BeAssignableTo<GetCoursesResult.SuccessfullyRetrieved>();

            ((GetCoursesResult.SuccessfullyRetrieved)result).Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetCourses_ReturnsSupplierNotEnabled_WhenRepeatPrescriptionsIsDisabledInUserSession()
        {
            // Arrange
            _userSession.IsRepeatPrescriptionsEnabled = false;
            
            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _visionClient.VerifyNoOtherCalls();
            result.Should().BeOfType<GetCoursesResult.SupplierNotEnabled>();
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

            _visionClient.Setup(x => x.GetEligibleRepeats(_userSession)).Returns(
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
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _visionClient.Verify(x => x.GetEligibleRepeats(_userSession));
            result.Should().BeAssignableTo<GetCoursesResult.SuccessfullyRetrieved>();
            ((GetCoursesResult.SuccessfullyRetrieved) result).Response.Should().NotBeNull();
            var getCourseResult = (GetCoursesResult.SuccessfullyRetrieved) result;
            getCourseResult.Response.Should().Be(response);
            capturedItemToMap.Repeats.Should().HaveCount(expectedNumberOfPrescriptions);
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavilable_WhenErrorReceivedFromVision()
        {
            // Arrange
            _visionClient.Setup(x => x.GetEligibleRepeats(_userSession))
                .Returns(
                    Task.FromResult(
                        new VisionPFSClient.VisionApiObjectResponse<EligibleRepeatsResponse>
                            (HttpStatusCode.InternalServerError)
                            {
                                RawResponse = _fixture.Create<VisionResponseEnvelope<EligibleRepeatsResponse>>()
                            }));

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavailable_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            _visionClient.Setup(x => x.GetEligibleRepeats(_userSession))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.SupplierSystemUnavailable>();
            _visionClient.Verify();
        }

        [TestMethod]
        public async Task Get_ReturnsBadData_ServiceReturns_InternalServerError()
        {
            _visionClient.Setup(x => x.GetEligibleRepeats(_userSession))
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
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _visionClient.Verify(x => x.GetEligibleRepeats(_userSession));
            result.Should().BeAssignableTo<GetCoursesResult.InternalServerError>();
        }
    }
}