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
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Linkage;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Linkage
{
    [TestClass]
    public class TppLinkageMapperTests
    {
        private TppLinkageMapper _systemUnderTest;
        private static LinkAccountCreate ValidLinkAccountCreateRequest => new LinkAccountCreate { OrganisationCode = "A123456" };
        private static LinkAccountReply ValidLinkAccountCreateReply => new LinkAccountReply { PassphraseToLink = "absc123", AccountId = "123456" };

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
            var addNhsUserRequest = ValidLinkAccountCreateRequest;
            var linkAccountReply = ValidLinkAccountCreateReply;

            var expectedResponse = new LinkageResponse
            {
                AccountId = linkAccountReply.AccountId,
                LinkageKey = linkAccountReply.PassphraseToLink,
                OdsCode = addNhsUserRequest.OrganisationCode
            };

            // Act
            var result = _systemUnderTest.Map(addNhsUserRequest, linkAccountReply);

            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ForValidInput_ReturnValidResponseWithHtml()
        {
            // Arrange
            var addNhsUserRequest = ValidLinkAccountCreateRequest;
            var linkAccountReply = ValidLinkAccountCreateReply;
            linkAccountReply.PassphraseToLink = "Y8C8m&quot;&amp;dbCY3vfs2";

            var expectedResponse = new LinkageResponse
            {
                AccountId = linkAccountReply.AccountId,
                LinkageKey = "Y8C8m\"&dbCY3vfs2",
                OdsCode = addNhsUserRequest.OrganisationCode
            };

            // Act
            var result = _systemUnderTest.Map(addNhsUserRequest, linkAccountReply);

            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ForNullRequest_ThrowsArgumentException()
        {
            // Arrange
            LinkAccountCreate request = null;
            var linkAccountReply = ValidLinkAccountCreateReply;

            // Act
            Action action = () => _systemUnderTest.Map(request, linkAccountReply);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void Map_ForNullRequestOrganisationCode_ThrowsArgumentException()
        {
            // Arrange
            var request = ValidLinkAccountCreateRequest;
            request.OrganisationCode = null;
            var linkAccountReply = ValidLinkAccountCreateReply;

            // Act
            Action action = () => _systemUnderTest.Map(request, linkAccountReply);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void Map_ForEmptyRequestOrganisationCode_ThrowsArgumentException()
        {
            // Arrange
            var request = ValidLinkAccountCreateRequest;
            request.OrganisationCode = string.Empty;
            var linkAccountReply = ValidLinkAccountCreateReply;

            // Act
            Action action = () => _systemUnderTest.Map(request, linkAccountReply);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void Map_ForNullResponse_ThrowsArgumentException()
        {
            // Arrange
            var request = ValidLinkAccountCreateRequest;
            LinkAccountReply linkAccountReply = null;

            // Act
            Action action = () => _systemUnderTest.Map(request, linkAccountReply);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void Map_ForNullResponsePassphraseToLink_ThrowsArgumentException()
        {
            // Arrange
            var request = ValidLinkAccountCreateRequest;
            var response = ValidLinkAccountCreateReply;
            response.PassphraseToLink = null;

            // Act
            Action action = () => _systemUnderTest.Map(request, response);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void Map_ForEmptyResponsePassphraseToLink_ThrowsArgumentException()
        {
            // Arrange
            var request = ValidLinkAccountCreateRequest;
            var linkAccountReply = ValidLinkAccountCreateReply;
            linkAccountReply.PassphraseToLink = string.Empty;

            // Act
            Action action = () => _systemUnderTest.Map(request, linkAccountReply);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void Map_ForNullResponseAccountId_ThrowsArgumentException()
        {
            // Arrange
            var addNhsUserRequest = ValidLinkAccountCreateRequest;
            var linkAccountReply = ValidLinkAccountCreateReply;
            linkAccountReply.AccountId = null;

            // Act
            Action action = () => _systemUnderTest.Map(addNhsUserRequest, linkAccountReply);

            // Assert
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void Map_ForEmptyResponseAccountId_ThrowsArgumentException()
        {
            // Arrange
            var addNhsUserRequest = ValidLinkAccountCreateRequest;
            var linkAccountReply = ValidLinkAccountCreateReply;
            linkAccountReply.AccountId = string.Empty;

            // Act
            Action action = () => _systemUnderTest.Map(addNhsUserRequest, linkAccountReply);

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}