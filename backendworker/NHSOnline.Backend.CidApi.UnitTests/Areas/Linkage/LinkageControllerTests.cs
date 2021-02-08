using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Temporal;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.CidApi.Areas.Linkage;
using NHSOnline.Backend.GpSystems.Im1Connection;
using UnitTestHelper;

namespace NHSOnline.Backend.CidApi.UnitTests.Areas.Linkage
{
    [TestClass]
    public class LinkageControllerTests
    {
        private const string DefaultOdsCode = "AB1234";
        private const string DefaultNhsNumber = "123 456 7890";
        private const string DefaultNhsNumberWithoutWhitespace = "1234567890";
        private const string DefaultSurname = "Surname";
        private static readonly DateTime? DefaultDateOfBirth = new DateTime(1980,1,1);
        private const string DefaultIdentityToken = "IDTOKEN";
        private const string DefaultEmailAddress = "john@email.com";
        private Mock<IAuditor> _mockAuditor;
        private IFixture _fixture;
        private Mock<IMinimumAgeValidator> _mockMinimumAgeValidator;
        private ConfigurationSettings _settings;
        private Mock<IOdsCodeMassager> _odsCodeMassager;
        private const int MinimumLinkageAge = 16;

        private const string CookieDomain = "CookieDomain";
        private const int PrescriptionsDefaultLastNumberMonthsToDisplay = 12;
        private const int DefaultSessionExpiryMinutes  = 10;
        private const int DefaultHttpTimeoutSeconds = 6;
        private const int MinimumAppAge = 16;
        private const string GetRequestAuditType = "Linkage_GetDetails_Request";
        private const string GetResponseAuditType = "Linkage_GetDetails_Response";
        private const string PostRequestAuditType = "Linkage_CreateKey_Request";
        private const string PostResponseAuditType = "Linkage_CreateKey_Response";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            _settings = new ConfigurationSettings(
                CookieDomain,
                PrescriptionsDefaultLastNumberMonthsToDisplay,
                DefaultSessionExpiryMinutes,
                DefaultHttpTimeoutSeconds,
                MinimumAppAge,
                MinimumLinkageAge);

            _mockMinimumAgeValidator = _fixture.Freeze<Mock<IMinimumAgeValidator>>();
            _mockMinimumAgeValidator.Setup(x => x.IsValid(It.IsAny<DateTime>(), It.IsAny<int>())).Returns(true);
            _odsCodeMassager = _fixture.Freeze<Mock<IOdsCodeMassager>>();
            _odsCodeMassager.Setup(x => x.CheckOdsCode(DefaultOdsCode)).Returns(DefaultOdsCode);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Get_ReturnsABadRequestResult_WhenTheOdsCodeIsNullOrEmpty(string odsCode)
        {
            // Arrange
            var linkageController = CreateLinkageController();

            // Act
            var result = await linkageController.Get(DefaultNhsNumber, DefaultSurname, DefaultDateOfBirth.Value, odsCode, DefaultIdentityToken);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task Get_UnknownOdsCode_Returns501NotImplemented()
        {
            // Arrange
            _odsCodeMassager.Setup(x => x.IsEnabled).Returns(true);

            var gpSystemResolver = _fixture.Create<Mock<IGpSystemResolver>>();
            gpSystemResolver.Setup(x => x.ResolveFromOdsCode(It.IsAny<string>())).ReturnsAsync(Option.None<IGpSystem>());

            var logger = _fixture.Create<Mock<ILogger<LinkageController>>>();

            var linkageController = new LinkageController(
                logger.Object,
                _mockAuditor.Object,
                _mockMinimumAgeValidator.Object,
                _settings,
                _odsCodeMassager.Object,
                gpSystemResolver.Object);

            // Act
            var result = await linkageController.Get(DefaultNhsNumber, DefaultSurname, DefaultDateOfBirth.Value, DefaultOdsCode, DefaultIdentityToken);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status501NotImplemented);
        }

