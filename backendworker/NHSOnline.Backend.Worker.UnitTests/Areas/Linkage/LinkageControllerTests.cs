using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Linkage;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.Areas.Session;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Linkage
{
    [TestClass]
    public class LinkageControllerTests
    {
        private const string DefaultOdsCode = "AB1234";
        private const string DefaultNhsNumber = "XX00000A";
        private const string DefaultSurname = "Surname";
        private const Supplier DefaultSupplier = Supplier.Emis;
        private static readonly DateTime? DefaultDateOfBirth = new DateTime(1980,1,1);
        private const string DefaultIdentityToken = "IDTOKEN";
        private const string DefaultEmailAddress = "john@email.com";
        private Mock<IAuditor> _mockAuditor;
        private IFixture _fixture;
        private Mock<IMinimumAgeValidator> _mockMinimumAgeValidator;
        private Mock<IOptions<ConfigurationSettings>> _settings;
        private const int MinimumLinkageAge = 16;
        
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

            _settings = _fixture.Freeze<Mock<IOptions<ConfigurationSettings>>>();
            _settings
                .Setup(x => x.Value)
                .Returns(new ConfigurationSettings()
                {
                    MinimumLinkageAge = MinimumLinkageAge,
                });
            _mockMinimumAgeValidator = _fixture.Freeze<Mock<IMinimumAgeValidator>>();
            _mockMinimumAgeValidator.Setup(x => x.IsValid(It.IsAny<DateTime>(), It.IsAny<int>())).Returns(true);
        }
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Get_ReturnsABadRequestResult_WhenTheOdsCodeIsNullOrEmpty(string odsCode)
        {
            LinkageController linkageController = CreateLinkageController();
            
            var result = await linkageController.Get(DefaultNhsNumber, DefaultSurname, DefaultDateOfBirth.Value, odsCode, DefaultIdentityToken);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Get_UnknownOdsCode_Returns501NotImplemented()
        {
            // Arrange
            const string nhsNumber = DefaultNhsNumber;
            const string odsCode = DefaultOdsCode;
            DateTime? dateOfBirth = DefaultDateOfBirth;
            const string surname = DefaultSurname;
            const string identityToken = DefaultIdentityToken;
            
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(DefaultOdsCode))
                .Returns(Task.FromResult(Option.None<Supplier>()));

            LinkageController linkageController = CreateLinkageController(mockOdsCodeLookup);

            // Act
            var result = await linkageController.Get(nhsNumber, surname, dateOfBirth.Value, odsCode, identityToken);

            // Assert
            var resultAsStatusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            resultAsStatusCodeResult.StatusCode.Should().Be(StatusCodes.Status501NotImplemented);
        }

        [TestMethod]
        public async Task Get_ReturnsTheSuccessResponse_WhenServiceIsSuccessfullyCalled()
        {
            const string nhsNumber = DefaultNhsNumber;
            const string odsCode = DefaultOdsCode;
            DateTime? dateOfBirth = DefaultDateOfBirth;
            const string surname = DefaultSurname;
            const string identityToken = DefaultIdentityToken;
            
            var expectedResponse = _fixture.Create<LinkageResponse>();

            var linkageService = MockLinkageService(new LinkageResult.SuccessfullyRetrieved(expectedResponse));

            var mockLinkageValidationService = MockLinkageValidationService(true);
            
            var gpSystemMock = MockGpSystem(linkageService, mockLinkageValidationService);
            var gpSystemFactoryMock = MockGpSystemFactory(gpSystemMock);

            LinkageController linkageController = CreateLinkageController(gpSystemFactoryMock: gpSystemFactoryMock);

            // Act
            var result = await linkageController.Get(nhsNumber, surname, dateOfBirth.Value, odsCode, identityToken);

            // Assert
            linkageService.Verify(x => x.GetLinkageKey(
                It.Is<GetLinkageRequest>(req => req.NhsNumber.Equals(nhsNumber, StringComparison.Ordinal) &&
                                                req.Surname.Equals(surname, StringComparison.Ordinal) &&
                                                req.DateOfBirth.Equals(dateOfBirth) &&
                                                req.OdsCode.Equals(odsCode, StringComparison.Ordinal) &&
                                                req.IdentityToken.Equals(identityToken, StringComparison.Ordinal))), Times.Once);
            
            var resultValue = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = resultValue.Should().BeAssignableTo<LinkageResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockAuditor.Verify(x => x.AuditWithExplicitNhsNumber(It.IsAny<string>(), It.IsAny<Supplier>(), GetRequestAuditType, It.IsAny<string>()));
            _mockAuditor.Verify(x => x.AuditWithExplicitNhsNumber(It.IsAny<string>(), It.IsAny<Supplier>(), GetResponseAuditType, It.IsAny<string>()));
        }

        [TestMethod]
        public async Task Get_ReturnsABadRequestResult_WhenRequestValidationFails()
        {
            LinkageController linkageController = CreateLinkageController();

            // Act
            var result = await linkageController.Get(DefaultNhsNumber, DefaultSurname, DefaultDateOfBirth.Value,
                DefaultOdsCode, DefaultIdentityToken);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
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
                Surname = DefaultSurname,
                DateOfBirth = DefaultDateOfBirth,
                OdsCode = odsCode,
                IdentityToken = DefaultIdentityToken,
                EmailAddress = DefaultEmailAddress,
            };
            
            LinkageController linkageController = CreateLinkageController();
            
            // Act
            var result = await linkageController.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }
        
        [TestMethod]
        public async Task Post_ReturnsABadRequestResult_WhenRequestValidationFails()
        {
            // Arrange
            var request = _fixture.Create<CreateLinkageRequest>();
            request.OdsCode = DefaultOdsCode;
            
            LinkageController linkageController = CreateLinkageController();

            // Act
            var result = await linkageController.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Post_UnknownOdsCode_Returns501NotImplemented()
        {
            // Arrange
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(DefaultOdsCode))
                .Returns(Task.FromResult(Option.None<Supplier>()));
            
            LinkageController linkageController = CreateLinkageController(mockOdsCodeLookup);
            
            // Act
            var result = await linkageController.Get(DefaultNhsNumber, DefaultSurname, DefaultDateOfBirth.Value, DefaultOdsCode, DefaultIdentityToken);

            // Assert
            var resultAsStatusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            resultAsStatusCodeResult.StatusCode.Should().Be(StatusCodes.Status501NotImplemented);
        }

        [TestMethod]
        public async Task Post_ReturnsTheSuccessResponse_WhenServiceIsSuccessfullyCalled()
        {
            const string nhsNumber = DefaultNhsNumber;
            const string odsCode = DefaultOdsCode;
            DateTime? dateOfBirth = DefaultDateOfBirth;
            const string surname = DefaultSurname;
            const string identityToken = DefaultIdentityToken;
            const string emailAddress = DefaultEmailAddress;

            var expectedResponse = _fixture.Create<LinkageResponse>();

            var mockResult = new LinkageResult.SuccessfullyRetrieved(expectedResponse);

            var mockLinkageService = new Mock<ILinkageService>();
            
            mockLinkageService.Setup(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req => req.NhsNumber.Equals(nhsNumber, StringComparison.Ordinal) &&
                                                   req.Surname.Equals(surname, StringComparison.Ordinal) &&
                                                   req.DateOfBirth.Equals(dateOfBirth) &&
                                                   req.OdsCode.Equals(odsCode, StringComparison.Ordinal) &&
                                                   req.IdentityToken.Equals(identityToken, StringComparison.Ordinal) &&
                                                   req.EmailAddress.Equals(emailAddress, StringComparison.Ordinal)))
            ).ReturnsAsync(mockResult);

            var mockLinkageValidationService = MockLinkageValidationService(true);

            var gpSystemMock = MockGpSystem(mockLinkageService, mockLinkageValidationService);
            var gpSystemFactoryMock = MockGpSystemFactory(gpSystemMock);

            var request = new CreateLinkageRequest
            {
                NhsNumber = nhsNumber,
                OdsCode = odsCode,
                Surname = surname,
                DateOfBirth = dateOfBirth,
                IdentityToken = identityToken,
                EmailAddress = emailAddress,
            };
            
            LinkageController linkageController = CreateLinkageController(gpSystemFactoryMock: gpSystemFactoryMock);

            // Act
            var result = await linkageController.Post(request);

            // Assert
            mockLinkageService.Verify(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req => req.NhsNumber.Equals(nhsNumber, StringComparison.Ordinal) &&
                                                   req.Surname.Equals(surname, StringComparison.Ordinal) &&
                                                   req.DateOfBirth.Equals(dateOfBirth) &&
                                                   req.OdsCode.Equals(odsCode, StringComparison.Ordinal) &&
                                                   req.IdentityToken.Equals(identityToken, StringComparison.Ordinal) &&
                                                   req.EmailAddress.Equals(emailAddress, StringComparison.Ordinal))), Times.Once);
            
            var resultValue = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = resultValue.Should().BeAssignableTo<LinkageResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockAuditor.Verify(x => x.AuditWithExplicitNhsNumber(It.IsAny<string>(), It.IsAny<Supplier>(), PostRequestAuditType, It.IsAny<string>()));
            _mockAuditor.Verify(x => x.AuditWithExplicitNhsNumber(It.IsAny<string>(), It.IsAny<Supplier>(), PostResponseAuditType, It.IsAny<string>()));
        }
        
        [TestMethod]
        public async Task Post_Returns403FailedAgeCheck_WhenDateOfBirthIsUnderTheMinimumLinkageAge()
        {
            const string nhsNumber = DefaultNhsNumber;
            const string odsCode = DefaultOdsCode;
            DateTime? dateOfBirth = DefaultDateOfBirth;
            const string surname = DefaultSurname;
            const string identityToken = DefaultIdentityToken;
            const string emailAddress = DefaultEmailAddress;

            var expectedResponse = _fixture.Create<LinkageResponse>();

            var mockResult = new LinkageResult.SuccessfullyRetrieved(expectedResponse);

            var mockLinkageService = new Mock<ILinkageService>();
            
            mockLinkageService.Setup(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req => req.NhsNumber.Equals(nhsNumber, StringComparison.Ordinal) &&
                                                   req.Surname.Equals(surname, StringComparison.Ordinal) &&
                                                   req.DateOfBirth.Equals(dateOfBirth) &&
                                                   req.OdsCode.Equals(odsCode, StringComparison.Ordinal) &&
                                                   req.IdentityToken.Equals(identityToken, StringComparison.Ordinal) &&
                                                   req.EmailAddress.Equals(emailAddress, StringComparison.Ordinal)))
            ).ReturnsAsync(mockResult);

            var mockLinkageValidationService = MockLinkageValidationService(true);

            var gpSystemMock = MockGpSystem(mockLinkageService, mockLinkageValidationService);
            var gpSystemFactoryMock = MockGpSystemFactory(gpSystemMock);

            var request = new CreateLinkageRequest
            {
                NhsNumber = nhsNumber,
                OdsCode = odsCode,
                Surname = surname,
                DateOfBirth = dateOfBirth,
                IdentityToken = identityToken,
                EmailAddress = emailAddress,
            };
            
            _mockMinimumAgeValidator.Setup(x => x.IsValid(It.IsAny<DateTime>(), MinimumLinkageAge)).Returns(false);
            
            LinkageController linkageController = CreateLinkageController(gpSystemFactoryMock: gpSystemFactoryMock);

            // Act
            var result = await linkageController.Post(request);

            // Assert
            var resultAsStatusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            resultAsStatusCodeResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }

        [DataTestMethod]
        [DataRow(typeof(LinkageResult.PatientNonCompetentOrUnderMinimumAge))]
        [DataRow(typeof(LinkageResult.AccountStatusInvalid))]
        [DataRow(typeof(LinkageResult.PatientMarkedAsArchived))]
        public async Task Post_Returns403Forbidden_ForVariousResults(Type resultType)
        {
            var linkageResult = (LinkageResult)Activator.CreateInstance(resultType);
            
            var linkageController = CreateLinkageController(linkageResult);
        
            var request = CreateLinkageRequest();
        
            // Act
            var actionResult = await linkageController.Post(request);
        
            // Assert                 
            var resultAsStatusCodeResult = actionResult.Should().BeAssignableTo<StatusCodeResult>().Subject;
            resultAsStatusCodeResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }
        
        private static CreateLinkageRequest CreateLinkageRequest()
        {
            var request = new CreateLinkageRequest
            {
                NhsNumber = DefaultNhsNumber,
                OdsCode = DefaultOdsCode,
                Surname = DefaultSurname,
                DateOfBirth = DefaultDateOfBirth,
                IdentityToken = DefaultEmailAddress,
                EmailAddress = DefaultEmailAddress,
            };
            return request;
        }
        
        
        private LinkageController CreateLinkageController(LinkageResult mockResult)
        {
            var mockLinkageService = MockLinkageService(mockResult);
        
            mockLinkageService.Setup(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req => req.NhsNumber.Equals(DefaultNhsNumber, StringComparison.Ordinal) &&
                                                   req.Surname.Equals(DefaultSurname, StringComparison.Ordinal) &&
                                                   req.DateOfBirth.Equals(DefaultDateOfBirth) &&
                                                   req.OdsCode.Equals(DefaultOdsCode, StringComparison.Ordinal) &&
                                                   req.IdentityToken.Equals(DefaultEmailAddress, StringComparison.Ordinal) &&
                                                   req.EmailAddress.Equals(DefaultEmailAddress, StringComparison.Ordinal)))
            ).ReturnsAsync(mockResult);
        
            var mockLinkageValidationService = MockLinkageValidationService(true);
        
            var gpSystemMock = MockGpSystem(mockLinkageService, mockLinkageValidationService);
            var gpSystemFactoryMock = MockGpSystemFactory(gpSystemMock);
        
            var linkageController = CreateLinkageController(gpSystemFactoryMock: gpSystemFactoryMock);
            return linkageController;
        }

        private LinkageController CreateLinkageController(
            Mock<IOdsCodeLookup> odsCodeLookupMock = null,
            Mock<IGpSystemFactory> gpSystemFactoryMock = null)
        {
            odsCodeLookupMock = odsCodeLookupMock ?? MockOdsCodeLookup();
            gpSystemFactoryMock = gpSystemFactoryMock ??
                                  MockGpSystemFactory();
            var logger = new LoggerFactory();

            return new LinkageController(logger, gpSystemFactoryMock.Object, odsCodeLookupMock.Object, _mockAuditor.Object, _mockMinimumAgeValidator.Object, _settings.Object);
        }

        private static Mock<IOdsCodeLookup> MockOdsCodeLookup(
            Supplier supplier = DefaultSupplier,
            string odsCode = DefaultOdsCode)
        {
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(odsCode)).Returns(Task.FromResult(Option.Some(supplier)));
            return mockOdsCodeLookup;
        }

        private static Mock<IGpSystemFactory> MockGpSystemFactory(
            Mock<IGpSystem> gpSystemMock = null)
        {
            gpSystemMock = gpSystemMock ?? MockGpSystem();
            var mockGpSystemFactory = new Mock<IGpSystemFactory>();
            mockGpSystemFactory.Setup(x => x.CreateGpSystem(DefaultSupplier)).Returns(gpSystemMock.Object);

            return mockGpSystemFactory;
        }

        private static Mock<IGpSystem> MockGpSystem(
            Mock<ILinkageService> linkageService = null,
            Mock<ILinkageRequestValidationService> linkageValidationService = null)
        {
            linkageService = linkageService ?? MockLinkageService();
            linkageValidationService = linkageValidationService ??
                                       MockLinkageValidationService();

            var mockGpSystem = new Mock<IGpSystem>();
            mockGpSystem.Setup(x => x.GetLinkageService()).Returns(linkageService.Object);
            mockGpSystem.Setup(x => x.GetLinkageRequestValidationService()).Returns(linkageValidationService.Object);

            return mockGpSystem;
        }

        private static Mock<ILinkageService> MockLinkageService(LinkageResult expectedResult = null)
        {
            var mockLinkageService = new Mock<ILinkageService>();
            
            mockLinkageService.Setup(x => x.GetLinkageKey(
                It.Is<GetLinkageRequest>(req => req.NhsNumber.Equals(DefaultNhsNumber, StringComparison.Ordinal) &&
                                                req.Surname.Equals(DefaultSurname, StringComparison.Ordinal) &&
                                                req.DateOfBirth.Equals(DefaultDateOfBirth) &&
                                                req.OdsCode.Equals(DefaultOdsCode, StringComparison.Ordinal) &&
                                                req.IdentityToken.Equals(DefaultIdentityToken, StringComparison.Ordinal))))
                .ReturnsAsync(expectedResult);

            return mockLinkageService;
        }
        
        private static Mock<ILinkageRequestValidationService> MockLinkageValidationService(bool validationResult = false)
        {
            var mockLinkageValidationService = new Mock<ILinkageRequestValidationService>();

            mockLinkageValidationService.Setup(x => x.Validate(It.IsAny<GetLinkageRequest>()))
                .Returns(validationResult);
            
            mockLinkageValidationService.Setup(x => x.Validate(It.IsAny<CreateLinkageRequest>())
            ).Returns(validationResult);

            return mockLinkageValidationService;
        }
    }
}
