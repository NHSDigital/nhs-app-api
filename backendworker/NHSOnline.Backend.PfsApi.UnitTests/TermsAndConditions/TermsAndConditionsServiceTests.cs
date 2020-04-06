using System;
using System.Globalization;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;

namespace NHSOnline.Backend.PfsApi.UnitTests.TermsAndConditions
{
    [TestClass]
    public class TermsAndConditionsServiceTests
    {
        private string _nhsLoginId;

        private ITermsAndConditionsService _systemUnderTest;
        private Mock<ITermsAndConditionsRepository> _mockTermsAndConditionsRepository;
        private Mock<ITermsAndConditionsConfiguration> _mockTermsAndConditionsConfiguration;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            _nhsLoginId = fixture.Create<string>();

            _mockTermsAndConditionsRepository = fixture.Freeze<Mock<ITermsAndConditionsRepository>>();
            _mockTermsAndConditionsConfiguration = fixture.Freeze<Mock<ITermsAndConditionsConfiguration>>();

            _systemUnderTest = fixture.Create<TermsAndConditionsService>();
        }

        [TestMethod]
        public async Task RecordConsent_WhenNotUpdatingAndNotConsentedAnalytics_ReturnsInitialConsentRecorded()
        {
            // Arrange
            var request = new ConsentRequest
            {
                UpdatingConsent = false,
                ConsentGiven = true,
                AnalyticsCookieAccepted = false,
            };

            var consentTime = DateTimeOffset.Now;

            TermsAndConditionsRecord record = null;
            _mockTermsAndConditionsRepository
                .Setup(x => x.Create(It.IsAny<TermsAndConditionsRecord>()))
                .Callback<TermsAndConditionsRecord>(x => record = x);

            // Act
            var result = await _systemUnderTest.RecordConsent(_nhsLoginId, request, consentTime);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Create(It.IsAny<TermsAndConditionsRecord>()));

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsRecordConsentResult.InitialConsentRecorded>();

            record.Should().NotBeNull();
            record.NhsLoginId.Should().Be(_nhsLoginId);
            record.ConsentGiven.Should().BeTrue();
            record.DateOfConsent.Should().Be(consentTime.ToString("s", CultureInfo.InvariantCulture));
            record.AnalyticsCookieAccepted.Should().BeFalse();
            record.DateOfAnalyticsCookieToggle.Should().Be(consentTime.ToString("s", CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public async Task RecordConsent_WhenNotUpdatingAndConsentedAnalytics_ReturnsInitialConsentRecorded()
        {
            // Arrange
            var request = new ConsentRequest
            {
                UpdatingConsent = false,
                ConsentGiven = true,
                AnalyticsCookieAccepted = true,
            };

            var consentTime = DateTimeOffset.Now;

            TermsAndConditionsRecord record = null;
            _mockTermsAndConditionsRepository
                .Setup(x => x.Create(It.IsAny<TermsAndConditionsRecord>()))
                .Callback<TermsAndConditionsRecord>(x => record = x);

            // Act
            var result = await _systemUnderTest.RecordConsent(_nhsLoginId, request, consentTime);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Create(It.IsAny<TermsAndConditionsRecord>()));

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsRecordConsentResult.InitialConsentRecorded>();

            record.Should().NotBeNull();
            record.NhsLoginId.Should().Be(_nhsLoginId);
            record.ConsentGiven.Should().BeTrue();
            record.DateOfConsent.Should().Be(consentTime.ToString("s", CultureInfo.InvariantCulture));
            record.AnalyticsCookieAccepted.Should().BeTrue();
            record.DateOfAnalyticsCookieToggle.Should().Be(consentTime.ToString("s", CultureInfo.InvariantCulture));
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
        public async Task RecordConsent_WhenUpdatingAndFoundExisting_ReturnsInitialConsentRecorded()
        {
            // Arrange
            var request = new ConsentRequest
            {
                UpdatingConsent = true,
                ConsentGiven = true,
            };

            var consentTime = DateTimeOffset.Now;
            var existingRecord = new TermsAndConditionsRecord
            {
                NhsLoginId = _nhsLoginId,
                ConsentGiven = true,
                DateOfConsent = DateTimeOffset.Now.AddHours(-5).ToString("s", CultureInfo.InvariantCulture)
            };

            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .ReturnsAsync(existingRecord);

            // Act
            var result = await _systemUnderTest.RecordConsent(_nhsLoginId, request, consentTime);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));
            _mockTermsAndConditionsRepository.Verify(x => x.Update(It.IsAny<TermsAndConditionsRecord>()));

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsRecordConsentResult.UpdateConsentRecorded>();

            existingRecord.NhsLoginId.Should().Be(_nhsLoginId);
            existingRecord.ConsentGiven.Should().BeTrue();
            existingRecord.DateOfConsent.Should().Be(consentTime.ToString("s", CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public async Task RecordConsent_WhenUpdatingAndNotFoundExisting_ReturnsInternalServerError()
        {
            // Arrange
            var request = new ConsentRequest
            {
                UpdatingConsent = true,
            };

            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .ReturnsAsync((TermsAndConditionsRecord)null);

            // Act
            var result = await _systemUnderTest.RecordConsent(_nhsLoginId, request, DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));
            _mockTermsAndConditionsRepository.VerifyNoOtherCalls();

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsRecordConsentResult.InternalServerError>();
        }

        [TestMethod]
        public async Task RecordConsent_WhenUpdatingAndFindThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new ConsentRequest
            {
                UpdatingConsent = true,
            };

            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.RecordConsent(_nhsLoginId, request, DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));
            _mockTermsAndConditionsRepository.VerifyNoOtherCalls();

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsRecordConsentResult.InternalServerError>();
        }

