using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Controllers;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Prescriptions
{
    [TestClass]
    public class PrescriptionsControllerTests
    {
        private PrescriptionsController _systemUnderTest;
        private static IFixture _fixture;
        private static IOptions<ConfigurationSettings> _options;

        private static int _prescriptionsDefaultLastNumberMonthsToDisplay;

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

            var httpContextMock = new Mock<HttpContext>();
            var responseMock = new Mock<HttpResponse>();

            _systemUnderTest = _fixture.Create<PrescriptionsController>();
        }

        [TestMethod]
        public void Get_PrescriptionsGet_ReturnsEmptyPrescriptionsList()
        {
            // Arrange
            var fromDate = DateTimeOffset.Now;

            // Act
            var result = _systemUnderTest.Get(fromDate);

            // Assert
            var resultObject = (result as OkObjectResult);
            var statusCodeResult = result.GetType().Should().BeAssignableTo<OkObjectResult>();

            var prescriptionListResponse = (resultObject.Value as PrescriptionListResponse);

            prescriptionListResponse.Courses.Count().Should().Be(0);
            prescriptionListResponse.Prescriptions.Count().Should().Be(0);
        }
    }
}