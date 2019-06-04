using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.CidApi.Areas.Im1Connection;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.CidApi.UnitTests.Areas.Im1Connection
{
    [TestClass]
    public class RetrieveLinkageKeysServiceTests
    {
        private const string DefaultOdsCode = "AB1234";
        private const string DefaultNhsNumber = "XX00000A";
        private const string DefaultSurname = "Surname";
        private static readonly DateTime DefaultDateOfBirth = new DateTime(1980, 1, 1);
        private const string DefaultIdentityToken = "IDTOKEN";

        private IFixture _fixture;
        private Mock<IOdsCodeMassager> _odsCodeMassager;
        private Mock<ILogger<RetrieveLinkageKeysService>> _logger;
        private Mock<ICreateLinkageKeysService> _createLinkageKeysService;
        private RetrieveLinkageKeysService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _createLinkageKeysService = _fixture.Create<Mock<ICreateLinkageKeysService>>();
            _logger = _fixture.Create<Mock<ILogger<RetrieveLinkageKeysService>>>();
            _odsCodeMassager = _fixture.Freeze<Mock<IOdsCodeMassager>>();
            _odsCodeMassager.Setup(x => x.CheckOdsCode(DefaultOdsCode)).Returns(DefaultOdsCode);
        }

        [TestMethod]
        public async Task GetLinkageKey_AllParametersPassed_ReturnsLinkageResult()
        {
            // Arrange
            var createIm1ConnectionRequest = CreateIm1ConnectionRequest();
            var mockResult = new LinkageResult.SuccessfullyRetrieved(_fixture.Create<LinkageResponse>());
            var linkageService = MockLinkageService(mockResult);
            var gpSystemMock = MockGpSystem(linkageService);

            var getLinkageKeysService = MockGetLinkageKeysService(mockResult, gpSystemMock.Object);
            _systemUnderTest = _fixture.Create<RetrieveLinkageKeysService>();

            new RetrieveLinkageKeysService(_logger.Object, getLinkageKeysService.Object,
                _createLinkageKeysService.Object);

            // Act

            var result =
                await _systemUnderTest.RetrieveLinkageKey(createIm1ConnectionRequest, gpSystemMock.Object);

            // Assert
            getLinkageKeysService.Verify(x => x.GetLinkageKey(
                It.Is<GetLinkageRequest>(req =>
                    req.NhsNumber.Equals(createIm1ConnectionRequest.NhsNumber, StringComparison.Ordinal) &&
                    req.Surname.Equals(createIm1ConnectionRequest.Surname, StringComparison.Ordinal) &&
                    req.DateOfBirth.Equals(createIm1ConnectionRequest.DateOfBirth) &&
                    req.OdsCode.Equals(createIm1ConnectionRequest.OdsCode, StringComparison.Ordinal) &&
                    req.IdentityToken.Equals(createIm1ConnectionRequest.IdentityToken, StringComparison.Ordinal))
                , gpSystemMock.Object), Times.Once);

            result.Should().BeAssignableTo<LinkageResult>();
            result.Should().BeOfType(typeof(LinkageResult.SuccessfullyRetrieved));
        }

        [TestMethod]
        public async Task CreateLinkageKey_AllParametersPassed_ReturnsLinkageResult()
        {
            // Arrange
            var createIm1ConnectionRequest = CreateIm1ConnectionRequest();
            var mockResult = new LinkageResult.SuccessfullyCreated(_fixture.Create<LinkageResponse>());
            var linkageService = MockLinkageService(mockResult);
            var gpSystemMock = MockGpSystem(linkageService);

            var mockCreateLinkageKeysService = new Mock<ICreateLinkageKeysService>();

            mockCreateLinkageKeysService.Setup(x => x.CreateLinkageKey(
                    It.Is<CreateLinkageRequest>(req =>
                        req.NhsNumber.Equals(DefaultNhsNumber, StringComparison.Ordinal) &&
                        req.Surname.Equals(DefaultSurname, StringComparison.Ordinal) &&
                        req.DateOfBirth.Equals(DefaultDateOfBirth) &&
                        req.OdsCode.Equals(DefaultOdsCode, StringComparison.Ordinal) &&
                        req.IdentityToken.Equals(DefaultIdentityToken, StringComparison.Ordinal)
                    ), gpSystemMock.Object))
                .ReturnsAsync(mockResult);

            _createLinkageKeysService = mockCreateLinkageKeysService;

            var getLinkageKeysService = MockGetLinkageKeysService(mockResult, gpSystemMock.Object);
            getLinkageKeysService.Setup(x => x.GetLinkageKey(It.IsAny<GetLinkageRequest>(), It.IsAny<IGpSystem>()))
                .ReturnsAsync(new LinkageResult.NotFound(Im1ConnectionErrorCodes.Code.UnknownError));
            _systemUnderTest = new RetrieveLinkageKeysService(_logger.Object, getLinkageKeysService.Object,
                _createLinkageKeysService.Object);

            // Act

            var result =
                await _systemUnderTest.RetrieveLinkageKey(createIm1ConnectionRequest, gpSystemMock.Object);

            // Assert
            _createLinkageKeysService.Verify(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req =>
                    req.NhsNumber.Equals(createIm1ConnectionRequest.NhsNumber, StringComparison.Ordinal) &&
                    req.Surname.Equals(createIm1ConnectionRequest.Surname, StringComparison.Ordinal) &&
                    req.DateOfBirth.Equals(createIm1ConnectionRequest.DateOfBirth) &&
                    req.OdsCode.Equals(createIm1ConnectionRequest.OdsCode, StringComparison.Ordinal) &&
                    req.IdentityToken.Equals(createIm1ConnectionRequest.IdentityToken, StringComparison.Ordinal))
                , gpSystemMock.Object), Times.Once);

            result.Should().BeAssignableTo<LinkageResult>();
            result.Should().BeOfType(typeof(LinkageResult.SuccessfullyCreated));
        }

        private static RetrieveLinkageKeysRequest CreateIm1ConnectionRequest()
        {
            var createIm1ConnectionRequest = new RetrieveLinkageKeysRequest()
            {
                NhsNumber = DefaultNhsNumber,
                Surname = DefaultSurname,
                DateOfBirth = DefaultDateOfBirth,
                OdsCode = DefaultOdsCode,
                IdentityToken = DefaultIdentityToken
            };
            return createIm1ConnectionRequest;
        }

        private Mock<IGpSystem> MockGpSystem(Mock<ILinkageService> linkageService = null)
        {
            linkageService = linkageService ?? MockLinkageService();

            var mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            mockGpSystem.Setup(x => x.GetLinkageService()).Returns(linkageService.Object);

            return mockGpSystem;
        }

        private Mock<ILinkageService> MockLinkageService(LinkageResult expectedResult = null)
        {
            var mockLinkageService = _fixture.Freeze<Mock<ILinkageService>>();

            mockLinkageService.Setup(x => x.CreateLinkageKey(
                    It.Is<CreateLinkageRequest>(req =>
                        req.NhsNumber.Equals(DefaultNhsNumber, StringComparison.Ordinal) &&
                        req.Surname.Equals(DefaultSurname, StringComparison.Ordinal) &&
                        req.DateOfBirth.Equals(DefaultDateOfBirth) &&
                        req.OdsCode.Equals(DefaultOdsCode, StringComparison.Ordinal) &&
                        req.IdentityToken.Equals(DefaultIdentityToken, StringComparison.Ordinal)
                    )))
                .ReturnsAsync(expectedResult);

            return mockLinkageService;
        }

        private Mock<IGetLinkageKeysService> MockGetLinkageKeysService(LinkageResult expectedResult, IGpSystem gpSystem)
        {
            var mockGetLinkageKeysService = _fixture.Freeze<Mock<IGetLinkageKeysService>>();

            mockGetLinkageKeysService.Setup(x => x.GetLinkageKey(
                    It.Is<GetLinkageRequest>(req => req.NhsNumber.Equals(DefaultNhsNumber, StringComparison.Ordinal) &&
                                                    req.Surname.Equals(DefaultSurname, StringComparison.Ordinal) &&
                                                    req.DateOfBirth.Equals(DefaultDateOfBirth) &&
                                                    req.OdsCode.Equals(DefaultOdsCode, StringComparison.Ordinal) &&
                                                    req.IdentityToken.Equals(DefaultIdentityToken,
                                                        StringComparison.Ordinal)
                    ), gpSystem))
                .ReturnsAsync(expectedResult);

            return mockGetLinkageKeysService;
        }
    }
}