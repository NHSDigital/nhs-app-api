using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTestHelper;
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.Support.Auditing;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.NominatedPharmacy
{
    [TestClass]
    public class NominatedPharmacyControllerTests
    {
        const string odsCode = "AB123";
        const string updatedOdsCode = "BB999";
        readonly string pertinentSerialChangeNumber = Guid.NewGuid().ToString();
        readonly string nhsNumber = "123 456 7891";

        private NominatedPharmacyGatewayUpdateService _systemUnderTest;
        private IFixture _fixture;

        private Mock<ILogger<NominatedPharmacyGatewayUpdateService>> _mockLogger;
        private Mock<INominatedPharmacyService> _mockNominatedPharmacyService;
        private Mock<IAuditor> _auditor;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockLogger = _fixture.Freeze<Mock<ILogger<NominatedPharmacyGatewayUpdateService>>>();
            _mockNominatedPharmacyService = _fixture.Freeze<Mock<INominatedPharmacyService>>();
            _auditor = _fixture.Freeze<Mock<IAuditor>>();

            _systemUnderTest = _fixture.Create<NominatedPharmacyGatewayUpdateService>();
        }

        [TestMethod]
        public async Task Update_ReturnsSuccessful200Result_WhenServiceReturnsSuccessfully()
        {
            // Arrange            
            var nominatedPharmacyResultBeforeUpdate = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, pertinentSerialChangeNumber);
            var nominatedPharmacyResultAfterUpdate = new GetNominatedPharmacyResult(HttpStatusCode.OK, updatedOdsCode, pertinentSerialChangeNumber);

            _mockNominatedPharmacyService
                .SetupSequence(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResultBeforeUpdate)) // first call
                .Returns(Task.FromResult(nominatedPharmacyResultAfterUpdate)); // second call

            var updateNominatedPharmacyResult = new UpdateNominatedPharmacyResult(HttpStatusCode.Accepted);

            _mockNominatedPharmacyService
                .Setup(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, nhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, updatedOdsCode, StringComparison.Ordinal))))
                .Returns(Task.FromResult(updateNominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nhsNumber, updatedOdsCode);

            // Assert
            _mockNominatedPharmacyService.Verify();
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>()), Times.Exactly(2));
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, nhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, updatedOdsCode, StringComparison.Ordinal))), Times.Once);
            var value = result.Should().BeAssignableTo<OkResult>().Subject;
        }

        [TestMethod]
        public async Task Update_ReturnsBadGateway_WhenUpdateReturnsSuccessfully_ButSubsequentRetrievalOfPharmacyIndicatesUpdateWasNotSuccessful()
        {
            // Arrange
            var nominatedPharmacyResultBeforeUpdate = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, pertinentSerialChangeNumber);

            _mockNominatedPharmacyService
                .SetupSequence(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResultBeforeUpdate)) // first call
                .Returns(Task.FromResult(nominatedPharmacyResultBeforeUpdate)); // second call also returns same value, indicating update failed

            var updateNominatedPharmacyResult = new UpdateNominatedPharmacyResult(HttpStatusCode.Accepted);

            _mockNominatedPharmacyService
                .Setup(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, nhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, updatedOdsCode, StringComparison.Ordinal))))
                .Returns(Task.FromResult(updateNominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nhsNumber, updatedOdsCode);

            // Assert
            _mockNominatedPharmacyService.Verify();

            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>()), Times.Exactly(2));
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, nhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, updatedOdsCode, StringComparison.Ordinal))), Times.Once);
            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Update_ReturnsSuccessful500Result_WhenGetPharmacyFails()
        {
            // Arrange
            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.BadGateway, odsCode, pertinentSerialChangeNumber);

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nhsNumber, updatedOdsCode);

            // Assert
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>()), Times.Once);
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(It.IsAny<NominatedPharmacyUpdate>()), Times.Never);

            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Update_Returns400Result_WhenUpdatePharmacyFails()
        {
            // Arrange            
            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, pertinentSerialChangeNumber);

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            var updateNominatedPharmacyResult = new UpdateNominatedPharmacyResult(HttpStatusCode.BadRequest);

            _mockNominatedPharmacyService
                .Setup(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, nhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, updatedOdsCode, StringComparison.Ordinal))))
                .Returns(Task.FromResult(updateNominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nhsNumber, updatedOdsCode);

            // Assert
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>()), Times.Once);
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, nhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, updatedOdsCode, StringComparison.Ordinal))), Times.Once);
            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Update_ReturnsBadGateway_WhenGetPharmacyHadMissingPertinentSerialChangeNumber()
        {
            // Arrange
            string pertinentSerialChangeNumber = null;

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, odsCode, pertinentSerialChangeNumber);

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nhsNumber, updatedOdsCode);

            // Assert
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>()), Times.Once);

            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }
    }
}
