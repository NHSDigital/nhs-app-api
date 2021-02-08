using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Prescriptions
{
    [TestClass]
    public sealed class PrescriptionsControllerGetTests: IDisposable
    {
        private PrescriptionsController _systemUnderTest;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IPrescriptionValidationService> _mockPrescriptionValidationService;
        private Mock<IPrescriptionService> _mockPrescriptionsService;
        private Mock<IAuditor> _mockAuditor;
        private Mock<ILogger<PrescriptionsController>> _mockLogger;
        private Mock<IErrorReferenceGenerator> _mockErrorReferenceGenerator;
        private ConfigurationSettings _options;
        private EmisUserSession _gpSession;

        private P9UserSession _userSession;

        private const string CookieDomain = "CookieDomain";
        private const int PrescriptionsDefaultLastNumberMonthsToDisplay = 2;
        private const int DefaultSessionExpiryMinutes  = 10;
        private const int DefaultHttpTimeoutSeconds = 6;
        private const int MinimumAppAge = 16;
        private const int MinimumLinkageAge = 16;

        private string _serviceDeskReference;
        private Guid _patientId;

        private const string GetRequestAuditType = "RepeatPrescriptions_ViewHistory_Request";
        private const string GetResponseAuditType = "RepeatPrescriptions_ViewHistory_Response";

        private const string RequestAuditMessage = "Attempting to view prescriptions";
        private const int MockNumberOfCourses = 10;

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();

            _userSession = new P9UserSession("csrfToken", "nhsNumber", new CitizenIdUserSession(), new EmisUserSession(), "im1token");
            _mockPrescriptionsService = new Mock<IPrescriptionService>();
            _mockPrescriptionValidationService = new Mock<IPrescriptionValidationService>();

            _mockAuditor = new Mock<IAuditor>();
            _mockLogger = new Mock<ILogger<PrescriptionsController>>();
            _gpSession = new EmisUserSession();

            _options = new ConfigurationSettings(CookieDomain,
                PrescriptionsDefaultLastNumberMonthsToDisplay,
                DefaultHttpTimeoutSeconds,
                DefaultSessionExpiryMinutes,
                MinimumAppAge,
                MinimumLinkageAge);

            _mockGpSystem = new Mock<IGpSystem>();
            _mockGpSystem
                .Setup(x => x.GetPrescriptionService())
                .Returns(_mockPrescriptionsService.Object);
            _mockGpSystem
                .Setup(x => x.GetPrescriptionValidationService())
                .Returns(_mockPrescriptionValidationService.Object);

            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);

            _mockErrorReferenceGenerator = new Mock<IErrorReferenceGenerator>();
            _serviceDeskReference = "service desk ref";

            _systemUnderTest = new PrescriptionsController(
                _options,
                _mockLogger.Object,
                _mockGpSystemFactory.Object,
                _mockAuditor.Object,
                _mockErrorReferenceGenerator.Object,
                new Mock<IMetricLogger>().Object);
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Arrange
            var fromDate = DateTime.Now;
            var response = new PrescriptionListResponse();
            var filteringCounts = new FilteringCounts
            {
                ReceivedCount = 10,
                ReturnedCount = 5
            };
            _mockPrescriptionsService.Setup(x => x.GetPrescriptions(
                    It.Is<GpLinkedAccountModel>(d =>
                        d.GpUserSession == _gpSession && d.PatientId == _patientId), It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>()))
                .Returns(Task.FromResult((GetPrescriptionsResult) new GetPrescriptionsResult.Success(response, filteringCounts)));
            _mockPrescriptionValidationService.Setup(x =>
                x.IsGetValid(fromDate, It.IsAny<DateTimeOffset>())).Returns(true);

            // Act
            var result = await _systemUnderTest.Get(fromDate, _patientId, _gpSession);

            // Assert
            _mockGpSystem.VerifyAll();
            _mockPrescriptionsService.VerifyAll();
            _mockPrescriptionValidationService.VerifyAll();
            _mockGpSystemFactory.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>().Subject
                .Value.Should().BeEquivalentTo(response);

            _mockAuditor.Verify(x => x.PreOperationAudit(GetRequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.PostOperationAudit(GetResponseAuditType, "Prescriptions successfully retrieved. " +
                                                                   $"Total prescriptions before filtering: {filteringCounts.ReceivedCount}, " +
                                                                   $"Total prescriptions returned after filtering: {filteringCounts.ReturnedCount}"));
        }

        [TestMethod]
        public async Task Get_CallsServiceWithDateXMonthsAgoFromConfig_WhenFromDateNotValid()
        {
            // Arrange
            DateTimeOffset? fromDateGenerated = null;
            var response = new PrescriptionListResponse();
            var filteringCounts = new FilteringCounts
            {
                ReceivedCount = 10,
                ReturnedCount = 5
            };
            _mockPrescriptionsService.Setup(x => x.GetPrescriptions(
                It.Is<GpLinkedAccountModel>(d =>
                    d.GpUserSession == _gpSession && d.PatientId == _patientId), It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>()))
                .Returns(Task.FromResult((GetPrescriptionsResult) new GetPrescriptionsResult.Success(response, filteringCounts)))
                .Callback((GpLinkedAccountModel s, DateTimeOffset? fd, DateTimeOffset? td) => fromDateGenerated = fd);
            _mockPrescriptionValidationService.Setup(x =>
                x.IsGetValid(It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset>())).Returns(false);

            // Act
            var result = await _systemUnderTest.Get(null, _patientId, _gpSession);

            // Assert
            _mockGpSystem.VerifyAll();
            _mockPrescriptionsService.VerifyAll();
            _mockPrescriptionValidationService.VerifyAll();
            _mockGpSystemFactory.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(response);
            fromDateGenerated.HasValue.Should().BeTrue();

            var xMonthsAgo = DateTimeOffset.Now.AddMonths(-PrescriptionsDefaultLastNumberMonthsToDisplay);
            fromDateGenerated.Value.Date.Should().Be(xMonthsAgo.Date);

            _mockAuditor.Verify(x => x.PreOperationAudit(GetRequestAuditType, RequestAuditMessage ));
            _mockAuditor.Verify(x => x.PostOperationAudit(GetResponseAuditType, "Prescriptions successfully retrieved. " +
                                                                   $"Total prescriptions before filtering: {filteringCounts.ReceivedCount}, " +
                                                                   $"Total prescriptions returned after filtering: {filteringCounts.ReturnedCount}"));
        }

        [DataTestMethod]
        [DataRow(typeof(GetPrescriptionsResult.BadGateway), StatusCodes.Status502BadGateway,
            "Error retrieving prescriptions: Supplier Unavailable")]
        [DataRow(typeof(GetPrescriptionsResult.Forbidden), StatusCodes.Status403Forbidden,
            "Error retrieving prescriptions: Insufficient permissions")]
        [DataRow(typeof(GetPrescriptionsResult.InternalServerError), StatusCodes.Status500InternalServerError,
            "Error retrieving prescriptions: Internal Server Error")]
        [DataRow(typeof(GetPrescriptionsResult.BadRequest), StatusCodes.Status400BadRequest,
            "Error retrieving prescriptions: Bad Request")]
        public async Task Get_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode,
            string expectedAuditResponseMessageFormat)
        {
            // Arrange
            var fromDate = DateTime.Now;
            var serviceResult = (GetPrescriptionsResult) Activator.CreateInstance(serviceResultType);
            _mockPrescriptionsService.Setup(x => x.GetPrescriptions(
                    It.Is<GpLinkedAccountModel>(d =>
                        d.GpUserSession == _gpSession && d.PatientId == _patientId), fromDate, It.IsAny<DateTimeOffset?>()))
                .Returns(Task.FromResult(serviceResult));
            _mockPrescriptionValidationService
                .Setup(x => x.IsGetValid(fromDate, It.IsAny<DateTimeOffset>()))
                .Returns(true);
            _mockErrorReferenceGenerator.Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Prescriptions,
                    expectedStatusCode, _gpSession.Supplier))
                .Returns(_serviceDeskReference);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.Get(fromDate, _patientId, _gpSession);

            // Assert
            _mockPrescriptionsService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(expectedStatusCode);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockAuditor.Verify(x => x.PreOperationAudit(GetRequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.PostOperationAudit(GetResponseAuditType, expectedAuditResponseMessageFormat));
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_LogsCoursesCountWithMaximumAllowanceDiscarded()
        {
            // Arrange
            var fromDate = DateTime.Now;
            var response = new PrescriptionListResponse();
            _mockPrescriptionsService.Setup(x => x.GetPrescriptions(
                    It.Is<GpLinkedAccountModel>(d =>
                        d.GpUserSession == _gpSession && d.PatientId == _patientId), It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>()))
                .Returns(Task.FromResult((GetPrescriptionsResult) new GetPrescriptionsResult.Success(response, new FilteringCounts
                {
                    ReceivedCount = MockNumberOfCourses,
                    ReceivedRepeatsCount = MockNumberOfCourses,
                    ExcessRepeatsCount = MockNumberOfCourses,
                    ReturnedCount = MockNumberOfCourses
                })));
            _mockPrescriptionValidationService.Setup(x =>
                x.IsGetValid(fromDate, It.IsAny<DateTimeOffset>())).Returns(true);

            // Act
            await _systemUnderTest.Get(fromDate, _patientId, _gpSession);

            // Assert
            var expectedLogMessage =
                $"Prescription Count: Prescriptions Received={MockNumberOfCourses} " +
                $"Prescriptions remaining after filtering out non-repeats={MockNumberOfCourses} " +
                $"Prescriptions filtered out for exceeding maximum allowance={MockNumberOfCourses} " +
                $"Prescriptions Returned to user={MockNumberOfCourses}";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}
