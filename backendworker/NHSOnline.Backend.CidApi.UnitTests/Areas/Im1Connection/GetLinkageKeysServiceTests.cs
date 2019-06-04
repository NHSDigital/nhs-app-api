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
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;

namespace NHSOnline.Backend.CidApi.UnitTests.Areas.Im1Connection
{
    [TestClass]
    public class GetLinkageKeysServiceTests
    {
        private const string DefaultOdsCode = "AB1234";
        private const string DefaultNhsNumber = "XX00000A";
        private const string DefaultSurname = "Surname";
        private static readonly DateTime? DefaultDateOfBirth = new DateTime(1980,1,1);
        private const string DefaultIdentityToken = "IDTOKEN";
        
        private IFixture _fixture;
        private Mock<IOdsCodeMassager> _odsCodeMassager;
        private GetLinkageKeysService _systemUnderTest;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Freeze<Mock<IAuditor>>();
            _fixture.Freeze<Mock<ILogger<GetLinkageKeysService>>>();
            _odsCodeMassager = _fixture.Freeze<Mock<IOdsCodeMassager>>();
            _odsCodeMassager.Setup(x => x.CheckOdsCode(DefaultOdsCode)).Returns(DefaultOdsCode);
            _systemUnderTest = _fixture.Create<GetLinkageKeysService>();
        }
        
        [TestMethod]
        public async Task Get_WhenServiceIsSuccessfullyCalled_ReturnsTheSuccessResponse()
        {   
            // Arrange
            var getLinkageRequest = new GetLinkageRequest()
            {
                NhsNumber = DefaultNhsNumber,
                Surname = DefaultSurname,
                DateOfBirth = DefaultDateOfBirth,
                OdsCode = DefaultOdsCode,
                IdentityToken = DefaultIdentityToken
            };
            
            var response = _fixture.Create<LinkageResponse>();
            var linkageService = MockLinkageService(new LinkageResult.SuccessfullyRetrieved(response));
            var gpSystemMock = MockGpSystem(linkageService);
            
            // Act
            var result = await _systemUnderTest.GetLinkageKey(getLinkageRequest, gpSystemMock.Object);

            // Assert
            linkageService.Verify(x => x.GetLinkageKey(
                It.Is<GetLinkageRequest>(req => req.NhsNumber.Equals(getLinkageRequest.NhsNumber, StringComparison.Ordinal) &&
                                                req.Surname.Equals(getLinkageRequest.Surname, StringComparison.Ordinal) &&
                                                req.DateOfBirth.Equals(getLinkageRequest.DateOfBirth) &&
                                                req.OdsCode.Equals(getLinkageRequest.OdsCode, StringComparison.Ordinal) &&
                                                req.IdentityToken.Equals(getLinkageRequest.IdentityToken, StringComparison.Ordinal))), Times.Once);

            result.Should().BeAssignableTo<LinkageResult>();
            result.Should().BeOfType(typeof(LinkageResult.SuccessfullyRetrieved));
        }
        
        private static Mock<IGpSystem> MockGpSystem(
            Mock<ILinkageService> linkageService = null)
        {
            linkageService = linkageService ?? MockLinkageService();

            var mockGpSystem = new Mock<IGpSystem>();
            mockGpSystem.Setup(x => x.GetLinkageService()).Returns(linkageService.Object);

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
    }
}
