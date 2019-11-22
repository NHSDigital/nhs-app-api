using System;
using System.Collections.Generic;
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
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions;
using NHSOnline.Backend.Support;
using TppUserSession = NHSOnline.Backend.GpSystems.Suppliers.Tpp.TppUserSession;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Prescriptions
{
    [TestClass]
    public class TppCourseServiceTests
    {
        private TppCourseService _systemUnderTest;
        private Mock<ITppClient> _tppClient;
        private Mock<ITppCourseMapper> _tppCourseMapper;
        private TppConfigurationSettings _settings;
        private TppUserSession _tppUserSession;
        private IFixture _fixture;
        private Guid _patientId;
        private const string ApplicationName = "appName";
        private const string ApplicationVersion = "13";
        private const string ApplicationProviderId = "providerId";
        private const string ApplicationDeviceType = "deviceType";
        private static readonly Uri ApiUrl = new Uri("http://tppapitest:60015/Test/");
        private const string ApiVersion = "12";
        private const int PrescriptionsMaxCoursesSoftLimit = 100;
        private const int CoursesMaxCoursesLimit = 100;
        private const string CertificatePath = "CertificatePath";
        private const string CertificatePassphrase = "CerticiatePassphrase";
        private const string Environment = "testEnv";

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();
            
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _tppUserSession = _fixture.Create<TppUserSession>();

            _tppClient = _fixture.Freeze<Mock<ITppClient>>();
            
            _tppCourseMapper = _fixture.Freeze<Mock<ITppCourseMapper>>();
            
            _settings = new TppConfigurationSettings(ApiUrl, ApiVersion, ApplicationName, ApplicationVersion, ApplicationProviderId, ApplicationDeviceType, 
                CertificatePath, CertificatePassphrase, PrescriptionsMaxCoursesSoftLimit, CoursesMaxCoursesLimit, Environment);

            _fixture.Inject(_settings);
            _systemUnderTest = _fixture.Create<TppCourseService>();
            _fixture.Customize<Medication>(c => c.With(s => s.Requestable, "y"));
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessResponseForHappyPath_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            _fixture.Customize<Medication>(c => c.With(s => s.Requestable, "y"));

            var listRepeatMedicationReply = _fixture.Create<ListRepeatMedicationReply>();

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_tppUserSession))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = listRepeatMedicationReply,
                    }));

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_tppUserSession, _patientId));

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_tppUserSession));
            result.Should().BeAssignableTo<GetCoursesResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessResponseForHappyPath_CanBeRequested_Capital_Y()
        {
            // Arrange
            _fixture.Customize<Medication>(c => c.With(s => s.Requestable, "Y"));

            var listRepeatMedicationReply = _fixture.Create<ListRepeatMedicationReply>();

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_tppUserSession))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = listRepeatMedicationReply,
                    }));

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_tppUserSession, _patientId));

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_tppUserSession));
            result.Should().BeAssignableTo<GetCoursesResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessResponseForHappyPath_WhenSuccessfulResponseFromTpp_NullRequestable()
        {
            // Arrange
            _fixture.Customize<Medication>(c => c.With(s => s.Requestable, () => null));

            var listRepeatMedicationReply = _fixture.Create<ListRepeatMedicationReply>();

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_tppUserSession))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = listRepeatMedicationReply,
                    }));

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_tppUserSession, _patientId));

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_tppUserSession));
            result.Should().BeAssignableTo<GetCoursesResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }

        [DataTestMethod]
        [DataRow(CoursesMaxCoursesLimit + 1, CoursesMaxCoursesLimit)]
        [DataRow(CoursesMaxCoursesLimit, CoursesMaxCoursesLimit)]
        [DataRow(CoursesMaxCoursesLimit - 1, CoursesMaxCoursesLimit - 1)]
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

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_tppUserSession)).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = listRepeatMedicationReply,
                    }));

            var response = new CourseListResponse();
            List<Medication> capturedItemToMap = null;
            _tppCourseMapper.Setup(x => x.Map(It.IsAny<List<Medication>>())).Returns(response)
                .Callback<List<Medication>>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_tppUserSession, _patientId));

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_tppUserSession));
            result.Should().BeAssignableTo<GetCoursesResult.Success>()
                .Subject.Response.Should().Be(response);

            capturedItemToMap.Should().HaveCount(expectedNumberOfPrescriptions);
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavailable_WhenErrorReceivedFromTpp()
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
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_tppUserSession, _patientId));

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.BadGateway>();
        }

        [TestMethod]
        public async Task Get_ReturnsBadGateway_WhenHttpExceptionOccursCallingTpp()
        {
            // Arrange
            _tppClient.Setup(x => x.ListRepeatMedicationPost(_tppUserSession))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_tppUserSession, _patientId));

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.BadGateway>();
            _tppClient.Verify();
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessResponse_TppResponseContainsNullableRequestableProperty()
        {
            // Arrange
            const int expectedNumberOfPrescriptions = 1;
            _fixture.Customize<Medication>(c => c.With(s => s.Requestable, "y"));

            var listRepeatMedicationReply = new ListRepeatMedicationReply
            {
                Medications = new List<Medication>
                {
                    new Medication
                    {
                        DrugId = Guid.NewGuid().ToString(),
                        Drug = "Drug",
                        Details = _fixture.Create<string>(),
                        Type = "Repeat",
                    },
                    new Medication
                    {
                        DrugId = Guid.NewGuid().ToString(),
                        Drug = "Drug",
                        Details = _fixture.Create<string>(),
                        Requestable = "y",
                        Type = "Repeat",
                    }
                },
                OnlineUserId = _fixture.Create<string>(),
                PatientId = _fixture.Create<string>(),
                Uuid = new Guid()
            };

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_tppUserSession))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = listRepeatMedicationReply,
                    }));
            
            var response = new CourseListResponse();
            List<Medication> capturedItemToMap = null;
            
            _tppCourseMapper.Setup(x => x.Map(It.IsAny<List<Medication>>())).Returns(response)
                .Callback<List<Medication>>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_tppUserSession, _patientId));

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_tppUserSession));
            result.Should().BeAssignableTo<GetCoursesResult.Success>()
                .Subject.Response.Should().NotBeNull();
            capturedItemToMap.Count.Should().Be(expectedNumberOfPrescriptions);
        }
        
        [TestMethod]
        public async Task Get_ReturnsForbidden_WhenErrorReceivedNoAccessFromTpp()
        {
            // Arrange
            var expectedError = _fixture.Build<Error>()
                .With(x => x.ErrorCode, TppApiErrorCodes.NoAccess)
                .Create();
            
            _tppClient.Setup(x => x.ListRepeatMedicationPost(_tppUserSession))
                .Returns(
                    Task.FromResult(
                        new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>
                            (HttpStatusCode.InternalServerError)
                            {
                                ErrorResponse = expectedError
                            }));
            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_tppUserSession, _patientId));

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.Forbidden>();
        }
    }
}