        [TestMethod]
        public async Task Get_ReturnsTheSuccessResponse_WhenServiceIsSuccessfullyCalled()
        {
            // Arrange
            var expectedResponse = _fixture.Create<LinkageResponse>();
            _odsCodeMassager.Setup(x => x.IsEnabled).Returns(true);

            var mockResult = new LinkageResult.SuccessfullyRetrieved(expectedResponse);
            var mockLinkageService = new Mock<ILinkageService>();
            mockLinkageService.Setup(x => x.GetLinkageKey(
                    It.IsAny<GetLinkageRequest>())).ReturnsAsync(mockResult).Verifiable();

            var gpSystem = _fixture.Create <Mock<IGpSystem>>();
            gpSystem.Setup(x => x.GetLinkageService()).Returns(mockLinkageService.Object);

            var mockLinkageValidationService = new Mock<ILinkageValidationService>();
            mockLinkageValidationService.Setup(x => x.IsGetValid(It.IsAny<GetLinkageRequest>())).Returns(true);
            gpSystem.Setup(x => x.GetLinkageValidationService()).Returns(mockLinkageValidationService.Object);

            var gpSystemResolver = _fixture.Create<Mock<IGpSystemResolver>>();
            gpSystemResolver.Setup(x => x.ResolveFromOdsCode(It.IsAny<string>())).ReturnsAsync(Option.Some(gpSystem.Object));

            var logger = _fixture.Create<Mock<ILogger<LinkageController>>>();

            var linkageController = new LinkageController(
                logger.Object,
                _mockAuditor.Object,
                _mockMinimumAgeValidator.Object,
                _settings,
                _odsCodeMassager.Object,
                gpSystemResolver.Object);

            // Act
            var result = await linkageController.Get(DefaultNhsNumber, DefaultSurname, DefaultDateOfBirth.Value, DefaultOdsCode, DefaultIdentityToken);

            // Assert
            mockLinkageService.Verify(x => x.GetLinkageKey(
                 It.Is<GetLinkageRequest>(req => req.NhsNumber.Equals(DefaultNhsNumber, StringComparison.Ordinal) &&
                                                 req.Surname.Equals(DefaultSurname, StringComparison.Ordinal) &&
                                                 req.DateOfBirth.Equals(DefaultDateOfBirth) &&
                                                 req.OdsCode.Equals(DefaultOdsCode, StringComparison.Ordinal) &&
                                                 req.IdentityToken.Equals(DefaultIdentityToken, StringComparison.Ordinal))), Times.Once);

            logger.VerifyLogger(LogLevel.Information, $"Retrieve LinkageKey for NhsNumber={DefaultNhsNumberWithoutWhitespace}", Times.Once());

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<LinkageResponse>()
                .Subject.Should().BeEquivalentTo(expectedResponse);
            _mockAuditor.Verify(x => x.PreOperationAuditRegistrationEvent(It.IsAny<string>(), It.IsAny<Supplier>(), GetRequestAuditType, It.IsAny<string>()));
            _mockAuditor.Verify(x => x.PostOperationAuditRegistrationEvent(It.IsAny<string>(), It.IsAny<Supplier>(), GetResponseAuditType, It.IsAny<string>()));
        }

