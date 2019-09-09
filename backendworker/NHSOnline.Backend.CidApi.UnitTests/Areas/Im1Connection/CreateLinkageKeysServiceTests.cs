using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.CidApi.Areas.Im1Connection;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Temporal;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.CidApi.UnitTests.Areas.Im1Connection
{
    [TestClass]
    public class CreateLinkageKeysServiceTests
    {
        private const string DefaultOdsCode = "AB1234";
        private const string DefaultNhsNumber = "XX00000A";
        private const string DefaultSurname = "Surname";
        private static readonly DateTime? DefaultDateOfBirth = new DateTime(1980,1,1);
        private const string DefaultIdentityToken = "IDTOKEN";
        private const string DefaultEmailAddress = "john@email.com";
        private IFixture _fixture;
        private Mock<IMinimumAgeValidator> _mockMinimumAgeValidator;
        private ConfigurationSettings _settings;
        private Mock<IOdsCodeMassager> _odsCodeMassager;
        private CreateLinkageKeysService _systemUnderTest;
        
        private const int MinimumLinkageAge = 16;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Freeze<Mock<IAuditor>>();
            _fixture.Freeze<Mock<ILogger<CreateLinkageKeysService>>>();
            _settings = _fixture.Freeze<ConfigurationSettings>();
            _settings.MinimumLinkageAge = (MinimumLinkageAge);
            _mockMinimumAgeValidator = _fixture.Freeze<Mock<IMinimumAgeValidator>>();
            _mockMinimumAgeValidator.Setup(x => x.IsValid(It.IsAny<DateTime>(), It.IsAny<int>())).Returns(true);
            _odsCodeMassager = _fixture.Freeze<Mock<IOdsCodeMassager>>();
            _odsCodeMassager.Setup(x => x.CheckOdsCode(DefaultOdsCode)).Returns(DefaultOdsCode);
            _systemUnderTest = _fixture.Create<CreateLinkageKeysService>();
        }
        
        [TestMethod]
        public async Task Create_WhenServiceIsSuccessfullyCalled_ReturnsTheSuccessResponse()
        {
            // Arrange
            var mockResult = new LinkageResult.SuccessfullyCreated(_fixture.Create<LinkageResponse>());
            var mockLinkageService = MockLinkageService(mockResult);
            var gpSystemMock = MockGpSystem(mockLinkageService);           
            var request = CreateLinkageRequest();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(request, gpSystemMock.Object);

            // Assert
            mockLinkageService.Verify(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req => req.NhsNumber.Equals(DefaultNhsNumber, StringComparison.Ordinal) &&
                                                   req.Surname.Equals(DefaultSurname, StringComparison.Ordinal) &&
                                                   req.DateOfBirth.Equals(DefaultDateOfBirth) &&
                                                   req.OdsCode.Equals(DefaultOdsCode, StringComparison.Ordinal) &&
                                                   req.IdentityToken.Equals(DefaultIdentityToken, StringComparison.Ordinal) &&
                                                   req.EmailAddress.Equals(DefaultEmailAddress, StringComparison.Ordinal))), Times.Once);
            
            result.Should().BeOfType<LinkageResult.SuccessfullyCreated>();
        }

        [TestMethod]
        public async Task Create_WhenDateOfBirthIsUnderMinimumLinkageAge_ReturnsPatientNonCompetentOrUnderMinimumAge()
        {
            // Arrange
            var mockResult = new LinkageResult.SuccessfullyCreated(_fixture.Create<LinkageResponse>());
            var mockLinkageService = MockLinkageService(mockResult);
            var gpSystemMock = MockGpSystem(mockLinkageService);
            var request = CreateLinkageRequest();

            _mockMinimumAgeValidator.Setup(x => x.IsValid(It.IsAny<DateTime>(), MinimumLinkageAge)).Returns(false);

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(request, gpSystemMock.Object);

            // Assert
            result.Should().BeOfType<LinkageResult.ErrorCase>().Subject
                .ErrorCode.Should().Be(InternalCode.UnderMinimumAgeOrNonCompetent);
        }

        [TestMethod]
        public async Task Create_WhenDateOfBirthIsNull_ReturnsPatientNonCompetentOrUnderMinimumAge()
        {
            // Arrange
            var mockResult = new LinkageResult.SuccessfullyCreated(_fixture.Create<LinkageResponse>());
            var mockLinkageService = MockLinkageService(mockResult);
            var gpSystemMock = MockGpSystem(mockLinkageService);
            var request = CreateLinkageRequest();
            request.DateOfBirth = null;

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(request, gpSystemMock.Object);

            // Assert
            result.Should().BeOfType<LinkageResult.ErrorCase>().Subject
                .ErrorCode.Should().Be(InternalCode.UnderMinimumAgeOrNonCompetent);
        }

        [DataTestMethod]
        [DataRow(InternalCode.InvalidProviderId)]
        [DataRow(InternalCode.PatientNotRegisteredAtThisPractice)]
        public async Task Post_ReturnsErrorCodesWhenGivenErrorCodes(InternalCode errorCode)
        {
            // Arrange
            var mockResult = new LinkageResult.ErrorCase(errorCode);
            var mockLinkageService = MockLinkageService(mockResult);
            var gpSystemMock = MockGpSystem(mockLinkageService);
            var request = CreateLinkageRequest();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(request, gpSystemMock.Object);

            // Assert
            result.Should().BeOfType<LinkageResult.ErrorCase>().Subject
                .ErrorCode.Should().Be(errorCode);
        }

        [TestMethod]
        public async Task Post_ReturnsNotFoundWhenNotFound()
        {
            // Arrange
            var mockResult = new LinkageResult.NotFound(InternalCode.UnknownError);
            var mockLinkageService = MockLinkageService(mockResult);
            var gpSystemMock = MockGpSystem(mockLinkageService);
            var request = CreateLinkageRequest();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(request, gpSystemMock.Object);

            // Assert
            result.Should().BeOfType<LinkageResult.NotFound>().Subject
                .ErrorCode.Should().Be(InternalCode.UnknownError);
        }

        private static CreateLinkageRequest CreateLinkageRequest()
        {
            var request = new CreateLinkageRequest
            {
                NhsNumber = DefaultNhsNumber,
                OdsCode = DefaultOdsCode,
                Surname = DefaultSurname,
                DateOfBirth = DefaultDateOfBirth,
                IdentityToken = DefaultIdentityToken,
                EmailAddress = DefaultEmailAddress,
            };
            return request;
        }
        
        private static Mock<IGpSystem> MockGpSystem(Mock<ILinkageService> linkageService = null)
        {
            linkageService = linkageService ?? MockLinkageService();

            var mockGpSystem = new Mock<IGpSystem>();
            mockGpSystem.Setup(x => x.GetLinkageService()).Returns(linkageService.Object);

            return mockGpSystem;
        }  
        
        private static Mock<ILinkageService> MockLinkageService(LinkageResult expectedResult = null)
        {
            var mockLinkageService = new Mock<ILinkageService>();
            
            mockLinkageService.Setup(x => x.CreateLinkageKey(
                    It.Is<CreateLinkageRequest>(req => req.NhsNumber.Equals(DefaultNhsNumber, StringComparison.Ordinal) &&
                                                       req.Surname.Equals(DefaultSurname, StringComparison.Ordinal) &&
                                                       req.DateOfBirth.Equals(DefaultDateOfBirth) &&
                                                       req.OdsCode.Equals(DefaultOdsCode, StringComparison.Ordinal) &&
                                                       req.IdentityToken.Equals(DefaultIdentityToken, StringComparison.Ordinal) &&
                                                       req.EmailAddress.Equals(DefaultEmailAddress, StringComparison.Ordinal))))
                .ReturnsAsync(expectedResult);

            return mockLinkageService;
        }
    }
}
