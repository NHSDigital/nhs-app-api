using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Im1Connection;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Im1Connection;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Im1Connection
{
    [TestClass]
    public class Im1ConnectionControllerTests
    {
        private const string DefaultOdsCode = "AB1234";
        private const SupplierEnum DefaultSupplier = SupplierEnum.Emis;
        private const string DefaultPatientIdentifier = "XX00000A";
        private const string DefaultConnectionToken = "b2ed6831-cdd4-4ef7-a9b4-0880c2a35d78";

        private readonly ITokenValidationService _defaultTokenValidationService = new EmisTokenValidationService();
        private Im1ConnectionController _im1ConnectionController;

        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();

            _im1ConnectionController = CreateIm1ConnectionController();
        }

        [TestMethod]
        public void Constructor_NullOdsCodeLookup_Throws()
        {
            var bridgeFactory = MockBridgeFactory();

            Action act = () => new Im1ConnectionController(null, bridgeFactory.Object, new LoggerFactory());

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("odsCodeLookup");
        }

        [TestMethod]
        public void Constructor_NullBridgeFactory_Throws()
        {
            var odsCodeLookup = MockOdsCodeLookup();

            Action act = () => new Im1ConnectionController(odsCodeLookup.Object, null, new LoggerFactory());

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("bridgeFactory");
        }

        [TestMethod]
        public async Task Get_ReturnsABadRequestResult_WhenTheConnectionTokenIsNull()
        {
            var result = await _im1ConnectionController.Get(null, DefaultOdsCode);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Get_ReturnsABadRequestResult_WhenTheConnectionTokenIsEmpty()
        {
            var result = await _im1ConnectionController.Get(string.Empty, DefaultOdsCode);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Get_ReturnsABadRequestResult_WhenTheOdsCodeIsNull()
        {
            var result = await _im1ConnectionController.Get(DefaultConnectionToken, null);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Get_ReturnsABadRequestResult_WhenTheOdsCodeIsEmpty()
        {
            var result = await _im1ConnectionController.Get(DefaultConnectionToken, string.Empty);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [DataTestMethod]
        [DataRow("AB123")]
        [DataRow("AB12345")]
        [DataRow("!£$123HJ")]
        public async Task Get_ReturnsABadRequestResult_WhenTheOdsCodeIsInAnInvalidFormat(string badOdsCode)
        {
            var result = await _im1ConnectionController.Get(DefaultConnectionToken, badOdsCode);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Get_ReturnsTheSuccessResponse_WhenServiceIsSuccessfullyCalled()
        {
            const string odsCode = DefaultOdsCode;
            const SupplierEnum supplier = DefaultSupplier;
            const string patientIdentifier = DefaultConnectionToken;

            var expectedNhsNumbers = new[]
            {
                new PatientNhsNumber { NhsNumber = "123ABC" },
                new PatientNhsNumber { NhsNumber = "456DEF" }
            };

            var expectedResponse = new PatientIm1ConnectionResponse
            {
                ConnectionToken = DefaultConnectionToken,
                NhsNumbers = expectedNhsNumbers
            };

            var im1ConnectionService = MockIm1ConnectionService(patientIdentifier, odsCode,
                new Im1ConnectionVerifyResult.SuccessfullyVerified(expectedResponse));
            var bridgeMock = MockBridge(im1ConnectionService);
            var bridgeFactoryMock = MockBridgeFactory(supplier, bridgeMock);
            im1ConnectionService.Setup(x => x.Verify(DefaultConnectionToken, odsCode))
                .ReturnsAsync(new Im1ConnectionVerifyResult.SuccessfullyVerified(expectedResponse));
            _im1ConnectionController =
                CreateIm1ConnectionController(bridgeFactoryMock: bridgeFactoryMock);

            var result = await _im1ConnectionController.Get(DefaultConnectionToken, odsCode);

            var resultValue = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = resultValue.Should().BeAssignableTo<PatientIm1ConnectionResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task Get_UnknownOdsCode_ReturnsNotFound()
        {
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(DefaultOdsCode))
                .Returns(Task.FromResult(Option.None<SupplierEnum>()));

            _im1ConnectionController = CreateIm1ConnectionController(mockOdsCodeLookup);

            var result = await _im1ConnectionController.Get(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [TestMethod]
        public async Task Get_UnknownOdsCodeFormat_ReturnsBadRequest()
        {
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(DefaultOdsCode))
                .Returns(Task.FromResult(Option.Some(SupplierEnum.Emis)));

            _im1ConnectionController = CreateIm1ConnectionController(mockOdsCodeLookup);

            var result = await _im1ConnectionController.Get(DefaultConnectionToken, "foo");

            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_UnknownOdsCode_ReturnsNotFound()
        {
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(request.OdsCode))
                .Returns(Task.FromResult(Option.None<SupplierEnum>()));

            _im1ConnectionController = CreateIm1ConnectionController(mockOdsCodeLookup);

            var result = await _im1ConnectionController.Post(request);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        private Im1ConnectionController CreateIm1ConnectionController(Mock<IOdsCodeLookup> odsCodeLookupMock = null,
            Mock<IBridgeFactory> bridgeFactoryMock = null)
        {
            odsCodeLookupMock = odsCodeLookupMock ?? MockOdsCodeLookup();
            bridgeFactoryMock = bridgeFactoryMock ?? MockBridgeFactory();

            return new Im1ConnectionController(odsCodeLookupMock.Object, bridgeFactoryMock.Object, new LoggerFactory());
        }

        private static Mock<IOdsCodeLookup> MockOdsCodeLookup(string odsCode = DefaultOdsCode,
            SupplierEnum supplier = DefaultSupplier)
        {
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(odsCode)).Returns(Task.FromResult(Option.Some(supplier)));
            return mockOdsCodeLookup;
        }

        private Mock<IBridgeFactory> MockBridgeFactory(
            SupplierEnum supplier = DefaultSupplier,
            Mock<IBridge> bridgeMock = null)
        {
            bridgeMock = bridgeMock ?? MockBridge();
            var mockBridgeFactory = new Mock<IBridgeFactory>();
            mockBridgeFactory.Setup(x => x.CreateBridge(supplier)).Returns(bridgeMock.Object);

            return mockBridgeFactory;
        }

        private Mock<IBridge> MockBridge(
            Mock<IIm1ConnectionService> nhsNumberProvider = null,
            ITokenValidationService tokenValidationService = null
        )
        {
            nhsNumberProvider = nhsNumberProvider ?? MockIm1ConnectionService();
            tokenValidationService = tokenValidationService ?? _defaultTokenValidationService;

            var mockBridge = new Mock<IBridge>();
            mockBridge.Setup(x => x.GetIm1ConnectionService()).Returns(nhsNumberProvider.Object);
            mockBridge.Setup(x => x.GetTokenValidationService()).Returns(tokenValidationService);

            return mockBridge;
        }

        private static Mock<IIm1ConnectionService> MockIm1ConnectionService(
            string patientIdentifier = DefaultPatientIdentifier, string odsCode = DefaultOdsCode,
            Im1ConnectionVerifyResult expectedResponse = null)
        {
            var mockIm1ConnectionService = new Mock<IIm1ConnectionService>();
            mockIm1ConnectionService.Setup(x => x.Verify(patientIdentifier, odsCode))
                .ReturnsAsync(expectedResponse);

            return mockIm1ConnectionService;
        }
    }
}