        [TestMethod]
        public async Task Get_ReturnsABadRequestResult_WhenRequestValidationFails()
        {
            var expectedResponse = _fixture.Create<LinkageResponse>();
            _odsCodeMassager.Setup(x => x.IsEnabled).Returns(true);

            var mockResult = new LinkageResult.SuccessfullyRetrieved(expectedResponse);
            var mockLinkageService = new Mock<ILinkageService>();
            mockLinkageService.Setup(x => x.GetLinkageKey(It.IsAny<GetLinkageRequest>())).ReturnsAsync(mockResult).Verifiable();

            var gpSystem = _fixture.Create<Mock<IGpSystem>>();
            gpSystem.Setup(x => x.GetLinkageService()).Returns(mockLinkageService.Object);

            var mockLinkageValidationService = new Mock<ILinkageValidationService>();
            mockLinkageValidationService.Setup(x => x.IsGetValid(It.IsAny<GetLinkageRequest>())).Returns(false);

            var gpSystemResolver = _fixture.Create<Mock<IGpSystemResolver>>();
            gpSystemResolver.Setup(x => x.ResolveFromOdsCode(It.IsAny<string>())).ReturnsAsync(Option.Some(gpSystem.Object));

            var logger = _fixture.Create<Mock<ILogger<LinkageController>>>();

            var linkageController = new LinkageController(
                logger.Object,
                _mockAuditor.Object,
                _mockMinimumAgeValidator.Object,
                _settings,
                _odsCodeMassager.Object,
                gpSystemResolver.Object);

            // Act
            var result = await linkageController.Get(DefaultNhsNumber, DefaultSurname, DefaultDateOfBirth.Value,
                DefaultOdsCode, DefaultIdentityToken);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_ReturnsABadRequestResult_WhenTheOdsCodeIsNullOrEmpty(string odsCode)
        {
            // Arrange
            var request = new CreateLinkageRequest
            {
                NhsNumber = DefaultNhsNumber,
                OdsCode = odsCode,
                Surname = DefaultSurname,
                DateOfBirth = DefaultDateOfBirth,
                IdentityToken = DefaultIdentityToken,
                EmailAddress = DefaultEmailAddress,
            };

            var linkageController = CreateLinkageController();

            // Act
            var result = await linkageController.Post(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_ReturnsABadRequestResult_WhenRequestValidationFails()
        {
            // Arrange

            var request = BuildCreateLinkageRequest();

            var gpSystem = _fixture.Create<Mock<IGpSystem>>();

            var mockLinkageValidationService = new Mock<ILinkageValidationService>();
            mockLinkageValidationService.Setup(x => x.IsPostValid(It.IsAny<CreateLinkageRequest>())).Returns(false);
            gpSystem.Setup(x => x.GetLinkageValidationService()).Returns(mockLinkageValidationService.Object);

            var mockgpSystemResolver = _fixture.Create<Mock<IGpSystemResolver>>();
            mockgpSystemResolver.Setup(x => x.ResolveFromOdsCode(It.IsAny<string>())).ReturnsAsync(Option.Some(gpSystem.Object));
            var logger = _fixture.Create<Mock<ILogger<LinkageController>>>();
            _odsCodeMassager.Setup(x => x.IsEnabled).Returns(true);

            var linkageController = new LinkageController(
                logger.Object,
                _mockAuditor.Object,
                _mockMinimumAgeValidator.Object,
                _settings,
                _odsCodeMassager.Object,
                mockgpSystemResolver.Object);

            // Act
            var result = await linkageController.Post(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_UnknownOdsCode_Returns501NotImplemented()
        {
            // Arrange
            var request = BuildCreateLinkageRequest();

            var mockGpSystemResolver = _fixture.Create<Mock<IGpSystemResolver>>();
            mockGpSystemResolver.Setup(x => x.ResolveFromOdsCode(It.IsAny<string>())).ReturnsAsync(Option.None<IGpSystem>());
            var logger = _fixture.Create<Mock<ILogger<LinkageController>>>();
            _odsCodeMassager.Setup(x => x.IsEnabled).Returns(true);

            var linkageController = new LinkageController(
                logger.Object,
                _mockAuditor.Object,
                _mockMinimumAgeValidator.Object,
                _settings,
                _odsCodeMassager.Object,
                mockGpSystemResolver.Object);

            // Act
            var result = await linkageController.Post(request);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status501NotImplemented);
        }

        [TestMethod]
        public async Task Post_ReturnsTheSuccessResponse_WhenServiceIsSuccessfullyCalled()
        {
            var expectedResponse = _fixture.Create<LinkageResponse>();
            var mockResult = new LinkageResult.SuccessfullyCreated(expectedResponse);
            var mockLinkageService = new Mock<ILinkageService>();

            mockLinkageService.Setup(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req => req.NhsNumber.Equals(DefaultNhsNumber, StringComparison.Ordinal) &&
                                                   req.Surname.Equals(DefaultSurname, StringComparison.Ordinal) &&
                                                   req.DateOfBirth.Equals(DefaultDateOfBirth) &&
                                                   req.OdsCode.Equals(DefaultOdsCode, StringComparison.Ordinal) &&
                                                   req.IdentityToken.Equals(DefaultIdentityToken, StringComparison.Ordinal) &&
                                                   req.EmailAddress.Equals(DefaultEmailAddress, StringComparison.Ordinal)))
            ).ReturnsAsync(mockResult);

            var request = BuildCreateLinkageRequest();

            var gpSystem = _fixture.Create<Mock<IGpSystem>>();

            var validationService = MockLinkageValidationService(true);
            gpSystem.Setup(x => x.GetLinkageValidationService()).Returns(validationService.Object);
            gpSystem.Setup(x => x.GetLinkageService()).Returns(mockLinkageService.Object);

            var mockGpSystemResolver = _fixture.Create<Mock<IGpSystemResolver>>();
            mockGpSystemResolver.Setup(x => x.ResolveFromOdsCode(It.IsAny<string>())).ReturnsAsync(Option.Some(gpSystem.Object));
            var logger = _fixture.Create<Mock<ILogger<LinkageController>>>();
            _odsCodeMassager.Setup(x => x.IsEnabled).Returns(true);

            var linkageController = new LinkageController(
                logger.Object,
                _mockAuditor.Object,
                _mockMinimumAgeValidator.Object,
                _settings,
                _odsCodeMassager.Object,
                mockGpSystemResolver.Object);

            // Act
            var result = await linkageController.Post(request);

            // Assert
            mockLinkageService.Verify(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req => req.NhsNumber.Equals(DefaultNhsNumber, StringComparison.Ordinal) &&
                                                   req.Surname.Equals(DefaultSurname, StringComparison.Ordinal) &&
                                                   req.DateOfBirth.Equals(DefaultDateOfBirth) &&
                                                   req.OdsCode.Equals(DefaultOdsCode, StringComparison.Ordinal) &&
                                                   req.IdentityToken.Equals(DefaultIdentityToken, StringComparison.Ordinal) &&
                                                   req.EmailAddress.Equals(DefaultEmailAddress, StringComparison.Ordinal))), Times.Once);

            logger.VerifyLogger(LogLevel.Information, $"Create LinkageKey for NhsNumber={DefaultNhsNumberWithoutWhitespace}", Times.Once());

            result.Should().BeAssignableTo<CreatedResult>()
                .Subject.Value.Should().BeAssignableTo<LinkageResponse>()
                .Subject.Should().BeEquivalentTo(expectedResponse);
            _mockAuditor.Verify(x => x.PreOperationAuditRegistrationEvent(It.IsAny<string>(), It.IsAny<Supplier>(), PostRequestAuditType, It.IsAny<string>()));
            _mockAuditor.Verify(x => x.PostOperationAuditRegistrationEvent(It.IsAny<string>(), It.IsAny<Supplier>(), PostResponseAuditType, It.IsAny<string>()));
        }

