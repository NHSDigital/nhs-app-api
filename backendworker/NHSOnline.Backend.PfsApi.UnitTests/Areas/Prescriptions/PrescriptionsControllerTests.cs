using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Prescriptions
{
    [TestClass]
    public class PrescriptionsControllerTests
    {
        private PrescriptionsController _systemUnderTest;
        private IFixture _fixture;
        private IOptions<ConfigurationSettings> _options;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IPrescriptionRequestValidationService> _prescriptionRequestValidationService;
        private UserSession _userSession;

        private Mock<IAuditor> _mockAuditor;
        
        private int _prescriptionsDefaultLastNumberMonthsToDisplay;

        private const string GetRequestAuditType = "RepeatPrescriptions_ViewHistory_Request";
        private const string GetResponseAuditType = "RepeatPrescriptions_ViewHistory_Response";         
        
        private const string PostRequestAuditType = "RepeatPrescriptions_OrderRepeatMedications_Request";
        private const string PostResponseAuditType = "RepeatPrescriptions_OrderRepeatMedications_Response";
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

            _userSession = _fixture.Create<UserSession>();

            _prescriptionsDefaultLastNumberMonthsToDisplay = _fixture.Create<int>();

            _options = Options.Create(new ConfigurationSettings
            {
                PrescriptionsDefaultLastNumberMonthsToDisplay = _prescriptionsDefaultLastNumberMonthsToDisplay
            });

            _fixture.Inject(_options);
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _prescriptionRequestValidationService = _fixture.Freeze<Mock<IPrescriptionRequestValidationService>>();

            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();
            
            _systemUnderTest = _fixture.Create<PrescriptionsController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var date = DateTime.Now;
            var mockGpSystem = new Mock<IGpSystem>();
            var prescriptionService = new Mock<IPrescriptionService>();

            var prescriptionRequestsGetResponse = new PrescriptionListResponse
            {
                Prescriptions = new List<PrescriptionItem>
                {
                    new PrescriptionItem
                    {
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry(),
                            new CourseEntry(),
                        },
                    },
                    new PrescriptionItem
                    {
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry(),
                        },
                    },
                },
            };

            var getPrescriptionsResult = new PrescriptionResult.SuccessfulGet(prescriptionRequestsGetResponse);

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionRequestValidationService())
                .Returns(_prescriptionRequestValidationService.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            prescriptionService.Setup(x => x.GetPrescriptions(_userSession.GpUserSession, date, It.IsAny<DateTimeOffset>())).Returns(Task.FromResult((PrescriptionResult)getPrescriptionsResult));

            _prescriptionRequestValidationService
                .Setup(x => x.IsValidFromDate(date, It.IsAny<DateTimeOffset>()))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Get(date);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            mockGpSystem.Verify(x => x.GetPrescriptionService());
            prescriptionService.Verify(x => x.GetPrescriptions(_userSession.GpUserSession, date, It.IsAny<DateTimeOffset>()));
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(prescriptionRequestsGetResponse);

            _mockAuditor.Verify(x => x.Audit(GetRequestAuditType, "Attempting to view prescriptions", It.IsAny<object[]>()));
            _mockAuditor.Verify(x => x.PostAudit(GetResponseAuditType, "Prescriptions successfully retrieved - 3 courses", It.IsAny<object[]>()));
        }

        [TestMethod]
        public async Task Get_CallsServiceWithDateXMonthsAgoFromConfig_WhenFromDateNotValid()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var prescriptionService = new Mock<IPrescriptionService>();

            var prescriptionRequestsGetResponse = new PrescriptionListResponse
            {
                Prescriptions = new List<PrescriptionItem>
                {
                    new PrescriptionItem
                    {
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry(),
                        },
                    },
                    new PrescriptionItem
                    {
                        Courses = new List<CourseEntry>
                        {
                            new CourseEntry(),
                            new CourseEntry(),
                        },
                    },
                },
            };

            var getPrescriptionsResult = new PrescriptionResult.SuccessfulGet(prescriptionRequestsGetResponse);

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionRequestValidationService())
                .Returns(_prescriptionRequestValidationService.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            DateTimeOffset? fromDateGenerated = null;
            prescriptionService.Setup(x => x.GetPrescriptions(_userSession.GpUserSession, It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>()))
                .Returns(Task.FromResult((PrescriptionResult)getPrescriptionsResult))
                .Callback((GpUserSession s, DateTimeOffset? fd, DateTimeOffset? td) => fromDateGenerated = fd);

            _prescriptionRequestValidationService
                .Setup(x => x.IsValidFromDate(It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset>()))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.Get(null);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            mockGpSystem.Verify(x => x.GetPrescriptionService());
            prescriptionService.Verify(x => x.GetPrescriptions(_userSession.GpUserSession, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()));
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(prescriptionRequestsGetResponse);
            Assert.IsNotNull(value);
            Assert.IsTrue(fromDateGenerated.HasValue);

            var xMonthsAgo = DateTimeOffset.Now.AddMonths(-_prescriptionsDefaultLastNumberMonthsToDisplay);
            Assert.AreEqual(xMonthsAgo.Date, fromDateGenerated.Value.Date);
            
            _mockAuditor.Verify(x => x.Audit(GetRequestAuditType, "Attempting to view prescriptions", It.IsAny<object[]>()));
            _mockAuditor.Verify(x => x.PostAudit(GetResponseAuditType, "Prescriptions successfully retrieved - 3 courses", It.IsAny<object[]>()));
        }
        
        [TestMethod]
        public async Task Post_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var courseId = Guid.NewGuid().ToString();

            var requestModel = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string>()
                {
                    courseId
                }
            };

            var mockGpSystem = new Mock<IGpSystem>();
            var prescriptionService = new Mock<IPrescriptionService>();

            var postPrescriptionResult = new PrescriptionResult.SuccessfulPost();

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionRequestValidationService())
                .Returns(_prescriptionRequestValidationService.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            prescriptionService.Setup(x => x.OrderPrescription(_userSession.GpUserSession, It.IsAny<RepeatPrescriptionRequest>()))
                .Returns(Task.FromResult((PrescriptionResult)postPrescriptionResult));
            
            _prescriptionRequestValidationService
                .Setup(x => x.IsValidRepeatPrescriptionRequest(It.IsAny<RepeatPrescriptionRequest>()))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Post(requestModel);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            mockGpSystem.Verify(x => x.GetPrescriptionService());
            prescriptionService.Verify(x => x.OrderPrescription(_userSession.GpUserSession, It.IsAny<RepeatPrescriptionRequest>()));
            result.Should().BeAssignableTo<CreatedResult>();
            
            _mockAuditor.Verify(x => x.Audit(PostRequestAuditType, "Attempting to create a prescription request with course ids: {0}", courseId));
            _mockAuditor.Verify(x => x.Audit(PostResponseAuditType, "Repeat prescription request successfully created with course ids: {0}", courseId));
        }
        
        [TestMethod]
        public async Task Post_ReturnsBadRequest_WhenModelValidationFails()
        {
            var firstGuid = Guid.NewGuid().ToString();
            var secondGuid = Guid.NewGuid().ToString();
            
            var requestModel = new RepeatPrescriptionRequest()
            {
                CourseIds = new List<string>
                {
                    firstGuid,
                    secondGuid
                }
            };
            
            var mockGpSystem = new Mock<IGpSystem>();
            var prescriptionService = new Mock<IPrescriptionService>();

            var postPrescriptionResult = new PrescriptionResult.SuccessfulPost();

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionRequestValidationService())
                .Returns(_prescriptionRequestValidationService.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            prescriptionService.Setup(x => x.OrderPrescription(_userSession.GpUserSession, It.IsAny<RepeatPrescriptionRequest>())).Returns(Task.FromResult((PrescriptionResult)postPrescriptionResult));
            
            _prescriptionRequestValidationService
                .Setup(x => x.IsValidRepeatPrescriptionRequest(It.IsAny<RepeatPrescriptionRequest>()))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.Post(requestModel);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            
            _mockAuditor.Verify(x => x.Audit(PostRequestAuditType, "Attempting to create a prescription request with course ids: {0}", $"{firstGuid},{secondGuid}"));
            _mockAuditor.Verify(x => x.Audit(PostResponseAuditType, "Error creating prescription request: Bad Request with course ids: {0}", $"{firstGuid},{secondGuid}"));
        }
    }
}