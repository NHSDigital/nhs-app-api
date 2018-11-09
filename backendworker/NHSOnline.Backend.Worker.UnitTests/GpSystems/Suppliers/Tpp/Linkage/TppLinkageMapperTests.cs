using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Linkage
{
    [TestClass]
    public class TppLinkageMapperTests
    {
        private TppLinkageMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new TppLinkageMapper();
        }

        [TestMethod]
        public void Map_ForValidInput_ReturnValidResponse()
        {
            AddNhsUserRequest addNhsUserRequest = Mock.Of<AddNhsUserRequest>();
            AddNhsUserResponse addNhsUserResponse = Mock.Of<AddNhsUserResponse>();

            var expectedResponse = new LinkageResponse()
            {
                AccountId = addNhsUserResponse.AccountId,
                LinkageKey = Constants.TppConstants.LinkageKey,
                OdsCode = addNhsUserRequest.OrganisationCode
            };

            var result = _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);
            
            result.Should().BeEquivalentTo(expectedResponse);

        }
        
        [TestMethod]
        public void Map_ForNullRequest_ThrowsArgumentNullException()
        {
            AddNhsUserRequest addNhsUserRequest = null;
            AddNhsUserResponse addNhsUserResponse = Mock.Of<AddNhsUserResponse>();

            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            action.Should().Throw<ArgumentNullException>();

        }
        
        [TestMethod]
        public void Map_ForNullResponse_ThrowsArgumentNullExceptionn()
        {
            AddNhsUserRequest addNhsUserRequest = Mock.Of<AddNhsUserRequest>();
            AddNhsUserResponse addNhsUserResponse = null;

            Action action = () => _systemUnderTest.Map(addNhsUserRequest, addNhsUserResponse);

            action.Should().Throw<ArgumentNullException>();

        }
    }
}