        [TestMethod]
        public async Task RecordConsent_WhenUpdatingAndThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new ConsentRequest
            {
                UpdatingConsent = true,
            };

            _mockTermsAndConditionsRepository.Setup(x => x.Update(It.IsAny<TermsAndConditionsRecord>()))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.RecordConsent(_nhsLoginId, request, DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));
            _mockTermsAndConditionsRepository.Verify(x => x.Update(It.IsAny<TermsAndConditionsRecord>()));

            result.Should().NotBeNull();
            result.Should().BeOfType<TermsAndConditionsRecordConsentResult.InternalServerError>();
        }

        [TestMethod]
        public async Task FetchConsent_WhenFoundAndNoUpdatedConsentRequired_ReturnsSuccess()
        {
            // Arrange
            var existingRecord = new TermsAndConditionsRecord
            {
                NhsLoginId = _nhsLoginId,
                ConsentGiven = true,
                DateOfConsent = DateTimeOffset.Now.ToString("s", CultureInfo.InvariantCulture),
                AnalyticsCookieAccepted = true
            };

            _mockTermsAndConditionsConfiguration.SetupGet(x => x.EffectiveDate)
                .Returns(DateTimeOffset.Now.AddHours(-5));

            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .ReturnsAsync(existingRecord);

            // Act
            var result = await _systemUnderTest.FetchConsent(_nhsLoginId);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));

            result.Should().NotBeNull();
            var response = result.Should().BeOfType<TermsAndConditionsFetchConsentResult.Success>().Subject.Response;

            response.Should().NotBeNull();
            response.ConsentGiven.Should().Be(existingRecord.ConsentGiven);
            response.UpdatedConsentRequired.Should().BeFalse();
            response.AnalyticsCookieAccepted.Should().Be(existingRecord.AnalyticsCookieAccepted);
        }

        [TestMethod]
        public async Task FetchConsent_WhenFoundAndUpdatedConsentRequired_ReturnsSuccess()
        {
            // Arrange
            var existingRecord = new TermsAndConditionsRecord
            {
                NhsLoginId = _nhsLoginId,
                ConsentGiven = true,
                DateOfConsent = DateTimeOffset.Now.AddHours(-5).ToString("s", CultureInfo.InvariantCulture),
                AnalyticsCookieAccepted = true
            };

            _mockTermsAndConditionsConfiguration.SetupGet(x => x.EffectiveDate)
                .Returns(DateTimeOffset.Now);

            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .ReturnsAsync(existingRecord);

            // Act
            var result = await _systemUnderTest.FetchConsent(_nhsLoginId);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));

            result.Should().NotBeNull();
            var response = result.Should().BeOfType<TermsAndConditionsFetchConsentResult.Success>().Subject.Response;

            response.Should().NotBeNull();
            response.ConsentGiven.Should().Be(existingRecord.ConsentGiven);
            response.UpdatedConsentRequired.Should().BeTrue();
            response.AnalyticsCookieAccepted.Should().Be(existingRecord.AnalyticsCookieAccepted);
        }

        [TestMethod]
        public async Task FetchConsent_WhenNotFound_ReturnsNoConsentFound()
        {
            // Arrange
            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .ReturnsAsync((TermsAndConditionsRecord)null);

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
        public async Task ToggleAnalyticsCookieAcceptance_WhenAccepted_ReturnsSuccess()
        {
            // Arrange
            var consentTime = DateTimeOffset.Now;

            var analyticsCookieAcceptance = new AnalyticsCookieAcceptance
            {
                AnalyticsCookieAccepted = true
            };

            var existingRecord = new TermsAndConditionsRecord
            {
                AnalyticsCookieAccepted = false,
                DateOfAnalyticsCookieToggle = null
            };

            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .ReturnsAsync(existingRecord);

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(_nhsLoginId, analyticsCookieAcceptance, consentTime);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));

            result.Should().NotBeNull();
            result.Should().BeOfType<ToggleAnalyticsCookieAcceptanceResult.Success>();

            existingRecord.AnalyticsCookieAccepted.Should().BeTrue();
            existingRecord.DateOfAnalyticsCookieToggle.Should().Be(consentTime.ToString("s", CultureInfo.InvariantCulture));
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

            var existingRecord = new TermsAndConditionsRecord
            {
                AnalyticsCookieAccepted = false,
                DateOfAnalyticsCookieToggle = null
            };

            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .ReturnsAsync(existingRecord);

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(_nhsLoginId, analyticsCookieAcceptance, consentTime);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));

            result.Should().NotBeNull();
            result.Should().BeOfType<ToggleAnalyticsCookieAcceptanceResult.Success>();

            existingRecord.AnalyticsCookieAccepted.Should().BeFalse();
            existingRecord.DateOfAnalyticsCookieToggle.Should().Be(consentTime.ToString("s", CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_WhenNotFound_ReturnsFailure()
        {
            // Arrange
            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .ReturnsAsync((TermsAndConditionsRecord)null);

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(_nhsLoginId, new AnalyticsCookieAcceptance(), DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));

            result.Should().NotBeNull();
            result.Should().BeOfType<ToggleAnalyticsCookieAcceptanceResult.Failure>();
        }

        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_WhenFindThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockTermsAndConditionsRepository.Setup(x => x.Find(_nhsLoginId))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(_nhsLoginId, new AnalyticsCookieAcceptance(), DateTimeOffset.Now);

            // Assert
            _mockTermsAndConditionsRepository.Verify(x => x.Find(_nhsLoginId));

            result.Should().NotBeNull();
            result.Should().BeOfType<ToggleAnalyticsCookieAcceptanceResult.Failure>();
        }
    }
}
