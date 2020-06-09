using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests.TermsAndConditions
{
    [TestClass]
    public class TermsAndConditionsServiceTests
    {
        private string _nhsLoginId;

        private TermsAndConditionsService _systemUnderTest;
        private Mock<ITermsAndConditionsRepository> _mockTermsAndConditionsRepository;
        private Mock<IMapper<TermsAndConditionsRecord, ConsentResponse>> _mockTermsAndConditionsToConsentMapper;
        private Mock<IConsentRequestToTermsAndConditionsMapper> _mockConsentRequestToTermsAndConditionsMapper;
        private Mock<IMapper<ConsentRequest, DateTimeOffset, UpdateRecordBuilder<TermsAndConditionsRecord>>> _mockConsentRequestToUpdateMapper;
        private Mock<IMapper<AnalyticsCookieAcceptance, DateTimeOffset, UpdateRecordBuilder<TermsAndConditionsRecord>>> _mockAnalyticsCookieAcceptanceToUpdateMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _nhsLoginId = "NhsLoginId";

            _mockTermsAndConditionsRepository = new Mock<ITermsAndConditionsRepository>();
            var logger = new Mock<ILogger<TermsAndConditionsService>>().Object;

            _mockTermsAndConditionsToConsentMapper = new Mock<IMapper<TermsAndConditionsRecord, ConsentResponse>>();
            _mockConsentRequestToTermsAndConditionsMapper = new Mock<IConsentRequestToTermsAndConditionsMapper>();
            _mockConsentRequestToUpdateMapper =
                new Mock<IMapper<ConsentRequest, DateTimeOffset, UpdateRecordBuilder<TermsAndConditionsRecord>>>();
            _mockAnalyticsCookieAcceptanceToUpdateMapper =
                new Mock<IMapper<AnalyticsCookieAcceptance, DateTimeOffset,
                    UpdateRecordBuilder<TermsAndConditionsRecord>>>();

            _systemUnderTest = new TermsAndConditionsService(
                logger,
                _mockTermsAndConditionsRepository.Object,
                _mockTermsAndConditionsToConsentMapper.Object,
                _mockConsentRequestToTermsAndConditionsMapper.Object,
                _mockConsentRequestToUpdateMapper.Object,
                _mockAnalyticsCookieAcceptanceToUpdateMapper.Object);
        }

        [TestMethod]
        public async Task RecordConsent_WhenNotUpdating_ReturnsInitialConsentRecorded()
        {
            // Arrange
            var request = new ConsentRequest
            {
                UpdatingConsent = false,
                ConsentGiven = true,
                AnalyticsCookieAccepted = false,
            };

            var consentTime = DateTimeOffset.Now;

            var record = new TermsAndConditionsRecord();
            _mockTermsAndConditionsRepository
                .Setup(x => x.Create(It.IsAny<TermsAndConditionsRecord>()))
                .ReturnsAsync(new RepositoryCreateResult<TermsAndConditionsRecord>.Created(record));

            // Act
            var result = await _systemUnderTest.RecordConsent(_nhsLoginId, request, consentTime);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Create(It.IsAny<TermsAndConditionsRecord>()));
            result.Should().BeOfType<TermsAndConditionsRecordConsentResult.InitialConsentRecorded>();
        }

        [TestMethod]
        public async Task RecordConsent_WhenNotUpdatingAndThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new ConsentRequest
            {
                UpdatingConsent = false,
            };

            _mockTermsAndConditionsRepository
                .Setup(x => x.Create(It.IsAny<TermsAndConditionsRecord>()))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.RecordConsent(_nhsLoginId, request, DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Create(It.IsAny<TermsAndConditionsRecord>()));

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsRecordConsentResult.InternalServerError>();
        }

        [TestMethod]
        public async Task RecordConsent_WhenNotUpdatingRepositoryError_ReturnsInternalServerError()
        {
            // Arrange
            var request = new ConsentRequest
            {
                UpdatingConsent = false,
            };

            _mockTermsAndConditionsRepository
                .Setup(x => x.Create(It.IsAny<TermsAndConditionsRecord>()))
                .ReturnsAsync(new RepositoryCreateResult<TermsAndConditionsRecord>.RepositoryError());

            // Act
            var result = await _systemUnderTest.RecordConsent(_nhsLoginId, request, DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Create(It.IsAny<TermsAndConditionsRecord>()));

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsRecordConsentResult.InternalServerError>();
        }

        [TestMethod]
        public async Task RecordConsent_WhenUpdating_ReturnsInitialConsentRecorded()
        {
            // Arrange
            var request = new ConsentRequest
            {
                UpdatingConsent = true,
                ConsentGiven = true,
            };

            var consentTime = DateTimeOffset.Now;

            _mockTermsAndConditionsRepository.Setup(x =>
                    x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()))
                .ReturnsAsync(new RepositoryUpdateResult<TermsAndConditionsRecord>.Updated());

            // Act
            var result = await _systemUnderTest.RecordConsent(_nhsLoginId, request, consentTime);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x =>
                x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()));

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsRecordConsentResult.UpdateConsentRecorded>();
        }

        [TestMethod]
        public async Task RecordConsent_WhenUpdatingAndNotFoundExisting_ReturnsInternalServerError()
        {
            // Arrange
            var request = new ConsentRequest
            {
                UpdatingConsent = true,
            };

            _mockTermsAndConditionsRepository.Setup(x =>
                    x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()))
                .ReturnsAsync(new RepositoryUpdateResult<TermsAndConditionsRecord>.NotFound());

            // Act
            var result = await _systemUnderTest.RecordConsent(_nhsLoginId, request, DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x =>
                x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()));
            _mockTermsAndConditionsRepository.VerifyNoOtherCalls();

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsRecordConsentResult.InternalServerError>();
        }

        [TestMethod]
        public async Task RecordConsent_WhenUpdatingReturnsError_ReturnsInternalServerError()
        {
            // Arrange
            var request = new ConsentRequest
            {
                UpdatingConsent = true,
            };

            _mockTermsAndConditionsRepository.Setup(x =>
                    x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()))
                .ReturnsAsync(new RepositoryUpdateResult<TermsAndConditionsRecord>.RepositoryError());

            // Act
            var result = await _systemUnderTest.RecordConsent(_nhsLoginId, request, DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x =>
                x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()));
            _mockTermsAndConditionsRepository.VerifyNoOtherCalls();

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsRecordConsentResult.InternalServerError>();
        }
        [TestMethod]
        public async Task RecordConsent_WhenUpdatingNoChange_ReturnsSuccess()
        {
            // Arrange
            var request = new ConsentRequest
            {
                UpdatingConsent = true,
            };

            _mockTermsAndConditionsRepository.Setup(x =>
                    x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()))
                .ReturnsAsync(new RepositoryUpdateResult<TermsAndConditionsRecord>.NoChange());

            // Act
            var result = await _systemUnderTest.RecordConsent(_nhsLoginId, request, DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x =>
                x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()));
            _mockTermsAndConditionsRepository.VerifyNoOtherCalls();

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsRecordConsentResult.UpdateConsentRecorded>();
        }

        [TestMethod]
        public async Task FetchConsent_WhenFound_ReturnsSuccess()
        {
            // Arrange
            var existingRecord = new TermsAndConditionsRecord();
            var newConsentResponse = new ConsentResponse();

            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<TermsAndConditionsRecord>.Found(new[] { existingRecord }));

            _mockTermsAndConditionsToConsentMapper.Setup(x => x.Map(existingRecord))
                .Returns(newConsentResponse);

            // Act
            var result = await _systemUnderTest.FetchConsent(_nhsLoginId);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));

            result.Should().NotBeNull();
            var response = result.Should().BeOfType<TermsAndConditionsFetchConsentResult.Success>().Subject.Response;
            response.Should().Be(newConsentResponse);
        }

        [TestMethod]
        public async Task FetchConsent_WhenNotFound_ReturnsNoConsentFound()
        {
            // Arrange
            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<TermsAndConditionsRecord>.NotFound());

            // Act
            var result = await _systemUnderTest.FetchConsent(_nhsLoginId);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsFetchConsentResult.NoConsentFound>();
        }

        [TestMethod]
        public async Task FetchConsent_WhenFindThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.FetchConsent(_nhsLoginId);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsFetchConsentResult.InternalServerError>();
        }

        [TestMethod]
        public async Task FetchConsent_WhenFindReturnsRepositoryError_ReturnsInternalServerError()
        {
            // Arrange
            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<TermsAndConditionsRecord>.RepositoryError());

            // Act
            var result = await _systemUnderTest.FetchConsent(_nhsLoginId);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsFetchConsentResult.InternalServerError>();
        }

        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_WhenAccepted_ReturnsSuccess()
        {
            // Arrange
            var consentTime = DateTimeOffset.Now;

            var analyticsCookieAcceptance = new AnalyticsCookieAcceptance
            {
                AnalyticsCookieAccepted = true
            };

            _mockTermsAndConditionsRepository.Setup(x =>
                    x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()))
                .ReturnsAsync(new RepositoryUpdateResult<TermsAndConditionsRecord>.Updated());

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(_nhsLoginId, analyticsCookieAcceptance, consentTime);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x =>
                x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()));

            result.Should().NotBeNull();
            result.Should().BeOfType<ToggleAnalyticsCookieAcceptanceResult.Success>();
        }

        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_WhenNotAccepted_ReturnsSuccess()
        {
            // Arrange
            var consentTime = DateTimeOffset.Now;

            var analyticsCookieAcceptance = new AnalyticsCookieAcceptance
            {
                AnalyticsCookieAccepted = false
            };

            _mockTermsAndConditionsRepository.Setup(x =>
                    x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()))
                .ReturnsAsync(new RepositoryUpdateResult<TermsAndConditionsRecord>.Updated());

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(_nhsLoginId, analyticsCookieAcceptance, consentTime);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x =>
                x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()));

            result.Should().NotBeNull();
            result.Should().BeOfType<ToggleAnalyticsCookieAcceptanceResult.Success>();
        }

        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_WhenNotFound_ReturnsFailure()
        {
            // Arrange
            _mockTermsAndConditionsRepository.Setup(x =>
                    x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()))
                .ReturnsAsync(new RepositoryUpdateResult<TermsAndConditionsRecord>.NotFound());

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(_nhsLoginId, new AnalyticsCookieAcceptance(), DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x =>
                x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()));

            result.Should().NotBeNull();
            result.Should().BeOfType<ToggleAnalyticsCookieAcceptanceResult.Failure>();
        }

        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_WhenNoChange_ReturnsSuccess()
        {
            // Arrange
            _mockTermsAndConditionsRepository.Setup(x =>
                    x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()))
                .ReturnsAsync(new RepositoryUpdateResult<TermsAndConditionsRecord>.NoChange());

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(_nhsLoginId, new AnalyticsCookieAcceptance(), DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x =>
                x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()));

            result.Should().NotBeNull();
            result.Should().BeOfType<ToggleAnalyticsCookieAcceptanceResult.Success>();
        }

        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_WhenRepositoryError_ReturnsFailure()
        {
            // Arrange
            _mockTermsAndConditionsRepository.Setup(x =>
                    x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()))
                .ReturnsAsync(new RepositoryUpdateResult<TermsAndConditionsRecord>.RepositoryError());

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(_nhsLoginId, new AnalyticsCookieAcceptance(), DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x =>
                x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()));

            result.Should().NotBeNull();
            result.Should().BeOfType<ToggleAnalyticsCookieAcceptanceResult.Failure>();
        }


        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_WhenThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockTermsAndConditionsRepository.Setup(x =>
                    x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()))
                .ThrowsAsync(new ArgumentException(""));

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(_nhsLoginId, new AnalyticsCookieAcceptance(), DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x =>
                x.Update(_nhsLoginId, It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>()));

            result.Should().NotBeNull();
            result.Should().BeOfType<ToggleAnalyticsCookieAcceptanceResult.Failure>();
        }
    }
}
