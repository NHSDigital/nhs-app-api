using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTestHelper;
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.Support;

 namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.NominatedPharmacy
{
    [TestClass]
    public class NominatedPharmacyGatewayUpdateServiceTests
    {
        private const string OdsCode = "AB123";
        private const string NominatedPharmacyTypeP3 = "P3";
        private const string UpdatedOdsCode = "BB999";
        private readonly string _pertinentSerialChangeNumber = Guid.NewGuid().ToString();
        private const string NhsNumber = "123 456 7891";
        private const string ObjectId = "ABXXX";

        private NominatedPharmacyGatewayUpdateService _systemUnderTest;
        private IFixture _fixture;
        private UserSession _userSession;


        private Mock<INominatedPharmacyService> _mockNominatedPharmacyService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockNominatedPharmacyService = _fixture.Freeze<Mock<INominatedPharmacyService>>();
            
            _userSession = _fixture.Create<UserSession>();

            _systemUnderTest = _fixture.Create<NominatedPharmacyGatewayUpdateService>();
        }

        [TestMethod]
        public async Task Update_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Arrange            
            var nominatedPharmacyResultBeforeUpdate = new GetNominatedPharmacyResult.Success(
                new GetNominatedPharmacyResponse(HttpStatusCode.OK, OdsCode, _pertinentSerialChangeNumber, true, NominatedPharmacyTypeP3, ObjectId));
            
            var nominatedPharmacyResultAfterUpdate = new GetNominatedPharmacyResult.Success(
                new GetNominatedPharmacyResponse(HttpStatusCode.OK, UpdatedOdsCode, _pertinentSerialChangeNumber, true, NominatedPharmacyTypeP3, ObjectId));

            _mockNominatedPharmacyService
                .SetupSequence(x => x.GetNominatedPharmacy(NhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult<GetNominatedPharmacyResult>(nominatedPharmacyResultBeforeUpdate)) // first call
                .Returns(Task.FromResult<GetNominatedPharmacyResult>(nominatedPharmacyResultAfterUpdate)); // second call

            var updateNominatedPharmacyResult = new UpdateNominatedPharmacyResult(HttpStatusCode.Accepted);

            _mockNominatedPharmacyService
                .Setup(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, NhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, _pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, UpdatedOdsCode, StringComparison.Ordinal) &&
                    string.Equals(npu.ObjectId, ObjectId, StringComparison.Ordinal))))
                .Returns(Task.FromResult(updateNominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(NhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession);

            // Assert
            _mockNominatedPharmacyService.Verify();
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Exactly(2));
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, NhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, _pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, UpdatedOdsCode, StringComparison.Ordinal) &&
                    string.Equals(npu.ObjectId, ObjectId, StringComparison.Ordinal))), Times.Once);
            result.Should().BeAssignableTo<UpdateNominatedPharmacyResponse.Success>();
        }

        [TestMethod]
        public async Task Update_ReturnsUpdatedButStillOldCode_WhenUpdateReturnsSuccessfully_ButSubsequentRetrievalOfPharmacyIndicatesUpdateWasNotSuccessful()
        {
            // Arrange
            var nominatedPharmacyResultBeforeUpdate = new GetNominatedPharmacyResult.Success(new GetNominatedPharmacyResponse(
                    HttpStatusCode.OK, OdsCode, _pertinentSerialChangeNumber, true, NominatedPharmacyTypeP3, ObjectId));
            
            _mockNominatedPharmacyService
                .SetupSequence(x => x.GetNominatedPharmacy(NhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult<GetNominatedPharmacyResult>(nominatedPharmacyResultBeforeUpdate)) // first call
                .Returns(Task.FromResult<GetNominatedPharmacyResult>(nominatedPharmacyResultBeforeUpdate)); // second call also returns same value, indicating update failed

            var updateNominatedPharmacyResult = new UpdateNominatedPharmacyResult(HttpStatusCode.Accepted);

            _mockNominatedPharmacyService
                .Setup(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, NhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, _pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, UpdatedOdsCode, StringComparison.Ordinal) &&
                    string.Equals(npu.ObjectId, ObjectId, StringComparison.Ordinal))))
                .Returns(Task.FromResult(updateNominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(NhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession);

            // Assert
            _mockNominatedPharmacyService.Verify();

            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Exactly(2));
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, NhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, _pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, UpdatedOdsCode, StringComparison.Ordinal) &&
                    string.Equals(npu.ObjectId, ObjectId, StringComparison.Ordinal))), Times.Once);
            result.Should().BeAssignableTo<UpdateNominatedPharmacyResponse.UpdatedButStillOldCode>();
        }

        [TestMethod]
        public async Task Update_ReturnsFailure_WhenGetPharmacyFails()
        {
            // Arrange
            var nominatedPharmacyResult = new GetNominatedPharmacyResult.Success(new GetNominatedPharmacyResponse(
                HttpStatusCode.BadGateway, OdsCode, _pertinentSerialChangeNumber, false, NominatedPharmacyTypeP3, ObjectId));

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(NhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult<GetNominatedPharmacyResult>(nominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(NhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession);

            // Assert
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Once);
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(It.IsAny<NominatedPharmacyUpdate>()), Times.Never);

            result.Should().BeAssignableTo<UpdateNominatedPharmacyResponse.GetNominatedPharmacyFailure>();
        }

        [TestMethod]
        public async Task Update_ReturnsBadGateway_WhenUpdatePharmacyFails()
        {
            // Arrange            
            var nominatedPharmacyResult = new GetNominatedPharmacyResult.Success(new GetNominatedPharmacyResponse(
                HttpStatusCode.OK, OdsCode, _pertinentSerialChangeNumber, true, NominatedPharmacyTypeP3, ObjectId));

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(NhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult<GetNominatedPharmacyResult>(nominatedPharmacyResult))
                .Verifiable();

            var updateNominatedPharmacyResult = new UpdateNominatedPharmacyResult(HttpStatusCode.BadRequest);

            _mockNominatedPharmacyService
                .Setup(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, NhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, _pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, UpdatedOdsCode, StringComparison.Ordinal) &&
                    string.Equals(npu.ObjectId, ObjectId, StringComparison.Ordinal))))
                .Returns(Task.FromResult(updateNominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(NhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession);

            // Assert
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Once);
            _mockNominatedPharmacyService.Verify(x => x.UpdateNominatedPharmacy(It.Is<NominatedPharmacyUpdate>(
                    npu =>
                    string.Equals(npu.NhsNumber, NhsNumber, StringComparison.Ordinal) &&
                    string.Equals(npu.PertinentSerialChangeNumber, _pertinentSerialChangeNumber, StringComparison.Ordinal) &&
                    npu.HasExistingNominatedPharmacy == true &&
                    string.Equals(npu.UpdatedOdsCode, UpdatedOdsCode, StringComparison.Ordinal) &&
                    string.Equals(npu.ObjectId, ObjectId, StringComparison.Ordinal))), Times.Once);
            result.Should().BeAssignableTo<UpdateNominatedPharmacyResponse.BadGateway>();
        }

        [TestMethod]
        public async Task Update_ReturnsNominatedPharmacyFailure_WhenGetPharmacyHadMissingPertinentSerialChangeNumber()
        {
            // Arrange
            string pertinentSerialChangeNumber = null;

            var nominatedPharmacyResult = new GetNominatedPharmacyResult.Success(new GetNominatedPharmacyResponse(
                HttpStatusCode.OK, OdsCode, pertinentSerialChangeNumber, true, NominatedPharmacyTypeP3, ObjectId));

            _mockNominatedPharmacyService
                .Setup(x => x.GetNominatedPharmacy(NhsNumber, _userSession.CitizenIdUserSession))
                .Returns(Task.FromResult<GetNominatedPharmacyResult>(nominatedPharmacyResult))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(NhsNumber, UpdatedOdsCode, _userSession.CitizenIdUserSession);

            // Assert
            _mockNominatedPharmacyService.Verify(x => x.GetNominatedPharmacy(It.IsAny<string>(), It.IsAny<CitizenIdUserSession>()), Times.Once);

            result.Should().BeAssignableTo<UpdateNominatedPharmacyResponse.GetNominatedPharmacyFailure>();
        }
    }
}
