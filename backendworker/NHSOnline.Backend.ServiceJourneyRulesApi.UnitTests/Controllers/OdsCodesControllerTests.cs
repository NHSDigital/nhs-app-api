using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Controllers;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.Controllers
{
    [TestClass]
    public class OdsCodesControllerTests
    {
        private const string TestOdsCode = "XYZ"; 

        private Mock<ILogger<OdsCodesController>> _mockLogger;
        private Mock<IJourneyRepository> _mockJourneyRepository;

        private OdsCodesController _systemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<OdsCodesController>>(MockBehavior.Loose);
            _mockJourneyRepository = new Mock<IJourneyRepository>(MockBehavior.Strict);

            _systemUnderTest = new OdsCodesController(
                _mockLogger.Object,
                _mockJourneyRepository.Object
            );
        }

        [TestMethod]
        public void Get_JourneyRepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockJourneyRepository
                .Setup(x => x.GetOdsCodes())
                .Throws<Exception>();

            // Act
            var result = _systemUnderTest.Get();

            // Assert
            _mockJourneyRepository.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public void Get_JourneyRepositoryReturnsEmptySet_ReturnsEmptySet()
        {
            // Arrange
            _mockJourneyRepository
                .Setup(x => x.GetOdsCodes())
                .Returns(new OdsCodesResponse());

            // Act
            var result = _systemUnderTest.Get();

            // Assert
            _mockJourneyRepository.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var output = statusCodeResult.Value.Should().BeAssignableTo<OdsCodesResponse>().Subject;
            output.OdsCodes.Should().BeEmpty();
        }

        [TestMethod]
        public void Get_JourneyRepositoryReturnsData_ReturnsData()
        {
            // Arrange
            _mockJourneyRepository
                .Setup(x => x.GetOdsCodes())
                .Returns(new OdsCodesResponse
                {
                    OdsCodes = new[] { TestOdsCode }
                });

            // Act
            var result = _systemUnderTest.Get();

            // Assert
            _mockJourneyRepository.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var output = statusCodeResult.Value.Should().BeAssignableTo<OdsCodesResponse>().Subject;
            output.OdsCodes.Should().BeEquivalentTo(TestOdsCode);
        }
    }
}
