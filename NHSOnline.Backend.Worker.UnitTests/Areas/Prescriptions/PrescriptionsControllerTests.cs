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
using NHSOnline.Backend.Worker.Router.Validators;
using NHSOnline.Backend.Worker.Session;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Prescriptions
{
    [TestClass]
    public class PrescriptionsControllerTests
    {
        private PrescriptionsController _systemUnderTest;
        private IFixture _fixture;
        private IOptions<ConfigurationSettings> _options;
        private Mock<ISystemProviderFactory> _systemProviderFactory;
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
            _systemProviderFactory = _fixture.Freeze<Mock<ISystemProviderFactory>>();
            _prescriptionRequestValidationService = _fixture.Freeze<Mock<IPrescriptionRequestValidationService>>();
            _userSession = _fixture.Create<UserSession>();
            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);
            var responseMock = new Mock<HttpResponse>();

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
            var systemProvider = new Mock<ISystemProvider>();
            var prescriptionService = new Mock<IPrescriptionService>();

            var prescriptionRequestsGetResponse = new PrescriptionListResponse();

            var getPrescriptionsResult = new GetPrescriptionsResult.SuccessfullyRetrieved(prescriptionRequestsGetResponse);

            // Arrange
            _systemProviderFactory.Setup(x => x.CreateSystemProvider(_userSession.Supplier))
                .Returns(systemProvider.Object);

            systemProvider.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            prescriptionService.Setup(x => x.Get(_userSession, date, It.IsAny<DateTimeOffset>())).Returns(Task.FromResult((GetPrescriptionsResult)getPrescriptionsResult));

            _prescriptionRequestValidationService
                .Setup(x => x.IsValidFromDate(date, It.IsAny<DateTimeOffset>()))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Get(date);

            // Assert
            _systemProviderFactory.Verify(x => x.CreateSystemProvider(_userSession.Supplier));
            systemProvider.Verify(x => x.GetPrescriptionService());
            prescriptionService.Verify(x => x.Get(_userSession, date, It.IsAny<DateTimeOffset>()));
            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            var value = okObjectResult.Value as GetPrescriptionsResult.SuccessfullyRetrieved;
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public async Task Get_CallsServiceWithDateXMonthsAgoFromConfig_WhenFromDateNotValid()
        {
            var emisSystemProvider = new Mock<ISystemProvider>();
            var prescriptionService = new Mock<IPrescriptionService>();

            var prescriptionRequestsGetResponse = new PrescriptionListResponse();

            var getPrescriptionsResult = new GetPrescriptionsResult.SuccessfullyRetrieved(prescriptionRequestsGetResponse);

            // Arrange
            _systemProviderFactory.Setup(x => x.CreateSystemProvider(_userSession.Supplier))
                .Returns(emisSystemProvider.Object);

            emisSystemProvider.Setup(x => x.GetPrescriptionService())
                .Returns(prescriptionService.Object);

            DateTimeOffset? fromDateGenerated = null;
            prescriptionService.Setup(x => x.Get(_userSession, It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>()))
                .Returns(Task.FromResult((GetPrescriptionsResult)getPrescriptionsResult))
                .Callback((UserSession s, DateTimeOffset? fd, DateTimeOffset? td) => fromDateGenerated = fd);

            _prescriptionRequestValidationService
                .Setup(x => x.IsValidFromDate(It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset>()))
                .Returns(false);

            // Act
            var result = await _systemUnderTest.Get(null);

            // Assert
            _systemProviderFactory.Verify(x => x.CreateSystemProvider(_userSession.Supplier));
            emisSystemProvider.Verify(x => x.GetPrescriptionService());
            prescriptionService.Verify(x => x.Get(_userSession, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()));
            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            var value = okObjectResult.Value as GetPrescriptionsResult.SuccessfullyRetrieved;
            Assert.IsNotNull(value);
            Assert.IsTrue(fromDateGenerated.HasValue);

            var xMonthsAgo = DateTimeOffset.Now.AddMonths(-_prescriptionsDefaultLastNumberMonthsToDisplay);
            Assert.AreEqual(xMonthsAgo.Date, fromDateGenerated.Value.Date);
        }
    }
}