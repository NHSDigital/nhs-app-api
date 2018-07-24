using System;
using System.Collections.Generic;
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
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions;
using TppUserSession = NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.TppUserSession;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Prescriptions
{
    [TestClass]
    public class TppCourseServiceTests
    {
        private TppCourseService _systemUnderTest;
        private Mock<ITppClient> _tppClient;
        private Mock<ITppCourseMapper> _tppCourseMapper;
        private IOptions<ConfigurationSettings> _options;
        private TppUserSession _userSession;
        private IFixture _fixture;

        private const int CoursesMaxCoursesLimit = 100;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _tppClient = _fixture.Freeze<Mock<ITppClient>>();
            _userSession = _fixture.Freeze<TppUserSession>();
            _tppCourseMapper = _fixture.Freeze<Mock<ITppCourseMapper>>();
            _options = Options.Create(new ConfigurationSettings
            {
                CoursesMaxCoursesLimit = CoursesMaxCoursesLimit
            });
            _fixture.Inject(_options);
            _systemUnderTest = _fixture.Create<TppCourseService>();
            _fixture.Customize<Medication>(c => c.With(s => s.Requestable, "y"));
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            _fixture.Customize<Medication>(c => c.With(s => s.Requestable, "y"));

            var listRepeatMedicationReply = _fixture.Create<ListRepeatMedicationReply>();

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_userSession))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = listRepeatMedicationReply,
                    }));

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_userSession));
            result.Should().BeAssignableTo<GetCoursesResult.SuccessfullyRetrieved>();
            ((GetCoursesResult.SuccessfullyRetrieved) result).Response.Should().NotBeNull();
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_CanBeRequested_Captial_Y()
        {
            // Arrange
            _fixture.Customize<Medication>(c => c.With(s => s.Requestable, "Y"));

            var listRepeatMedicationReply = _fixture.Create<ListRepeatMedicationReply>();

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_userSession))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = listRepeatMedicationReply,
                    }));

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_userSession));
            result.Should().BeAssignableTo<GetCoursesResult.SuccessfullyRetrieved>();
            ((GetCoursesResult.SuccessfullyRetrieved) result).Response.Should().NotBeNull();
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromTpp_NullRequestable()
        {
            // Arrange
            _fixture.Customize<Medication>(c => c.With(s => s.Requestable, null));

            var listRepeatMedicationReply = _fixture.Create<ListRepeatMedicationReply>();

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_userSession))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = listRepeatMedicationReply,
                    }));

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_userSession));
            result.Should().BeAssignableTo<GetCoursesResult.SuccessfullyRetrieved>();
            ((GetCoursesResult.SuccessfullyRetrieved) result).Response.Should().NotBeNull();
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

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_userSession)).Returns(
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
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_userSession));
            result.Should().BeAssignableTo<GetCoursesResult.SuccessfullyRetrieved>();
            ((GetCoursesResult.SuccessfullyRetrieved) result).Response.Should().NotBeNull();

            var getCourseResult = (GetCoursesResult.SuccessfullyRetrieved) result;
            getCourseResult.Response.Should().Be(response);

            capturedItemToMap.Should().HaveCount(expectedNumberOfPrescriptions);
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavilable_WhenErrorReceivedFromEmis()
        {
            // Arrange
            _tppClient.Setup(x => x.ListRepeatMedicationPost(_userSession))
                .Returns(
                    Task.FromResult(
                        new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>
                            (HttpStatusCode.InternalServerError)
                            {
                                ErrorResponse = _fixture.Create<Error>()
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
            _tppClient.Setup(x => x.ListRepeatMedicationPost(_userSession))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.SupplierSystemUnavailable>();
            _tppClient.Verify();
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponse_TppResponseContainsNullableRequestableProperty()
        {
            // Arrange
            const int ExpectedNumberOfPrescriptions = 1;
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

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_userSession))
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
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_userSession));
            result.Should().BeAssignableTo<GetCoursesResult.SuccessfullyRetrieved>();
            ((GetCoursesResult.SuccessfullyRetrieved) result).Response.Should().NotBeNull();
            Assert.AreEqual(ExpectedNumberOfPrescriptions, capturedItemToMap.Count);
        }
        
        [TestMethod]
        public async Task Get_ReturnsForbidden_WhenErrorReceivedNoAccessFromTpp()
        {
            var expectedError = _fixture.Create<Error>();

            // Tpp forbidden error code 
            expectedError.ErrorCode = TppApiErrorCodes.NoAccess;
            
            // Arrange
            _tppClient.Setup(x => x.ListRepeatMedicationPost(_userSession))
                .Returns(
                    Task.FromResult(
                        new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>
                            (HttpStatusCode.InternalServerError)
                            {
                                ErrorResponse = expectedError
                            }));
            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.SupplierNotEnabled>();
        }
    }
}