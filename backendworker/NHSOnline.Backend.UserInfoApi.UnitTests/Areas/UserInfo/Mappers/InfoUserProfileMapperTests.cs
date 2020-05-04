using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Mappers;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;

namespace NHSOnline.Backend.UserInfoApi.UnitTests.Areas.UserInfo.Mappers
{
    [TestClass]
    public sealed class InfoUserProfileMapperTests
    {
        private InfoUserProfileMapper _systemUnderTest;
        private Mock<IMapper<string, ProofLevel?>> _mockProofLevelMapper;
        private UserProfile _userProfile;

        [TestInitialize]
        public void TestInitialize()
        {
            _userProfile = new UserProfile(
                new Auth.CitizenId.Models.UserInfo
                {
                    GpIntegrationCredentials = { OdsCode = "ODS Code" },
                    NhsNumber = "NHS Number",
                },
                "Access Token"
            );

            _mockProofLevelMapper = new Mock<IMapper<string, ProofLevel?>>();
            _systemUnderTest = new InfoUserProfileMapper(
                _mockProofLevelMapper.Object,
                new Mock<ILogger<InfoUserProfileMapper>>().Object
            );
        }

        [TestMethod]
        public void Map_UserProfileIsNull_ThrowsException()
        {
            // Act and Assert
            Action act = () => _systemUnderTest.Map(null);

            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(ProofLevel.P5)]
        public void Map_UserProfileProofLevelIsNotP9_ReturnsInfoUserProfileWithoutNhsNumber(ProofLevel? proofLevel)
        {
            // Arrange
            _mockProofLevelMapper.Setup(x => x.Map(_userProfile.IdentityProofingLevel))
                .Returns(proofLevel);

            // Act
            var result = _systemUnderTest.Map(_userProfile);

            // Assert
            _mockProofLevelMapper.VerifyAll();

            result.Should().BeEquivalentTo(new InfoUserProfile { NhsNumber = null, OdsCode = _userProfile.OdsCode });
        }

        [TestMethod]
        public void Map_UserProfileProofLevelIsP9_ReturnsInfoUserProfileWithNhsNumber()
        {
            // Arrange

            _mockProofLevelMapper.Setup(x => x.Map(_userProfile.IdentityProofingLevel))
                .Returns(ProofLevel.P9);

            // Act
            var result = _systemUnderTest.Map(_userProfile);

            // Assert
            _mockProofLevelMapper.VerifyAll();

            result.Should().BeEquivalentTo(new InfoUserProfile
            {
                NhsNumber = _userProfile.NhsNumber,
                OdsCode = _userProfile.OdsCode
            });
        }
    }
}
