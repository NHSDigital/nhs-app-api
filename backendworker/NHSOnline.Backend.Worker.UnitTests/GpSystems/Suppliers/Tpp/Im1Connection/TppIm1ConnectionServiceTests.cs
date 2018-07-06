using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Im1Connection
{
    [TestClass]
    public class TppIm1ConnectionServiceTests
    {
        private const string DefaultConnectionToken = "{\"accountid\":\"account_id\",\"passphrase\":\"passphrase\"}";
        private const string DefaultOdsCode = "token";


        private IFixture _fixture;
        private Mock<ITppClient> _mockTppClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockTppClient = _fixture.Freeze<Mock<ITppClient>>();
        }

        [TestMethod]
        public async Task Verify_ReturnsAConnection_WhenRequested()
        {
            var authenticateReply = _fixture.Create<AuthenticateReply>();

            var expectedNhsNumbers = new List<PatientNhsNumber>
            {
                new PatientNhsNumber
                {
                    NhsNumber = authenticateReply.User?.Person?.NationalId?.Value
                }
            };

            _mockTppClient.Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        Body = authenticateReply,
                    }));

            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SuccessfullyVerified>();

            var successResult = result as Im1ConnectionVerifyResult.SuccessfullyVerified;

            successResult.Response.ConnectionToken.Should().Be(DefaultConnectionToken);
            successResult.Response.NhsNumbers.Should().BeEquivalentTo(expectedNhsNumbers);
        }

        [TestMethod]
        public async Task Verify_ReturnsAEmptyNhsNumbers_WhenTppRespondsWithEmptyNhsNumber()
        {
            _mockTppClient.Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        Body = null
                    }));

            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SuccessfullyVerified>();

            var successResult = result as Im1ConnectionVerifyResult.SuccessfullyVerified;

            successResult.Response.ConnectionToken.Should().Be(DefaultConnectionToken);
            successResult.Response.NhsNumbers.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Verify_ReturnsSupplierSystemUnavailable_WhenTppClientReturnsErrorResponse()
        {
            _mockTppClient.Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = _fixture.Create<Error>()
                    }));

            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Verify_ReturnsSupplierSystemUnavailable_WhenTppClientReturnsBadgateway()
        {
            _mockTppClient.Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.BadGateway)
                    {
                        ErrorResponse = null
                    }));

            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SupplierSystemUnavailable>();
        }
    }
}