        [TestMethod]
        public async Task Post_Returns403FailedAgeCheck_WhenDateOfBirthIsUnderTheMinimumLinkageAge()
        {
            // Arrange
            var expectedResponse = _fixture.Create<LinkageResponse>();

            var mockResult = new LinkageResult.SuccessfullyRetrieved(expectedResponse);

            var mockLinkageService = new Mock<ILinkageService>();

            mockLinkageService.Setup(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req => req.NhsNumber.Equals(DefaultNhsNumber, StringComparison.Ordinal) &&
                                                   req.Surname.Equals(DefaultSurname, StringComparison.Ordinal) &&
                                                   req.DateOfBirth.Equals(DefaultDateOfBirth) &&
                                                   req.OdsCode.Equals(DefaultOdsCode, StringComparison.Ordinal) &&
                                                   req.IdentityToken.Equals(DefaultIdentityToken, StringComparison.Ordinal) &&
                                                   req.EmailAddress.Equals(DefaultEmailAddress, StringComparison.Ordinal)))
            ).ReturnsAsync(mockResult);

            var request = BuildCreateLinkageRequest();

            _mockMinimumAgeValidator.Setup(x => x.IsValid(It.IsAny<DateTime>(), MinimumLinkageAge)).Returns(false);

            var linkageController = CreateLinkageController();

