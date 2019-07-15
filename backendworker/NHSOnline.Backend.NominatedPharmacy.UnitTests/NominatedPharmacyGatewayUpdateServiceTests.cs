﻿using System;
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
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.NominatedPharmacy
{
    [TestClass]
    public class NominatedPharmacyControllerTests
    {
        private const string OdsCode = "AB123";
        private const string NominatedPharmacyTypeP3 = "P3";
        private const string UpdatedOdsCode = "BB999";
        private readonly string pertinentSerialChangeNumber = Guid.NewGuid().ToString();
        private readonly string nhsNumber = "123 456 7891";

        private NominatedPharmacyGatewayUpdateService _systemUnderTest;
        private IFixture _fixture;
        private UserSession _userSession;


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
            
            _userSession = _fixture.Create<UserSession>();

            _systemUnderTest = _fixture.Create<NominatedPharmacyGatewayUpdateService>();
        }

        [TestMethod]
        public async Task Update_ReturnsSuccessful200Result_WhenServiceReturnsSuccessfully()
        {
            // Arrange            
            var nominatedPharmacyResultBeforeUpdate = new GetNominatedPharmacyResult(HttpStatusCode.OK, OdsCode, pertinentSerialChangeNumber, true, NominatedPharmacyTypeP3);
            var nominatedPharmacyResultAfterUpdate = new GetNominatedPharmacyResult(HttpStatusCode.OK, UpdatedOdsCode, pertinentSerialChangeNumber, true, NominatedPharmacyTypeP3);

            _mockNominatedPharmacyService
                .SetupSequence(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult(nominatedPharmacyResultBeforeUpdate)) // first call
                .Returns(Task.FromResult(nominatedPharmacyResultAfterUpdate)); // second call

            var updateNominatedPharmacyResult = new UpdateNominatedPharmacyResult(HttpStatusCode.Accepted);

            _mockNominatedPharmacyService
                .Setup(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, nhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, UpdatedOdsCode, StringComparison.Ordinal))))
                .Returns(Task.FromResult(updateNominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession);

            // Assert
            _mockNominatedPharmacyService.Verify();
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Exactly(2));
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, nhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, UpdatedOdsCode, StringComparison.Ordinal))), Times.Once);
            var value = result.Should().BeAssignableTo<OkResult>().Subject;
        }

        [TestMethod]
        public async Task Update_ReturnsBadGateway_WhenUpdateReturnsSuccessfully_ButSubsequentRetrievalOfPharmacyIndicatesUpdateWasNotSuccessful()
        {
            // Arrange
            var nominatedPharmacyResultBeforeUpdate = new GetNominatedPharmacyResult(HttpStatusCode.OK, OdsCode, pertinentSerialChangeNumber, true, NominatedPharmacyTypeP3);

            _mockNominatedPharmacyService
                .SetupSequence(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult(nominatedPharmacyResultBeforeUpdate)) // first call
                .Returns(Task.FromResult(nominatedPharmacyResultBeforeUpdate)); // second call also returns same value, indicating update failed

            var updateNominatedPharmacyResult = new UpdateNominatedPharmacyResult(HttpStatusCode.Accepted);

            _mockNominatedPharmacyService
                .Setup(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, nhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, UpdatedOdsCode, StringComparison.Ordinal))))
                .Returns(Task.FromResult(updateNominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession);

            // Assert
            _mockNominatedPharmacyService.Verify();

            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Exactly(2));
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, nhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, UpdatedOdsCode, StringComparison.Ordinal))), Times.Once);
            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Update_ReturnsSuccessful500Result_WhenGetPharmacyFails()
        {
            // Arrange
            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.BadGateway, OdsCode, pertinentSerialChangeNumber, false, NominatedPharmacyTypeP3);

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession);

            // Assert
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Once);
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(It.IsAny<NominatedPharmacyUpdate>()), Times.Never);

            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Update_Returns400Result_WhenUpdatePharmacyFails()
        {
            // Arrange            
            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, OdsCode, pertinentSerialChangeNumber, true, NominatedPharmacyTypeP3);

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            var updateNominatedPharmacyResult = new UpdateNominatedPharmacyResult(HttpStatusCode.BadRequest);

            _mockNominatedPharmacyService
                .Setup(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, nhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, UpdatedOdsCode, StringComparison.Ordinal))))
                .Returns(Task.FromResult(updateNominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession);

            // Assert
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Once);
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, nhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, UpdatedOdsCode, StringComparison.Ordinal))), Times.Once);
            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Update_ReturnsBadGateway_WhenGetPharmacyHadMissingPertinentSerialChangeNumber()
        {
            // Arrange
            string pertinentSerialChangeNumber = null;

            var nominatedPharmacyResult = new GetNominatedPharmacyResult(HttpStatusCode.OK, OdsCode, pertinentSerialChangeNumber, true, NominatedPharmacyTypeP3);

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(nhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult(nominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession);

            // Assert
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Once);

            var value = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            value.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }
    }
}
