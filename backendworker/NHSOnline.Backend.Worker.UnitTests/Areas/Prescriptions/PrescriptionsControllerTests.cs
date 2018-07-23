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
using NHSOnline.Backend.Worker.Areas.Prescriptions;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Prescriptions
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

        private int _prescriptionsDefaultLastNumberMonthsToDisplay;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _prescriptionsDefaultLastNumberMonthsToDisplay = _fixture.Create<int>();

            _options = Options.Create(new ConfigurationSettings
            {
                PrescriptionsDefaultLastNumberMonthsToDisplay = _prescriptionsDefaultLastNumberMonthsToDisplay
            });

            _fixture.Inject(_options);
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _prescriptionRequestValidationService = _fixture.Freeze<Mock<IPrescriptionRequestValidationService>>();

            _userSession = _fixture.Create<UserSession>();
            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);

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

            var prescriptionRequestsGetResponse = new PrescriptionListResponse();

            var getPrescriptionsResult = new PrescriptionResult.SuccessfulGet(prescriptionRequestsGetResponse);

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionRequestValidationService())
                .Returns(_prescriptionRequestValidationService.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            prescriptionService.Setup(x => x.GetPrescriptions(_userSession, date, It.IsAny<DateTimeOffset>())).Returns(Task.FromResult((PrescriptionResult)getPrescriptionsResult));

            _prescriptionRequestValidationService
                .Setup(x => x.IsValidFromDate(date, It.IsAny<DateTimeOffset>()))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Get(date);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.Supplier));
            mockGpSystem.Verify(x => x.GetPrescriptionService());
            prescriptionService.Verify(x => x.GetPrescriptions(_userSession, date, It.IsAny<DateTimeOffset>()));
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(prescriptionRequestsGetResponse);
        }

        [TestMethod]
        public async Task Get_CallsServiceWithDateXMonthsAgoFromConfig_WhenFromDateNotValid()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var prescriptionService = new Mock<IPrescriptionService>();

            var prescriptionRequestsGetResponse = new PrescriptionListResponse();

            var getPrescriptionsResult = new PrescriptionResult.SuccessfulGet(prescriptionRequestsGetResponse);

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionRequestValidationService())
                .Returns(_prescriptionRequestValidationService.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            DateTimeOffset? fromDateGenerated = null;
            prescriptionService.Setup(x => x.GetPrescriptions(_userSession, It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>()))
                .Returns(Task.FromResult((PrescriptionResult)getPrescriptionsResult))
                .Callback((UserSession s, DateTimeOffset? fd, DateTimeOffset? td) => fromDateGenerated = fd);

            _prescriptionRequestValidationService
                .Setup(x => x.IsValidFromDate(It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset>()))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.Get(null);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.Supplier));
            mockGpSystem.Verify(x => x.GetPrescriptionService());
            prescriptionService.Verify(x => x.GetPrescriptions(_userSession, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()));
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(prescriptionRequestsGetResponse);
            Assert.IsNotNull(value);
            Assert.IsTrue(fromDateGenerated.HasValue);

            var xMonthsAgo = DateTimeOffset.Now.AddMonths(-_prescriptionsDefaultLastNumberMonthsToDisplay);
            Assert.AreEqual(xMonthsAgo.Date, fromDateGenerated.Value.Date);
        }
        
        [TestMethod]
        public async Task Post_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var requestModel = new RepeatPrescriptionRequest();
            var mockGpSystem = new Mock<IGpSystem>();
            var prescriptionService = new Mock<IPrescriptionService>();

            var postPrescriptionResult = new PrescriptionResult.SuccessfulPost();

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionRequestValidationService())
                .Returns(_prescriptionRequestValidationService.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            prescriptionService.Setup(x => x.OrderPrescription(_userSession, It.IsAny<RepeatPrescriptionRequest>()))
                .Returns(Task.FromResult((PrescriptionResult)postPrescriptionResult));
            
            _prescriptionRequestValidationService
                .Setup(x => x.IsValidRepeatPrescriptionRequest(It.IsAny<RepeatPrescriptionRequest>()))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Post(requestModel);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.Supplier));
            mockGpSystem.Verify(x => x.GetPrescriptionService());
            prescriptionService.Verify(x => x.OrderPrescription(_userSession, It.IsAny<RepeatPrescriptionRequest>()));
            var createdresult = result as CreatedResult;
            Assert.IsTrue(createdresult.StatusCode == 201);
        }
        
        [TestMethod]
        public async Task Post_ReturnsBadRequest_WhenModelValidationFails()
        {
            var requestModel = new RepeatPrescriptionRequest()
            {
                CourseIds = new List<string>
                {
                    "009211be-e36e-4833-8f74-0089637e7b7f",
                    "6b1ad388-817b-4dc1-831a-e6ddea0f0ed2"
                }
            };
            
            var mockGpSystem = new Mock<IGpSystem>();
            var prescriptionService = new Mock<IPrescriptionService>();

            var postPrescriptionResult = new PrescriptionResult.SuccessfulPost();

            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionRequestValidationService())
                .Returns(_prescriptionRequestValidationService.Object);

            mockGpSystem.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            prescriptionService.Setup(x => x.OrderPrescription(_userSession, It.IsAny<RepeatPrescriptionRequest>())).Returns(Task.FromResult((PrescriptionResult)postPrescriptionResult));
            
            _prescriptionRequestValidationService
                .Setup(x => x.IsValidRepeatPrescriptionRequest(It.IsAny<RepeatPrescriptionRequest>()))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.Post(requestModel);

            // Assert
            var statusCodeResult = result as StatusCodeResult;
            Assert.IsTrue(statusCodeResult.StatusCode == 400);
        }
    }
}