using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.UnitTests.Areas;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Linkage
{
    [TestClass]
    public class TppLinkageMapperTests
    {
        private TppLinkageMapper _systemUnderTest;
        private AddNhsUserRequest ValidAddNhsUserRequest => new AddNhsUserRequest { OrganisationCode = "A123456" };
        private AddNhsUserResponse ValidAddNhsUserResponse => new AddNhsUserResponse { PassphraseToLink = "absc123", AccountId = "123456" };

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            var logger = fixture.Freeze<Mock<ILogger<TppLinkageMapper>>>();
            _systemUnderTest = new TppLinkageMapper(logger.Object);
        }

        [TestMethod]
        public void Map_ForValidInput_ReturnValidResponse()
        {
            var addNhsUserRequest = ValidAddNhsUserRequest;
            var addNhsUserResponse = ValidAddNhsUserResponse;

            var expectedResponse = new LinkageResponse()
            {
                AccountId = addNhsUserResponse.AccountId,
                LinkageKey = addNhsUserResponse.PassphraseToLink,
                OdsCode = addNhsUserRequest.OrganisationCode
            };

            var result = _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);
            
            result.Should().BeEquivalentTo(expectedResponse);

        }

        [TestMethod]
        public void Map_ForValidInput_ReturnValidResponseWithHtml()
        {
            var addNhsUserRequest = ValidAddNhsUserRequest;
            var addNhsUserResponse = ValidAddNhsUserResponse;
            addNhsUserResponse.PassphraseToLink = "Y8C8m&quot;&amp;dbCY3vfs2";

            var expectedResponse = new LinkageResponse()
            {
                AccountId = addNhsUserResponse.AccountId,
                LinkageKey = "Y8C8m\"&dbCY3vfs2",
                OdsCode = addNhsUserRequest.OrganisationCode
            };

            var result = _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            result.Should().BeEquivalentTo(expectedResponse);

        }

        [TestMethod]
        public void Map_ForNullRequest_ThrowsArgumentNullException()
        {
            AddNhsUserRequest addNhsUserRequest = null;
            var addNhsUserResponse = ValidAddNhsUserResponse;

            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            action.Should().Throw<ArgumentNullException>();

        }

        [TestMethod]
        public void Map_ForNullRequestOrganistationCode_ThrowsArgumentNullException()
        {
            var addNhsUserRequest = ValidAddNhsUserRequest;
            addNhsUserRequest.OrganisationCode = null;
            var addNhsUserResponse = ValidAddNhsUserResponse;

            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Map_ForEmptyRequestOrganistationCode_ThrowsArgumentNullException()
        {
            var addNhsUserRequest = ValidAddNhsUserRequest;
            addNhsUserRequest.OrganisationCode = string.Empty;
            var addNhsUserResponse = ValidAddNhsUserResponse;

            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            action.Should().Throw<ArgumentNullException>();
        }


        [TestMethod]
        public void Map_ForNullResponse_ThrowsArgumentNullException()
        {
            var addNhsUserRequest = ValidAddNhsUserRequest;
            AddNhsUserResponse addNhsUserResponse = null;

            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            action.Should().Throw<ArgumentNullException>();

        }

        [TestMethod]
        public void Map_ForNullResponsePassphraseToLink_ThrowsArgumentNullException()
        {
            var addNhsUserRequest = ValidAddNhsUserRequest;
            var addNhsUserResponse = ValidAddNhsUserResponse;
            addNhsUserResponse.PassphraseToLink = null;

            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Map_ForEmptyResponsePassphraseToLink_ThrowsArgumentNullException()
        {
            var addNhsUserRequest = ValidAddNhsUserRequest;
            var addNhsUserResponse = ValidAddNhsUserResponse;
            addNhsUserResponse.PassphraseToLink = string.Empty;

            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Map_ForNullResponseAccountId_ThrowsArgumentNullException()
        {
            var addNhsUserRequest = ValidAddNhsUserRequest;
            var addNhsUserResponse = ValidAddNhsUserResponse;
            addNhsUserResponse.AccountId = null;

            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Map_ForEmptyResponseAccountId_ThrowsArgumentNullException()
        {
            var addNhsUserRequest = ValidAddNhsUserRequest;
            var addNhsUserResponse = ValidAddNhsUserResponse;
            addNhsUserResponse.AccountId = string.Empty;

            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}