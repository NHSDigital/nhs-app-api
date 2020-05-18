using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support.Repository;
using NHSOnline.Backend.UserInfoApi.Repository;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;
using UnitTestHelper;

namespace NHSOnline.Backend.UserInfoApi.UnitTests.Areas.UserInfo
{
    [TestClass]
    public class InfoServiceGetTests
    {
        private InfoService _systemUnderTest;
        private Mock<IInfoRepository> _mockInfoRepository;
        private string _nhsLoginId;
        private AccessToken _accessToken;
        private string _nhsNumber;
        private string _odsCode;
        private InfoUserProfile _userProfile;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockInfoRepository = new Mock<IInfoRepository>();
            _nhsLoginId = "NHS Login Id";
            _nhsNumber = "NHS Number";
            _odsCode = "ODS Code";
            _userProfile = new InfoUserProfile { NhsNumber = _nhsNumber, OdsCode = _odsCode };
            var mockLogger = new Mock<ILogger<InfoService>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _nhsLoginId),
                new Claim("nhs_number", _nhsNumber)
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);
            _systemUnderTest = new InfoService(_mockInfoRepository.Object, mockLogger.Object);
        }

        [TestMethod]
        public async Task Send_SuccessCreated()
        {
            // Arrange
            UserAndInfo actualUserInfo = null;
            var userAndInfo = new UserAndInfo()
            {
                Info = new Info
                {
                    NhsNumber = _nhsNumber,
                    OdsCode = _odsCode
                }
            };
            var expectedResult = new PostInfoResult.Created(userAndInfo);

            _mockInfoRepository.Setup(x => x.Create(It.IsAny<UserAndInfo>()))
                .Callback<UserAndInfo>(u => actualUserInfo = u)
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(userAndInfo));

            // Act
            var result = await _systemUnderTest.Send(_accessToken, _userProfile);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.Created>();
            result.Should().BeEquivalentTo(expectedResult);
            actualUserInfo.NhsLoginId.Should().BeEquivalentTo(_nhsLoginId);
            actualUserInfo.Info.NhsNumber.Should().BeEquivalentTo(_nhsNumber);
            actualUserInfo.Info.OdsCode.Should().Be(_odsCode);
        }

        [TestMethod]
        public async Task Send_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.Create(It.IsAny<UserAndInfo>()))
                .Throws(new ArgumentException(string.Empty));

            // Act
            var result = await _systemUnderTest.Send(_accessToken, _userProfile);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetInfo_SuccessFound()
        {
            // Arrange
            var userInfo = new UserAndInfo { NhsLoginId = _accessToken.Subject };

            _mockInfoRepository.Setup(x => x.FindByNhsLoginId(_accessToken.Subject))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(new[] { userInfo }));

            // Act
            var result = await _systemUnderTest.GetInfo(_accessToken);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.Found>().Subject
                .UserInfoRecords.Single().Should().Be(userInfo);
        }

        [TestMethod]
        public async Task GetInfo_SuccessNotFound()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByNhsLoginId(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.NotFound());

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
            _mockInfoRepository.Setup(x => x.FindByNhsLoginId(It.IsAny<string>()))
                .Throws(new ArgumentException(string.Empty));

            // Act
            var result = await _systemUnderTest.GetInfo(_accessToken);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetInfoByNhsNumber_SuccessFoundMultiple()
        {
            // Arrange
            var nhsLoginId1 = "NHS Login 1";
            var nhsLoginId2 = "NHS Login 2";
            var userInfos = new[]
            {
                new UserAndInfo { NhsLoginId = nhsLoginId1 },
                new UserAndInfo { NhsLoginId = nhsLoginId2 },
            };
            const string nhsNumber = "NHS number";

            _mockInfoRepository.Setup(x => x.FindByNhsNumber(nhsNumber))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(userInfos));

            // Act
            var result = await _systemUnderTest.GetInfoByNhsNumber(nhsNumber);

            // Assert
            _mockInfoRepository.VerifyAll();
            var foundRecords = result.Should().BeAssignableTo<GetInfoResult.Found>().Subject.UserInfoRecords;
            var foundNhsLoginIds = foundRecords.Select(record => record.NhsLoginId);
            foundNhsLoginIds.Should().BeEquivalentTo(new List<string> { nhsLoginId1, nhsLoginId2 });
        }

        [TestMethod]
        public async Task GetInfoByNhsNumber_SuccessFoundNone()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByNhsNumber(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.NotFound());

            // Act
            var result = await _systemUnderTest.GetInfoByNhsNumber("NHS number");

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.NotFound>();
        }

        [TestMethod]
        public async Task GetInfoByNhsNumber_RepositoryNullResponse_ReturnsInternalServerError()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByNhsNumber(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.InternalServerError());

            // Act
            var result = await _systemUnderTest.GetInfoByNhsNumber("NHS number");

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetInfoByNhsNumber_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByNhsNumber(It.IsAny<string>()))
                .Throws(new ArgumentException(string.Empty));

            // Act
            var result = await _systemUnderTest.GetInfoByNhsNumber("NHS number");

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetInfoByOdsCode_SuccessFoundMultiple()
        {
            // Arrange
            var nhsLoginId1 = "NHS Login 1";
            var nhsLoginId2 = "NHS Login 2";
            var userInfos = new[]
            {
                new UserAndInfo { NhsLoginId = nhsLoginId1 },
                new UserAndInfo { NhsLoginId = nhsLoginId2 },
            };
            var odsCode = "ODS Code";

            _mockInfoRepository.Setup(x => x.FindByOdsCode(odsCode))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(userInfos));

            // Act
            var result = await _systemUnderTest.GetInfoByOdsCode(odsCode);

            // Assert
            _mockInfoRepository.VerifyAll();
            var foundRecords = result.Should().BeAssignableTo<GetInfoResult.Found>().Subject.UserInfoRecords;
            var foundNhsLoginIds = foundRecords.Select(record => record.NhsLoginId);
            foundNhsLoginIds.Should().BeEquivalentTo(new List<string> { nhsLoginId1, nhsLoginId2 });
        }

        [TestMethod]
        public async Task GetInfoByOdsCode_SuccessFoundNone()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByOdsCode(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.NotFound());

            // Act
            var result = await _systemUnderTest.GetInfoByOdsCode("ODS Code");

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.NotFound>();
        }

        [TestMethod]
        public async Task GetInfoByOdsCode_RepositoryNullResponse_ReturnsInternalServerError()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByOdsCode(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.InternalServerError());

            // Act
            var result = await _systemUnderTest.GetInfoByOdsCode("ODS Code");

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetInfoByOdsCode_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockInfoRepository.Setup(x => x.FindByOdsCode(It.IsAny<string>()))
                .Throws(new ArgumentException(string.Empty));

            // Act
            var result = await _systemUnderTest.GetInfoByOdsCode("ODS Code");

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<GetInfoResult.InternalServerError>();
        }
    }
}