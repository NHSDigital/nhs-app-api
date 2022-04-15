using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.UserInfo.Mappers;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UserInfo.Areas.UserInfo.Models;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.UserInfo.Mappers
{
    [TestClass]
    public sealed class InfoUserProfileMapperTests
    {
        private InfoUserProfileMapper _systemUnderTest;
        private Mock<IMapper<string, ProofLevel?>> _mockProofLevelMapper;
        private CitizenIdSessionResult _citizenIdSessionResult;

        [TestInitialize]
        public void TestInitialize()
        {
            _citizenIdSessionResult = new CitizenIdSessionResult
            {
                Session = new CitizenIdUserSession
                {
                    ProofLevel = ProofLevel.P9,
                    OdsCode = "ODS Code",
                },
                NhsNumber = "NHS Number",
                Email = "email"
            };

            _mockProofLevelMapper = new Mock<IMapper<string, ProofLevel?>>();
            _systemUnderTest = new InfoUserProfileMapper(
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
        public void Map_UserProfileProofLevelIsNotP9_ReturnsInfoUserProfileWithoutNhsNumber(ProofLevel proofLevel)
        {
            // Arrange
            _citizenIdSessionResult.Session.ProofLevel = proofLevel;

            // Act
            var result = _systemUnderTest.Map(_citizenIdSessionResult);

            // Assert
            _mockProofLevelMapper.VerifyAll();

            result.Should().BeEquivalentTo(new InfoUserProfile
            {
                NhsNumber = null,
                OdsCode = _citizenIdSessionResult.Session.OdsCode,
                Email = _citizenIdSessionResult.Email
            });
        }

        [TestMethod]
        public void Map_UserProfileProofLevelIsP9_ReturnsInfoUserProfileWithNhsNumber()
        {
            // Act
            var result = _systemUnderTest.Map(_citizenIdSessionResult);

            // Assert
            _mockProofLevelMapper.VerifyAll();

            result.Should().BeEquivalentTo(new InfoUserProfile
            {
                NhsNumber = "NHSNumber",
                OdsCode = _citizenIdSessionResult.Session.OdsCode,
                Email = _citizenIdSessionResult.Email
            });
        }
    }
}
