using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UserInfoApi.Repository;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;
using UnitTestHelper;

namespace NHSOnline.Backend.UserInfoApi.UnitTests.Areas.UserInfo
{
    [TestClass]
    public class InfoServiceTests
    {
        private IFixture _fixture;
        private InfoService _systemUnderTest;
        private Mock<IInfoRepository> _mockInfoRepository;
        private string _nhsLoginId;
        private AccessToken _accessToken;
        private string _nhsNumber;
        private string _odsCode;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            _mockInfoRepository = _fixture.Freeze<Mock<IInfoRepository>>();
            _nhsLoginId = _fixture.Create<string>();
            _nhsNumber = _fixture.Create<string>();
            _odsCode = _fixture.Create<string>();
            var mockLogger = _fixture.Freeze<Mock<ILogger<InfoService>>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _nhsLoginId),
                new Claim("nhs_number", _nhsNumber)
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);
            _systemUnderTest = _fixture.Create<InfoService>();
        }

        [TestMethod]
        public async Task Send_SuccessCreated()
        {
            // Arrange
            UserAndInfo actualUserInfo = null;

            _mockInfoRepository.Setup(x => x.Create(It.IsAny<UserAndInfo>()))
                .Callback<UserAndInfo>(u => actualUserInfo = u)
                .ReturnsAsync(new PostInfoResult.Created());

            // Act
            var result = await _systemUnderTest.Send(_accessToken, _odsCode);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.Created>();
            actualUserInfo.NhsLoginId.Should().BeEquivalentTo(_nhsLoginId);
            actualUserInfo.Info.NhsNumber.Should().BeEquivalentTo(_nhsNumber);
            actualUserInfo.Info.OdsCode.Should().Be(_odsCode);
        }

        [TestMethod]
        public async Task Send_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.Create(It.IsAny<UserAndInfo>())).Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Send(_accessToken, _odsCode);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Send_RepositoryThrowsMongoException_ReturnsBadGateway()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.Create(It.IsAny<UserAndInfo>())).Throws(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.Send(_accessToken, _odsCode);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetInfo_SuccessFound()
        {
            // Arrange
            var userInfo = _fixture.Create<UserAndInfo>();

            _mockInfoRepository.Setup(x => x.FindByNhsLoginId(It.IsAny<string>()))
                .ReturnsAsync(userInfo);

            // Act
            var result = await _systemUnderTest.GetInfo(_accessToken);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.Found>().Subject
                .UserInfo.Should().BeEquivalentTo(userInfo);
        }

        [TestMethod]
        public async Task GetInfo_SuccessNotFound()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByNhsLoginId(It.IsAny<string>()))
                .ReturnsAsync((UserAndInfo)null);

            // Act
            var result = await _systemUnderTest.GetInfo(_accessToken);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.NotFound>();
        }

        [TestMethod]
        public async Task GetInfo_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByNhsLoginId(It.IsAny<string>())).Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.GetInfo(_accessToken);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetInfo_RepositoryThrowsMongoException_ReturnsBadGateway()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByNhsLoginId(It.IsAny<string>())).Throws(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.GetInfo(_accessToken);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetInfoByNhsNumber_SuccessFoundMultiple()
        {
            // Arrange
            var userInfo = _fixture.Create<IEnumerable<UserAndInfo>>();
            var nhsNumber = _fixture.Create<string>();
            var nhsLoginIds = userInfo.Select(info => info.NhsLoginId);

            _mockInfoRepository.Setup(x => x.FindByNhsNumber(nhsNumber))
                .ReturnsAsync(userInfo);

            // Act
            var result = await _systemUnderTest.GetInfoByNhsNumber(nhsNumber);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.FoundMultiple>().Subject.NhsLoginIds
                .Should().BeEquivalentTo(nhsLoginIds);
        }

        [TestMethod]
        public async Task GetInfoByNhsNumber_SuccessFoundNone()
        {
            var noInfo = Enumerable.Empty<UserAndInfo>();
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByNhsNumber(It.IsAny<string>()))
                .ReturnsAsync(noInfo);

            // Act
            var result = await _systemUnderTest.GetInfoByNhsNumber(_fixture.Create<string>());

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.FoundMultiple>().Subject
                .NhsLoginIds.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetInfoByNhsNumber_RepositoryNullResponse_ReturnsInternalServerError()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByNhsNumber(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<UserAndInfo>) null);

            // Act
            var result = await _systemUnderTest.GetInfoByNhsNumber(_fixture.Create<string>());

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetInfoByNhsNumber_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByNhsNumber(It.IsAny<string>()))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.GetInfoByNhsNumber(_fixture.Create<string>());

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetInfoByNhsNumber_RepositoryThrowsMongoException_ReturnsBadGateway()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByNhsNumber(It.IsAny<string>()))
                .Throws(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.GetInfoByNhsNumber(_fixture.Create<string>());

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetInfoByOdsCode_SuccessFoundMultiple()
        {
            // Arrange
            var userInfo = _fixture.Create<IEnumerable<UserAndInfo>>();
            var odsCode = _fixture.Create<string>();
            var nhsLoginIds = userInfo.Select(info => info.NhsLoginId);

            _mockInfoRepository.Setup(x => x.FindByOdsCode(odsCode))
                .ReturnsAsync(userInfo);

            // Act
            var result = await _systemUnderTest.GetInfoByOdsCode(odsCode);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.FoundMultiple>().Subject.NhsLoginIds
                .Should().BeEquivalentTo(nhsLoginIds);
        }

        [TestMethod]
        public async Task GetInfoByOdsCode_SuccessFoundNone()
        {
            var noInfo = Enumerable.Empty<UserAndInfo>();
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByOdsCode(It.IsAny<string>()))
                .ReturnsAsync(noInfo);

            // Act
            var result = await _systemUnderTest.GetInfoByOdsCode(_fixture.Create<string>());

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.FoundMultiple>().Subject
                .NhsLoginIds.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetInfoByOdsCode_RepositoryNullResponse_ReturnsInternalServerError()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByOdsCode(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<UserAndInfo>) null);

            // Act
            var result = await _systemUnderTest.GetInfoByOdsCode(_fixture.Create<string>());

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetInfoByOdsCode_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByOdsCode(It.IsAny<string>()))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.GetInfoByOdsCode(_fixture.Create<string>());

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetInfoByOdsCode_RepositoryThrowsMongoException_ReturnsBadGateway()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByOdsCode(It.IsAny<string>()))
                .Throws(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.GetInfoByOdsCode(_fixture.Create<string>());

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.BadGateway>();
        }
    }
}