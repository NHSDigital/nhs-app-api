using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Im1Connection;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
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
            var gpSystemFactory = MockGpSystemFactory();

            Action act = () => new Im1ConnectionController(null, gpSystemFactory.Object, new LoggerFactory());

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("odsCodeLookup");
        }

        [TestMethod]
        public void Constructor_NullGpSystemFactoryFactory_Throws()
        {
            var odsCodeLookup = MockOdsCodeLookup();

            Action act = () => new Im1ConnectionController(odsCodeLookup.Object, null, new LoggerFactory());

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("gpSystemFactory");
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
            var gpSystemMock = MockGpSystem(im1ConnectionService);
            var gpSystemFactoryMock = MockGpSystemFactory(supplier, gpSystemMock);
            im1ConnectionService.Setup(x => x.Verify(DefaultConnectionToken, odsCode))
                .ReturnsAsync(new Im1ConnectionVerifyResult.SuccessfullyVerified(expectedResponse));
            _im1ConnectionController =
                CreateIm1ConnectionController(gpSystemFactoryMock: gpSystemFactoryMock);

            var result = await _im1ConnectionController.Get(DefaultConnectionToken, odsCode);

            var resultValue = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = resultValue.Should().BeAssignableTo<PatientIm1ConnectionResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task Get_UnknownOdsCode_Returns501NotImplemented()
        {
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(DefaultOdsCode))
                .Returns(Task.FromResult(Option.None<SupplierEnum>()));

            _im1ConnectionController = CreateIm1ConnectionController(mockOdsCodeLookup);

            var result = await _im1ConnectionController.Get(DefaultConnectionToken, DefaultOdsCode);

            var resultAsStatusCodeResult = result as StatusCodeResult;
            resultAsStatusCodeResult.Should().NotBeNull();
            resultAsStatusCodeResult.StatusCode.Should().Be(StatusCodes.Status501NotImplemented);
        }


        [TestMethod]
        public async Task Post_UnknownOdsCode_ReturnsNotImplemented()
        {
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(request.OdsCode))
                .Returns(Task.FromResult(Option.None<SupplierEnum>()));

            _im1ConnectionController = CreateIm1ConnectionController(mockOdsCodeLookup);

            var result = await _im1ConnectionController.Post(request) as StatusCodeResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status501NotImplemented);
        }

        private Im1ConnectionController CreateIm1ConnectionController(Mock<IOdsCodeLookup> odsCodeLookupMock = null,
            Mock<IGpSystemFactory> gpSystemFactoryMock = null)
        {
            odsCodeLookupMock = odsCodeLookupMock ?? MockOdsCodeLookup();
            gpSystemFactoryMock = gpSystemFactoryMock ?? MockGpSystemFactory();

            return new Im1ConnectionController(odsCodeLookupMock.Object, gpSystemFactoryMock.Object, new LoggerFactory());
        }

        private static Mock<IOdsCodeLookup> MockOdsCodeLookup(string odsCode = DefaultOdsCode,
            SupplierEnum supplier = DefaultSupplier)
        {
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(odsCode)).Returns(Task.FromResult(Option.Some(supplier)));
            return mockOdsCodeLookup;
        }

        private Mock<IGpSystemFactory> MockGpSystemFactory(
            SupplierEnum supplier = DefaultSupplier,
            Mock<IGpSystem> gpSystemMock = null)
        {
            gpSystemMock = gpSystemMock ?? MockGpSystem();
            var mockGpSystemFactory = new Mock<IGpSystemFactory>();
            mockGpSystemFactory.Setup(x => x.CreateGpSystem(supplier)).Returns(gpSystemMock.Object);

            return mockGpSystemFactory;
        }

        private Mock<IGpSystem> MockGpSystem(
            Mock<IIm1ConnectionService> nhsNumberProvider = null,
            ITokenValidationService tokenValidationService = null
        )
        {
            nhsNumberProvider = nhsNumberProvider ?? MockIm1ConnectionService();
            tokenValidationService = tokenValidationService ?? _defaultTokenValidationService;

            var mockGpSystem = new Mock<IGpSystem>();
            mockGpSystem.Setup(x => x.GetIm1ConnectionService()).Returns(nhsNumberProvider.Object);
            mockGpSystem.Setup(x => x.GetTokenValidationService()).Returns(tokenValidationService);

            return mockGpSystem;
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