using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Linkage;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Linkage
{
    [TestClass]
    public class LinkageControllerTests
    {
        private const string DefaultOdsCode = "AB1234";
        private const Supplier DefaultSupplier = Supplier.Emis;
        private const string DefaultNhsNumber = "XX00000A";
        private const string DefaultIdentityToken = "IDTOKEN";
        private const string DefaultEmailAddress = "john@email.com";
        private Mock<IAuditor> _mockAuditor;
        private IFixture _fixture;
        private LinkageController _linkageController;
        
        private const string GetRequestAuditType = "Linkage_GetDetails_Request";
        private const string GetResponseAuditType = "Linkage_GetDetails_Response";
        private const string PostRequestAuditType = "Linkage_CreateKey_Request";
        private const string PostResponseAuditType = "Linkage_CreateKey_Response";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();
            _linkageController = CreateLinkageController();
        }
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Get_ReturnsABadRequestResult_WhenTheOdsCodeIsNullOrEmpty(string odsCode)
        {
            var result = await _linkageController.Get(DefaultNhsNumber, odsCode, DefaultIdentityToken);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Get_ReturnsABadRequestResult_WhenTheNhsNumberIsNullOrEmpty(string nhsNumber)
        {
            var result = await _linkageController.Get(nhsNumber, DefaultOdsCode, DefaultIdentityToken);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Get_ReturnsABadRequestResult_WhenTheIdentityTokenIsNullOrEmpty(string identityToken)
        {
            var result = await _linkageController.Get(DefaultNhsNumber, DefaultOdsCode, identityToken);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Get_UnknownOdsCode_Returns501NotImplemented()
        {
            // Arrage
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(DefaultOdsCode))
                .Returns(Task.FromResult(Option.None<Supplier>()));

            _linkageController = CreateLinkageController(mockOdsCodeLookup);

            // Act
            var result = await _linkageController.Get(DefaultNhsNumber, DefaultOdsCode, DefaultIdentityToken);

            // Assert
            var resultAsStatusCodeResult = result as StatusCodeResult;
            resultAsStatusCodeResult.Should().NotBeNull();
            resultAsStatusCodeResult.StatusCode.Should().Be(StatusCodes.Status501NotImplemented);
        }

        [TestMethod]
        public async Task Get_ReturnsTheSuccessResponse_WhenServiceIsSuccessfullyCalled()
        {
            const Supplier supplier = DefaultSupplier;

            var expectedResponse = _fixture.Create<LinkageResponse>();

            var linkageService = MockLinkageService(new LinkageResult.SuccessfullyRetrieved(expectedResponse));
            var gpSystemMock = MockGpSystem(linkageService);
            var gpSystemFactoryMock = MockGpSystemFactory(supplier, gpSystemMock);

            _linkageController = CreateLinkageController(gpSystemFactoryMock: gpSystemFactoryMock);

            // Act
            var result = await _linkageController.Get(DefaultNhsNumber, DefaultOdsCode, DefaultIdentityToken);

            // Assert
            linkageService.Verify(x => x.GetLinkageKey(DefaultNhsNumber, DefaultOdsCode, DefaultIdentityToken), Times.Once);
            var resultValue = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = resultValue.Should().BeAssignableTo<LinkageResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockAuditor.Verify(x => x.AuditWithExplicitNhsNumber(It.IsAny<string>(), It.IsAny<Supplier>(), GetRequestAuditType, It.IsAny<string>()));
            _mockAuditor.Verify(x => x.AuditWithExplicitNhsNumber(It.IsAny<string>(), It.IsAny<Supplier>(), GetResponseAuditType, It.IsAny<string>()));
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
                IdentityToken = DefaultIdentityToken,
                EmailAddress = DefaultEmailAddress,
            };

            // Act
            var result = await _linkageController.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_ReturnsABadRequestResult_WhenTheNhsNumberIsNullOrEmpty(string nhsNumber)
        {
            // Arrange
            var request = new CreateLinkageRequest
            {
                NhsNumber = nhsNumber,
                OdsCode = DefaultOdsCode,
                IdentityToken = DefaultIdentityToken,
                EmailAddress = DefaultEmailAddress,
            };

            // Act
            var result = await _linkageController.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_ReturnsABadRequestResult_WhenTheIdentityTokenIsNullOrEmpty(string identityToken)
        {
            // Arrange
            var request = new CreateLinkageRequest
            {
                NhsNumber = DefaultNhsNumber,
                OdsCode = DefaultOdsCode,
                IdentityToken = identityToken,
                EmailAddress = DefaultEmailAddress,
            };

            // Act
            var result = await _linkageController.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_ReturnsABadRequestResult_WhenTheEmailAddressIsNullOrEmpty(string emailAddress)
        {
            // Arrange
            var request = new CreateLinkageRequest
            {
                NhsNumber = DefaultNhsNumber,
                OdsCode = DefaultOdsCode,
                IdentityToken = DefaultIdentityToken,
                EmailAddress = emailAddress,
            };

            // Act
            var result = await _linkageController.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Post_UnknownOdsCode_Returns501NotImplemented()
        {
            // Arrage
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(DefaultOdsCode))
                .Returns(Task.FromResult(Option.None<Supplier>()));

            _linkageController = CreateLinkageController(mockOdsCodeLookup);

            // Act
            var result = await _linkageController.Get(DefaultNhsNumber, DefaultOdsCode, DefaultIdentityToken);

            // Assert
            var resultAsStatusCodeResult = result as StatusCodeResult;
            resultAsStatusCodeResult.Should().NotBeNull();
            resultAsStatusCodeResult.StatusCode.Should().Be(StatusCodes.Status501NotImplemented);
        }

        [TestMethod]
        public async Task Post_ReturnsTheSuccessResponse_WhenServiceIsSuccessfullyCalled()
        {
            const string nhsNumber = DefaultNhsNumber;
            const string odsCode = DefaultOdsCode;
            const string identityToken = DefaultIdentityToken;
            const string emailAddress = DefaultEmailAddress;
            const Supplier supplier = DefaultSupplier;

            var expectedResponse = _fixture.Create<LinkageResponse>();

            var mockResult = new LinkageResult.SuccessfullyRetrieved(expectedResponse);

            var mockLinkageService = new Mock<ILinkageService>();
            mockLinkageService.Setup(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req => req.NhsNumber.Equals(nhsNumber, StringComparison.Ordinal) &&
                                                   req.OdsCode.Equals(odsCode, StringComparison.Ordinal) &&
                                                   req.IdentityToken.Equals(identityToken, StringComparison.Ordinal) &&
                                                   req.EmailAddress.Equals(emailAddress, StringComparison.Ordinal)))
            ).ReturnsAsync(mockResult);

            var gpSystemMock = MockGpSystem(mockLinkageService);
            var gpSystemFactoryMock = MockGpSystemFactory(supplier, gpSystemMock);

            _linkageController = CreateLinkageController(gpSystemFactoryMock: gpSystemFactoryMock);

            var request = new CreateLinkageRequest
            {
                NhsNumber = nhsNumber,
                OdsCode = odsCode,
                IdentityToken = identityToken,
                EmailAddress = emailAddress,
            };

            // Act
            var result = await _linkageController.Post(request);

            // Assert
            mockLinkageService.Verify(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req => req.NhsNumber.Equals(nhsNumber, StringComparison.Ordinal) &&
                                                   req.OdsCode.Equals(odsCode, StringComparison.Ordinal) &&
                                                   req.IdentityToken.Equals(identityToken, StringComparison.Ordinal) &&
                                                   req.EmailAddress.Equals(emailAddress, StringComparison.Ordinal)))
                                                   , Times.Once);
            var resultValue = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = resultValue.Should().BeAssignableTo<LinkageResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockAuditor.Verify(x => x.AuditWithExplicitNhsNumber(It.IsAny<string>(), It.IsAny<Supplier>(), PostRequestAuditType, It.IsAny<string>()));
            _mockAuditor.Verify(x => x.AuditWithExplicitNhsNumber(It.IsAny<string>(), It.IsAny<Supplier>(), PostResponseAuditType, It.IsAny<string>()));
        }

        private LinkageController CreateLinkageController(
            Mock<IOdsCodeLookup> odsCodeLookupMock = null,
            Mock<IGpSystemFactory> gpSystemFactoryMock = null)
        {
            odsCodeLookupMock = odsCodeLookupMock ?? MockOdsCodeLookup();
            gpSystemFactoryMock = gpSystemFactoryMock ?? MockGpSystemFactory();
            var logger = new LoggerFactory();

            return new LinkageController(logger, gpSystemFactoryMock.Object, odsCodeLookupMock.Object, _mockAuditor.Object);
        }

        private static Mock<IOdsCodeLookup> MockOdsCodeLookup(
            string odsCode = DefaultOdsCode,
            Supplier supplier = DefaultSupplier)
        {
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(odsCode)).Returns(Task.FromResult(Option.Some(supplier)));
            return mockOdsCodeLookup;
        }

        private Mock<IGpSystemFactory> MockGpSystemFactory(
            Supplier supplier = DefaultSupplier,
            Mock<IGpSystem> gpSystemMock = null)
        {
            gpSystemMock = gpSystemMock ?? MockGpSystem();
            var mockGpSystemFactory = new Mock<IGpSystemFactory>();
            mockGpSystemFactory.Setup(x => x.CreateGpSystem(supplier)).Returns(gpSystemMock.Object);

            return mockGpSystemFactory;
        }

        private static Mock<IGpSystem> MockGpSystem(
            Mock<ILinkageService> linkageService = null)
        {
            linkageService = linkageService ?? MockLinkageService();

            var mockGpSystem = new Mock<IGpSystem>();
            mockGpSystem.Setup(x => x.GetLinkageService()).Returns(linkageService.Object);

            return mockGpSystem;
        }

        private static Mock<ILinkageService> MockLinkageService(
            LinkageResult expectedResult = null)
        {
            var mockLinkageService = new Mock<ILinkageService>();
            mockLinkageService.Setup(x => x.GetLinkageKey(DefaultNhsNumber, DefaultOdsCode, DefaultIdentityToken)).ReturnsAsync(expectedResult);

            return mockLinkageService;
        }
    }
}
