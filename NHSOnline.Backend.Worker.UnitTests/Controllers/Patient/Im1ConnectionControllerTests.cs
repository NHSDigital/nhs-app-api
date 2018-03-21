using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Controllers.Patient;
using NHSOnline.Backend.Worker.Models.Patient;
using NHSOnline.Backend.Worker.Ods;
using NHSOnline.Backend.Worker.Suppliers;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.UnitTests.Controllers.Patient
{
    [TestClass]
    public class Im1ConnectionControllerTests
    {
        private const string DefaultOdsCode = "AB1234";
        private const SupplierEnum DefaultSupplier = SupplierEnum.Emis;
        private const string DefaultPatientIdentifier = "XX00000A";
        private const string DefaultConnectionToken = DefaultPatientIdentifier;

        private Im1ConnectionController sut;

        private static IFixture _fixture;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _fixture = new AutoFixture.Fixture();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            sut = CreateIm1ConnectionController();
        }

        [TestMethod]
        public void Constructor_NullOdsCodeLookup_Throws()
        {
            var systemProviderFactory = MockSystemProviderFactory();

            Action act = () => new Im1ConnectionController(null, systemProviderFactory.Object);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("odsCodeLookup");
        }

        [TestMethod]
        public void Constructor_NullSystemProviderFactory_Throws()
        {
            var odsCodeLookup = MockOdsCodeLookup();

            Action act = () => new Im1ConnectionController(odsCodeLookup.Object, null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("systemProviderFactory");
        }

        [TestMethod]
        public async Task Get_ReturnsABadRequestResult_WhenTheConnectionTokenIsNull()
        {
            var result = await sut.Get(null, DefaultOdsCode);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Get_ReturnsABadRequestResult_WhenTheConnectionTokenIsEmpty()
        {
            var result = await sut.Get(string.Empty, DefaultOdsCode);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Get_ReturnsABadRequestResult_WhenTheOdsCodeIsNull()
        {
            var result = await sut.Get(DefaultConnectionToken, null);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Get_ReturnsABadRequestResult_WhenTheOdsCodeIsEmpty()
        {
            var result = await sut.Get(DefaultConnectionToken, string.Empty);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [DataTestMethod]
        [DataRow("AB123")]
        [DataRow("AB12345")]
        [DataRow("!£$123HJ")]
        public async Task Get_ReturnsABadRequestResult_WhenTheOdsCodeIsInAnInvalidFormat(string badOdsCode)
        {
            var result = await sut.Get(DefaultConnectionToken, badOdsCode);

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
                new PatientNhsNumber {NhsNumber = "123ABC"},
                new PatientNhsNumber {NhsNumber = "456DEF"}
            };

            var expectedResponse = new PatientIm1ConnectionResponse
            {
                ConnectionToken = DefaultConnectionToken,
                NhsNumbers = expectedNhsNumbers
            };

            var im1ConnectionService = MockIm1ConnectionService(patientIdentifier, odsCode, new Im1ConnectionVerifyResult.SuccessfullyVerified(expectedResponse));
            var systemProviderMock = MockSystemProvider(im1ConnectionService);
            var systemProviderFactoryMock = MockSystemProviderFactory(supplier, systemProviderMock);
            im1ConnectionService.Setup(x => x.Verify(DefaultConnectionToken, odsCode))
                .ReturnsAsync(new Im1ConnectionVerifyResult.SuccessfullyVerified(expectedResponse));
            sut =
                CreateIm1ConnectionController(systemProviderFactoryMock: systemProviderFactoryMock);

            var result = await sut.Get(DefaultConnectionToken, odsCode);

            result.Should().BeAssignableTo<OkObjectResult>();
            // ReSharper disable once PossibleNullReferenceException
            var responseObject = (result as OkObjectResult).Value;
            responseObject.Should().BeAssignableTo<PatientIm1ConnectionResponse>();
            responseObject.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task Get_UnknownOdsCode_ReturnsNotFound()
        {
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(DefaultOdsCode))
                .Returns(Task.FromResult(Option.None<SupplierEnum>()));

            sut = CreateIm1ConnectionController(mockOdsCodeLookup);

            var result = await sut.Get(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [TestMethod]
        public async Task Post_UnknownOdsCode_ReturnsNotFound()
        {
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(DefaultOdsCode))
                .Returns(Task.FromResult(Option.None<SupplierEnum>()));

            sut = CreateIm1ConnectionController(mockOdsCodeLookup);

            var result = await sut.Post(request);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        private Im1ConnectionController CreateIm1ConnectionController(Mock<IOdsCodeLookup> odsCodeLookupMock = null,
            Mock<ISystemProviderFactory> systemProviderFactoryMock = null)
        {
            odsCodeLookupMock = odsCodeLookupMock ?? MockOdsCodeLookup();
            systemProviderFactoryMock = systemProviderFactoryMock ?? MockSystemProviderFactory();

            return new Im1ConnectionController(odsCodeLookupMock.Object, systemProviderFactoryMock.Object);
        }

        private Mock<IOdsCodeLookup> MockOdsCodeLookup(string odsCode = DefaultOdsCode,
            SupplierEnum supplier = DefaultSupplier)
        {
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(odsCode)).Returns(Task.FromResult(Option.Some(supplier)));

            return mockOdsCodeLookup;
        }

        private Mock<ISystemProviderFactory> MockSystemProviderFactory(SupplierEnum supplier = DefaultSupplier,
            Mock<ISystemProvider> systemProviderMock = null)
        {
            systemProviderMock = systemProviderMock ?? MockSystemProvider();
            var mockSystemProviderFactory = new Mock<ISystemProviderFactory>();
            mockSystemProviderFactory.Setup(x => x.CreateSystemProvider(supplier)).Returns(systemProviderMock.Object);

            return mockSystemProviderFactory;
        }

        private Mock<ISystemProvider> MockSystemProvider(Mock<IIm1ConnectionService> nhsNumberProvider = null)
        {
            nhsNumberProvider = nhsNumberProvider ?? MockIm1ConnectionService();
            var mockSystemProvider = new Mock<ISystemProvider>();
            mockSystemProvider.Setup(x => x.GetIm1ConnectionService()).Returns(nhsNumberProvider.Object);

            return mockSystemProvider;
        }

        private Mock<IIm1ConnectionService> MockIm1ConnectionService(string patientIdentifier = DefaultPatientIdentifier, string odsCode = DefaultOdsCode,
            Im1ConnectionVerifyResult expectedResponse = null)
        {
            var mockIm1ConnectionService = new Mock<IIm1ConnectionService>();
            mockIm1ConnectionService.Setup(x => x.Verify(patientIdentifier, odsCode))
                .ReturnsAsync(expectedResponse);

            return mockIm1ConnectionService;
        }
    }
}