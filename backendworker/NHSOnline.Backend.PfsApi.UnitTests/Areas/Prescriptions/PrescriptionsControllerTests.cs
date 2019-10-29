using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support.Settings;
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
        private ConfigurationSettings _options;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IPrescriptionValidationService> _prescriptionRequestValidationService;
        private UserSession _userSession;
        private Guid _patientId;

        private Mock<IAuditor> _mockAuditor;

        private const string GetRequestAuditType = "RepeatPrescriptions_ViewHistory_Request";
        private const string GetResponseAuditType = "RepeatPrescriptions_ViewHistory_Response";

        private const string CookieDomain = "CookieDomain";
        private int PrescriptionsDefaultLastNumberMonthsToDisplay;   
        private const int DefaultSessionExpiryMinutes  = 10;
        private const int DefaultHttpTimeoutSeconds = 6;
        private const int MinimumAppAge = 16;
        private const int MinimumLinkageAge = 16;

        private DateTimeOffset? CurrentTermsConditionsEffectiveDate = DateTimeOffset.Now;
        
        private const string PostRequestAuditType = "RepeatPrescriptions_OrderRepeatMedications_Request";
        private const string PostResponseAuditType = "RepeatPrescriptions_OrderRepeatMedications_Response";
        
        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

            _userSession = _fixture.Create<UserSession>();

            PrescriptionsDefaultLastNumberMonthsToDisplay  = _fixture.Create<int>();

            _options = new ConfigurationSettings(CookieDomain, PrescriptionsDefaultLastNumberMonthsToDisplay, DefaultHttpTimeoutSeconds, DefaultSessionExpiryMinutes, 
            MinimumAppAge, MinimumLinkageAge, CurrentTermsConditionsEffectiveDate);

            _fixture.Inject(_options);
            
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _prescriptionRequestValidationService = _fixture.Freeze<Mock<IPrescriptionValidationService>>();

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

            var getPrescriptionsResult = new GetPrescriptionsResult.Success(prescriptionRequestsGetResponse);

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionRequestValidationService())
                .Returns(_prescriptionRequestValidationService.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            prescriptionService.Setup(x => x.GetPrescriptions(
                It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId), 
                date, It.IsAny<DateTimeOffset>())).Returns(Task.FromResult((GetPrescriptionsResult)getPrescriptionsResult));

            _prescriptionRequestValidationService
                .Setup(x => x.IsGetValid(date, It.IsAny<DateTimeOffset>()))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Get(date, _patientId);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            mockGpSystem.Verify(x => x.GetPrescriptionService());
            prescriptionService.Verify(x => x.GetPrescriptions(
                It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId), date, It.IsAny<DateTimeOffset>()));
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(prescriptionRequestsGetResponse);

            _mockAuditor.Verify(x => x.Audit(GetRequestAuditType, "Attempting to view prescriptions", It.IsAny<object[]>()));
            _mockAuditor.Verify(x => x.Audit(GetResponseAuditType, "Prescriptions successfully retrieved - 3 courses", It.IsAny<object[]>()));
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

            var getPrescriptionsResult = new GetPrescriptionsResult.Success(prescriptionRequestsGetResponse);

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionRequestValidationService())
                .Returns(_prescriptionRequestValidationService.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            DateTimeOffset? fromDateGenerated = null;
            prescriptionService.Setup(x => x.GetPrescriptions(
                    It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId),
                    It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>()))
                .Returns(Task.FromResult((GetPrescriptionsResult)getPrescriptionsResult))
                .Callback((GpLinkedAccountModel s, DateTimeOffset? fd, DateTimeOffset? td) => fromDateGenerated = fd);

            _prescriptionRequestValidationService
                .Setup(x => x.IsGetValid(It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset>()))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.Get(null, _patientId);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            mockGpSystem.Verify(x => x.GetPrescriptionService());
            prescriptionService.Verify(x => x.GetPrescriptions(
                It.Is<GpLinkedAccountModel>(
                d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId),
                It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()));
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(prescriptionRequestsGetResponse);
            fromDateGenerated.HasValue.Should().BeTrue();

            var xMonthsAgo = DateTimeOffset.Now.AddMonths(-PrescriptionsDefaultLastNumberMonthsToDisplay);
            fromDateGenerated.Value.Date.Should().Be(xMonthsAgo.Date);
            
            _mockAuditor.Verify(x => x.Audit(GetRequestAuditType, "Attempting to view prescriptions", It.IsAny<object[]>()));
            _mockAuditor.Verify(x => x.Audit(GetResponseAuditType, "Prescriptions successfully retrieved - 3 courses", It.IsAny<object[]>()));
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

            var postPrescriptionResult = new OrderPrescriptionResult.Success();

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionRequestValidationService())
                .Returns(_prescriptionRequestValidationService.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);
            
            prescriptionService.Setup(x => x.OrderPrescription(
                It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId), It.IsAny<RepeatPrescriptionRequest>())).Returns(Task.FromResult((OrderPrescriptionResult)postPrescriptionResult));
            
            _prescriptionRequestValidationService
                .Setup(x => x.IsPostValid(It.IsAny<RepeatPrescriptionRequest>()))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Post(requestModel, _patientId);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            mockGpSystem.Verify(x => x.GetPrescriptionService());
            
            prescriptionService.Verify(x => x.OrderPrescription(It.Is<GpLinkedAccountModel>(
                d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId), It.IsAny<RepeatPrescriptionRequest>()));
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

            var postPrescriptionResult = new OrderPrescriptionResult.Success();

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionRequestValidationService())
                .Returns(_prescriptionRequestValidationService.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            prescriptionService.Setup(x => x.OrderPrescription(It.Is<GpLinkedAccountModel>(
                d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId), It.IsAny<RepeatPrescriptionRequest>())).Returns(Task.FromResult((OrderPrescriptionResult)postPrescriptionResult));
            
            _prescriptionRequestValidationService
                .Setup(x => x.IsPostValid(It.IsAny<RepeatPrescriptionRequest>()))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.Post(requestModel, _patientId);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            
            _mockAuditor.Verify(x => x.Audit(PostRequestAuditType, "Attempting to create a prescription request with course ids: {0}", $"{firstGuid},{secondGuid}"));
            _mockAuditor.Verify(x => x.Audit(PostResponseAuditType, "Error creating prescription request: Bad Request with course ids: {0}", $"{firstGuid},{secondGuid}"));
        }
    }
}