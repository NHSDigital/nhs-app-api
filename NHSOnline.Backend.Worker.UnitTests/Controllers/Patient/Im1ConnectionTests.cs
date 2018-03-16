using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Controllers.Patient;
using NHSOnline.Backend.Worker.Models.Patient;
using NHSOnline.Backend.Worker.Ods;
using NHSOnline.Backend.Worker.Suppliers;

namespace NHSOnline.Backend.Worker.UnitTests.Controllers.Patient
{
    [TestClass]
    public class Im1ConnectionTests
    {
        private const string DefaultOdsCode = "AB1234";
        private const SupplierEnum DefaultSupplier = SupplierEnum.Emis;
        private const string DefaultPatientIdentifier = "XX00000A";
        private const string DefaultConnectionToken = DefaultPatientIdentifier;

        private Im1ConnectionController _im1ConnectionController;

        [TestInitialize]
        public void TestInitialize()
        {
            _im1ConnectionController = CreateIm1ConnectionController();
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
        public async Task Get_ReturnsTheNhsNumberAssociatedWithTheSuppliedToken_WhenValidlyRequested()
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

            var nhsNumberProvider = MockNhsNumberProvider(patientIdentifier, expectedNhsNumbers);
            var systemProviderMock = MockSystemProvider(nhsNumberProvider);
            var systemProviderFactoryMock = MockSystemProviderFactory(supplier, systemProviderMock);
            nhsNumberProvider.Setup(x => x.GetNhsNumbersAsync(DefaultConnectionToken, odsCode))
                .ReturnsAsync(expectedNhsNumbers);
            _im1ConnectionController =
                CreateIm1ConnectionController(systemProviderFactoryMock: systemProviderFactoryMock);

            var result = await _im1ConnectionController.Get(DefaultConnectionToken, odsCode);

            result.Should().BeAssignableTo<OkObjectResult>();
            // ReSharper disable once PossibleNullReferenceException
            var responseObject = (result as OkObjectResult).Value;
            responseObject.Should().BeAssignableTo<PatientIm1ConnectionResponse>();
            responseObject.Should().BeEquivalentTo(expectedResponse);
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
            mockOdsCodeLookup.Setup(x => x.LookupSupplier(odsCode)).Returns(Task.FromResult(supplier));

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

        private Mock<ISystemProvider> MockSystemProvider(Mock<INhsNumberProvider> nhsNumberProvider = null)
        {
            nhsNumberProvider = nhsNumberProvider ?? MockNhsNumberProvider();
            var mockSystemProvider = new Mock<ISystemProvider>();
            mockSystemProvider.Setup(x => x.GetNhsNumberProvider()).Returns(nhsNumberProvider.Object);

            return mockSystemProvider;
        }

        private Mock<INhsNumberProvider> MockNhsNumberProvider(string patientIdentifier = DefaultPatientIdentifier,
            IEnumerable<PatientNhsNumber> expectedNhsNumbers = null)
        {
            expectedNhsNumbers = expectedNhsNumbers ?? new List<PatientNhsNumber>();
            var mockNhsNumberProvider = new Mock<INhsNumberProvider>();
            mockNhsNumberProvider.Setup(x => x.GetNhsNumbersAsync(patientIdentifier, ""))
                .ReturnsAsync(expectedNhsNumbers);

            return mockNhsNumberProvider;
        }
    }
}