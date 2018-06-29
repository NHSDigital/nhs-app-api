using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Linkage;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Linkage
{
    [TestClass]
    public class LinkageControllerTests
    {
        private const string DefaultOdsCode = "AB1234";
        private const SupplierEnum DefaultSupplier = SupplierEnum.Emis;
        private const string DefaultNhsNumber = "XX00000A";

        private LinkageController _linkageController;

        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();

            _linkageController = CreateLinkageController();
        }
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Get_ReturnsABadRequestResult_WhenTheOdsCodeIsNullOrEmpty(string odsCode)
        {
            var result = await _linkageController.Get(DefaultNhsNumber, odsCode);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Get_ReturnsABadRequestResult_WhenTheNhsNumberIsNullOrEmpty(string nhsNumber)
        {
            var result = await _linkageController.Get(nhsNumber, DefaultOdsCode);

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Get_UnknownOdsCode_Returns501NotImplemented()
        {
            // Arrage
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(DefaultOdsCode))
                .Returns(Task.FromResult(Option.None<SupplierEnum>()));

            _linkageController = CreateLinkageController(mockOdsCodeLookup);

            // Act
            var result = await _linkageController.Get(DefaultNhsNumber, DefaultOdsCode);

            // Assert
            var resultAsStatusCodeResult = result as StatusCodeResult;
            resultAsStatusCodeResult.Should().NotBeNull();
            resultAsStatusCodeResult.StatusCode.Should().Be(StatusCodes.Status501NotImplemented);
        }

        [TestMethod]
        public async Task Get_ReturnsTheSuccessResponse_WhenServiceIsSuccessfullyCalled()
        {
            const SupplierEnum supplier = DefaultSupplier;

            var expectedResponse = _fixture.Create<LinkageResponse>();

            var linkageService = MockLinkageService(new GetLinkageResult.SuccessfullyRetrieved(expectedResponse));
            var gpSystemMock = MockGpSystem(linkageService);
            var gpSystemFactoryMock = MockGpSystemFactory(supplier, gpSystemMock);

            _linkageController = CreateLinkageController(gpSystemFactoryMock: gpSystemFactoryMock);

            // Act
            var result = await _linkageController.Get(DefaultNhsNumber, DefaultOdsCode);

            // Assert
            linkageService.Verify(x => x.GetLinkageKey(DefaultNhsNumber, DefaultOdsCode), Times.Once);
            var resultValue = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = resultValue.Should().BeAssignableTo<LinkageResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_ReturnsABadRequestResult_WhenTheOdsCodeIsNullOrEmpty(string odsCode)
        {
            // Arrange
            var request = new CreateLinkageRequest
            {
                NhsNumber = DefaultNhsNumber,
                OdsCode = odsCode,
            };

            // Act
            var result = await _linkageController.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Post_ReturnsABadRequestResult_WhenTheNhsNumberIsNullOrEmpty(string nhsNumber)
        {
            // Arrange
            var request = new CreateLinkageRequest
            {
                NhsNumber = nhsNumber,
                OdsCode = DefaultOdsCode,
            };

            // Act
            var result = await _linkageController.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task Post_UnknownOdsCode_Returns501NotImplemented()
        {
            // Arrage
            var mockOdsCodeLookup = new Mock<IOdsCodeLookup>();
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(DefaultOdsCode))
                .Returns(Task.FromResult(Option.None<SupplierEnum>()));

            _linkageController = CreateLinkageController(mockOdsCodeLookup);

            // Act
            var result = await _linkageController.Get(DefaultNhsNumber, DefaultOdsCode);

            // Assert
            var resultAsStatusCodeResult = result as StatusCodeResult;
            resultAsStatusCodeResult.Should().NotBeNull();
            resultAsStatusCodeResult.StatusCode.Should().Be(StatusCodes.Status501NotImplemented);
        }

        [TestMethod]
        public async Task Post_ReturnsTheSuccessResponse_WhenServiceIsSuccessfullyCalled()
        {
            const string nhsNumber = DefaultNhsNumber;
            const string odsCode = DefaultOdsCode;
            const SupplierEnum supplier = DefaultSupplier;

            var expectedResponse = _fixture.Create<LinkageResponse>();

            var mockResult = new CreateLinkageResult.SuccessfullyRetrieved(expectedResponse);

            var mockLinkageService = new Mock<ILinkageService>();
            mockLinkageService.Setup(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req => req.NhsNumber == nhsNumber && req.OdsCode == odsCode))
            ).ReturnsAsync(mockResult);

            var gpSystemMock = MockGpSystem(mockLinkageService);
            var gpSystemFactoryMock = MockGpSystemFactory(supplier, gpSystemMock);

            _linkageController = CreateLinkageController(gpSystemFactoryMock: gpSystemFactoryMock);

            var request = new CreateLinkageRequest
            {
                NhsNumber = nhsNumber,
                OdsCode = odsCode,
            };

            // Act
            var result = await _linkageController.Post(request);

            // Assert
            mockLinkageService.Verify(x => x.CreateLinkageKey(
                It.Is<CreateLinkageRequest>(req => req.NhsNumber == nhsNumber && req.OdsCode == odsCode)), Times.Once);
            var resultValue = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var actualResponse = resultValue.Should().BeAssignableTo<LinkageResponse>().Subject;
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        private LinkageController CreateLinkageController(
            Mock<IOdsCodeLookup> odsCodeLookupMock = null,
            Mock<IGpSystemFactory> gpSystemFactoryMock = null)
        {
            odsCodeLookupMock = odsCodeLookupMock ?? MockOdsCodeLookup();
            gpSystemFactoryMock = gpSystemFactoryMock ?? MockGpSystemFactory();
            var logger = new LoggerFactory();

            return new LinkageController(logger, gpSystemFactoryMock.Object, odsCodeLookupMock.Object);
        }

        private static Mock<IOdsCodeLookup> MockOdsCodeLookup(
            string odsCode = DefaultOdsCode,
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
            Mock<ILinkageService> linkageService = null)
        {
            linkageService = linkageService ?? MockLinkageService();

            var mockGpSystem = new Mock<IGpSystem>();
            mockGpSystem.Setup(x => x.GetLinkageService()).Returns(linkageService.Object);

            return mockGpSystem;
        }

        private static Mock<ILinkageService> MockLinkageService(
            GetLinkageResult expectedResult = null)
        {
            var mockLinkageService = new Mock<ILinkageService>();
            mockLinkageService.Setup(x => x.GetLinkageKey(DefaultNhsNumber, DefaultOdsCode)).ReturnsAsync(expectedResult);

            return mockLinkageService;
        }
    }
}