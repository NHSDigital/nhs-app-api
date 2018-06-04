using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Prescriptions;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Prescriptions;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Prescriptions
{
    [TestClass]
    public class PrescriptionsControllerTests
    {
        private PrescriptionsController _systemUnderTest;
        private IFixture _fixture;
        private IOptions<ConfigurationSettings> _options;
        private Mock<IBridgeFactory> _mockBridgeFactory;
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
            _mockBridgeFactory = _fixture.Freeze<Mock<IBridgeFactory>>();
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
            var mockBridge = new Mock<IBridge>();
            var prescriptionService = new Mock<IPrescriptionService>();

            var prescriptionRequestsGetResponse = new PrescriptionListResponse();

            var getPrescriptionsResult = new PrescriptionResult.SuccessfullGet(prescriptionRequestsGetResponse);

            // Arrange
            _mockBridgeFactory.Setup(x => x.CreateBridge(_userSession.Supplier))
                .Returns(mockBridge.Object);

            mockBridge.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            prescriptionService.Setup(x => x.Get(_userSession, date, It.IsAny<DateTimeOffset>())).Returns(Task.FromResult((PrescriptionResult)getPrescriptionsResult));

            _prescriptionRequestValidationService
                .Setup(x => x.IsValidFromDate(date, It.IsAny<DateTimeOffset>()))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Get(date);

            // Assert
            _mockBridgeFactory.Verify(x => x.CreateBridge(_userSession.Supplier));
            mockBridge.Verify(x => x.GetPrescriptionService());
            prescriptionService.Verify(x => x.Get(_userSession, date, It.IsAny<DateTimeOffset>()));
            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            var value = okObjectResult.Value as PrescriptionResult.SuccessfullGet;
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public async Task Get_CallsServiceWithDateXMonthsAgoFromConfig_WhenFromDateNotValid()
        {
            var mockEmisBridge = new Mock<IBridge>();
            var prescriptionService = new Mock<IPrescriptionService>();

            var prescriptionRequestsGetResponse = new PrescriptionListResponse();

            var getPrescriptionsResult = new PrescriptionResult.SuccessfullGet(prescriptionRequestsGetResponse);

            // Arrange
            _mockBridgeFactory.Setup(x => x.CreateBridge(_userSession.Supplier))
                .Returns(mockEmisBridge.Object);

            mockEmisBridge.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            DateTimeOffset? fromDateGenerated = null;
            prescriptionService.Setup(x => x.Get(_userSession, It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>()))
                .Returns(Task.FromResult((PrescriptionResult)getPrescriptionsResult))
                .Callback((UserSession s, DateTimeOffset? fd, DateTimeOffset? td) => fromDateGenerated = fd);

            _prescriptionRequestValidationService
                .Setup(x => x.IsValidFromDate(It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset>()))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.Get(null);

            // Assert
            _mockBridgeFactory.Verify(x => x.CreateBridge(_userSession.Supplier));
            mockEmisBridge.Verify(x => x.GetPrescriptionService());
            prescriptionService.Verify(x => x.Get(_userSession, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()));
            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            var value = okObjectResult.Value as PrescriptionResult.SuccessfullGet;
            Assert.IsNotNull(value);
            Assert.IsTrue(fromDateGenerated.HasValue);

            var xMonthsAgo = DateTimeOffset.Now.AddMonths(-_prescriptionsDefaultLastNumberMonthsToDisplay);
            Assert.AreEqual(xMonthsAgo.Date, fromDateGenerated.Value.Date);
        }
        
        [TestMethod]
        public async Task Post_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var requestModel = new RepeatPrescriptionRequest();
            var mockBridge = new Mock<IBridge>();
            var prescriptionService = new Mock<IPrescriptionService>();

            var postPrescriptionResult = new PrescriptionResult.SuccessfullPost();

            // Arrange
            _mockBridgeFactory.Setup(x => x.CreateBridge(_userSession.Supplier))
                .Returns(mockBridge.Object);

            mockBridge.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            prescriptionService.Setup(x => x.Post(_userSession, It.IsAny<RepeatPrescriptionRequest>())).Returns(Task.FromResult((PrescriptionResult)postPrescriptionResult));
            
            _prescriptionRequestValidationService
                .Setup(x => x.IsValidRepeatPrescriptionRequest(It.IsAny<RepeatPrescriptionRequest>()))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Post(requestModel);

            // Assert
            _mockBridgeFactory.Verify(x => x.CreateBridge(_userSession.Supplier));
            mockBridge.Verify(x => x.GetPrescriptionService());
            prescriptionService.Verify(x => x.Post(_userSession, It.IsAny<RepeatPrescriptionRequest>()));
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
            
            var mockBridge = new Mock<IBridge>();
            var prescriptionService = new Mock<IPrescriptionService>();

            var postPrescriptionResult = new PrescriptionResult.SuccessfullPost();

            // Arrange
            _mockBridgeFactory.Setup(x => x.CreateBridge(_userSession.Supplier))
                .Returns(mockBridge.Object);

            mockBridge.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            prescriptionService.Setup(x => x.Post(_userSession, It.IsAny<RepeatPrescriptionRequest>())).Returns(Task.FromResult((PrescriptionResult)postPrescriptionResult));
            
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