            // Act
            var result = await linkageController.Post(request);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }

       [DataTestMethod]
       [DataRow(Im1ConnectionErrorCodes.InternalCode.UnderMinimumAgeOrNonCompetent)]
       [DataRow(Im1ConnectionErrorCodes.InternalCode.InvalidProviderId)]
       [DataRow(Im1ConnectionErrorCodes.InternalCode.UserAccountAlreadyExistsWithPatientDemographicDetailsAndIsArchived)]
       [DataRow(Im1ConnectionErrorCodes.InternalCode.MultipleRecordsFoundWithNhsNumber)]
       public async Task Post_Returns403Forbidden_ForVariousResults(Im1ConnectionErrorCodes.InternalCode errorCode)
       {
           // Arrange
            var mockResult = new LinkageResult.ErrorCase(errorCode);
            var mockLinkageService = new Mock<ILinkageService>();

            mockLinkageService.Setup(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req => req.NhsNumber.Equals(DefaultNhsNumber, StringComparison.Ordinal) &&
                                                   req.Surname.Equals(DefaultSurname, StringComparison.Ordinal) &&
                                                   req.DateOfBirth.Equals(DefaultDateOfBirth) &&
                                                   req.OdsCode.Equals(DefaultOdsCode, StringComparison.Ordinal) &&
                                                   req.IdentityToken.Equals(DefaultIdentityToken, StringComparison.Ordinal) &&
                                                   req.EmailAddress.Equals(DefaultEmailAddress, StringComparison.Ordinal)))
            ).ReturnsAsync(mockResult);

            var request = BuildCreateLinkageRequest();

            var gpSystem = _fixture.Create<Mock<IGpSystem>>();

            var validationService = MockLinkageValidationService(true);
            gpSystem.Setup(x => x.GetLinkageValidationService()).Returns(validationService.Object);
            gpSystem.Setup(x => x.GetLinkageService()).Returns(mockLinkageService.Object);

            var mockGpSystemResolver = _fixture.Create<Mock<IGpSystemResolver>>();
            mockGpSystemResolver.Setup(x => x.ResolveFromOdsCode(It.IsAny<string>())).ReturnsAsync(Option.Some(gpSystem.Object));
            var logger = _fixture.Create<Mock<ILogger<LinkageController>>>();
            _odsCodeMassager.Setup(x => x.IsEnabled).Returns(true);

            var linkageController = new LinkageController(
                logger.Object,
                _mockAuditor.Object,
                _mockMinimumAgeValidator.Object,
                _settings,
                _odsCodeMassager.Object,
                mockGpSystemResolver.Object);

           // Act
           var actionResult = await linkageController.Post(request);

           // Assert
           actionResult.Should().BeAssignableTo<StatusCodeResult>()
               .Subject.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
       }

        private LinkageController CreateLinkageController(
            Mock<IGpSystemResolver> mockGpSystemResolver = null)
        {
            var gpSystem = _fixture.Create <Mock<IGpSystem>>();

            var validationService = MockLinkageValidationService(true);
            gpSystem.Setup(x => x.GetLinkageValidationService()).Returns(validationService.Object);

            var defaultGpSystemResolver = _fixture.Create<Mock<IGpSystemResolver>>();

            defaultGpSystemResolver.Setup(x => x.ResolveFromOdsCode(It.IsAny<string>())).ReturnsAsync(Option.Some(gpSystem.Object));
            var odsCodeLookup = mockGpSystemResolver ?? defaultGpSystemResolver;
            var logger = _fixture.Create<Mock<ILogger<LinkageController>>>();

            return new LinkageController(
                logger.Object,
                _mockAuditor.Object,
                _mockMinimumAgeValidator.Object,
                _settings,
                _odsCodeMassager.Object,
                odsCodeLookup.Object);
        }

        private static Mock<ILinkageValidationService> MockLinkageValidationService(bool validationResult = false)
        {
            var mockLinkageValidationService = new Mock<ILinkageValidationService>();

            mockLinkageValidationService.Setup(x => x.IsGetValid(It.IsAny<GetLinkageRequest>())).Returns(validationResult);

            mockLinkageValidationService.Setup(x => x.IsPostValid(It.IsAny<CreateLinkageRequest>())).Returns(validationResult);

            return mockLinkageValidationService;
        }

        private static CreateLinkageRequest BuildCreateLinkageRequest()
        {
            return new CreateLinkageRequest
            {
                NhsNumber = DefaultNhsNumber,
                OdsCode = DefaultOdsCode,
                Surname = DefaultSurname,
                DateOfBirth = DefaultDateOfBirth,
                IdentityToken = DefaultIdentityToken,
                EmailAddress = DefaultEmailAddress,
            };
        }
    }
}
