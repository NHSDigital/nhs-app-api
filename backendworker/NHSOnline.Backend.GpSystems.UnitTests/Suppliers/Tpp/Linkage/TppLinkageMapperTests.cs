using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Linkage
{
    [TestClass]
    public class TppLinkageMapperTests
    {
        private TppLinkageMapper _systemUnderTest;
        private static AddNhsUserRequest ValidAddNhsUserRequest => new AddNhsUserRequest { OrganisationCode = "A123456" };
        private static AddNhsUserResponse ValidAddNhsUserResponse => new AddNhsUserResponse { PassphraseToLink = "absc123", AccountId = "123456" };

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            var logger = fixture.Freeze<Mock<ILogger<TppLinkageMapper>>>();
            _systemUnderTest = new TppLinkageMapper(logger.Object);
        }

        [TestMethod]
        public void Map_ForValidInput_ReturnValidResponse()
        {
            // Arrange
            var addNhsUserRequest = ValidAddNhsUserRequest;
            var addNhsUserResponse = ValidAddNhsUserResponse;

            var expectedResponse = new LinkageResponse()
            {
                AccountId = addNhsUserResponse.AccountId,
                LinkageKey = addNhsUserResponse.PassphraseToLink,
                OdsCode = addNhsUserRequest.OrganisationCode
            };

            // Act
            var result = _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);
            
            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ForValidInput_ReturnValidResponseWithHtml()
        {
            // Arrange
            var addNhsUserRequest = ValidAddNhsUserRequest;
            var addNhsUserResponse = ValidAddNhsUserResponse;
            addNhsUserResponse.PassphraseToLink = "Y8C8m&quot;&amp;dbCY3vfs2";

            var expectedResponse = new LinkageResponse()
            {
                AccountId = addNhsUserResponse.AccountId,
                LinkageKey = "Y8C8m\"&dbCY3vfs2",
                OdsCode = addNhsUserRequest.OrganisationCode
            };

            // Act
            var result = _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ForNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            AddNhsUserRequest addNhsUserRequest = null;
            var addNhsUserResponse = ValidAddNhsUserResponse;

            // Act
            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Map_ForNullRequestOrganisationCode_ThrowsArgumentNullException()
        {
            // Arrange
            var addNhsUserRequest = ValidAddNhsUserRequest;
            addNhsUserRequest.OrganisationCode = null;
            var addNhsUserResponse = ValidAddNhsUserResponse;

            // Act
            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Map_ForEmptyRequestOrganisationCode_ThrowsArgumentNullException()
        {
            // Arrange
            var addNhsUserRequest = ValidAddNhsUserRequest;
            addNhsUserRequest.OrganisationCode = string.Empty;
            var addNhsUserResponse = ValidAddNhsUserResponse;

            // Act
            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Map_ForNullResponse_ThrowsArgumentNullException()
        {
            // Arrange
            var addNhsUserRequest = ValidAddNhsUserRequest;
            AddNhsUserResponse addNhsUserResponse = null;

            // Act
            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Map_ForNullResponsePassphraseToLink_ThrowsArgumentNullException()
        {
            // Arrange
            var addNhsUserRequest = ValidAddNhsUserRequest;
            var addNhsUserResponse = ValidAddNhsUserResponse;
            addNhsUserResponse.PassphraseToLink = null;

            // Act
            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Map_ForEmptyResponsePassphraseToLink_ThrowsArgumentNullException()
        {
            // Arrange
            var addNhsUserRequest = ValidAddNhsUserRequest;
            var addNhsUserResponse = ValidAddNhsUserResponse;
            addNhsUserResponse.PassphraseToLink = string.Empty;

            // Act
            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Map_ForNullResponseAccountId_ThrowsArgumentNullException()
        {
            // Arrange
            var addNhsUserRequest = ValidAddNhsUserRequest;
            var addNhsUserResponse = ValidAddNhsUserResponse;
            addNhsUserResponse.AccountId = null;

            // Act
            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Map_ForEmptyResponseAccountId_ThrowsArgumentNullException()
        {
            // Arrange
            var addNhsUserRequest = ValidAddNhsUserRequest;
            var addNhsUserResponse = ValidAddNhsUserResponse;
            addNhsUserResponse.AccountId = string.Empty;

            // Act
            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }
    }
}