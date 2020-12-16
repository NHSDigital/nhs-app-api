using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.CidApi.Areas.Registration;
using NHSOnline.Backend.CidApi.Areas.Registration.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.CidApi.UnitTests.Areas.Registration
{
    [TestClass]
    public sealed class RegistrationControllerTests : IDisposable
    {
        private const string DefaultOdsCode = "AB1234";
        private IFixture _fixture;
        private Mock<IOdsCodeMassager> _odsCodeMassager;
        private Mock<IOdsCodeLookup> _odsCodeLookup;
        private Mock<ILogger<RegistrationController>> _logger;
        private RegistrationController _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = new Mock<ILogger<RegistrationController>>();

            _odsCodeMassager = _fixture.Freeze<Mock<IOdsCodeMassager>>();
            _odsCodeMassager.Setup(x => x.CheckOdsCode(DefaultOdsCode))
                .Returns(DefaultOdsCode);
            _odsCodeLookup = new Mock<IOdsCodeLookup>();

            _systemUnderTest = new RegistrationController(
                _logger.Object,
                _odsCodeMassager.Object,
                _odsCodeLookup.Object);
        }

        [DataTestMethod]
        [DataRow("A29928", Supplier.Emis, "EMIS")]
        [DataRow("A55555", Supplier.Tpp, "TPP")]
        [DataRow("A77777", Supplier.Microtest, "Microtest")]
        [DataRow("A66666", Supplier.Vision, "Vision")]
        [DataRow("F00001", Supplier.Fake, "NhsTestGpSupplier")]
        public async Task RegistrationControllerGet_ReturnsCorrectGpSystem_IfSuccessful(string odsCode,
            Supplier supplier,
            string expectedGpSystem)
        {
            _odsCodeMassager.Setup(x => x.CheckOdsCode(odsCode))
                .Returns(odsCode);

            _odsCodeLookup.Setup(l => l.LookupSupplier(odsCode))
                .Returns(Task.FromResult(Option.Some(supplier)));

            var result = await _systemUnderTest.Get(odsCode);

            var resultValue = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            var gpSupplierResponseValue = resultValue.Should().BeAssignableTo<GpSystemSupplierResponse>();

            gpSupplierResponseValue.Subject.GpSystemSupplier.Should().Be(expectedGpSystem);
            gpSupplierResponseValue.Subject.OdsCode.Should().Be(odsCode);
            resultValue.Should().NotBeAssignableTo<GpSystemSupplierErrorResponse>();
        }

        [TestMethod]
        public async Task RegistrationControllerGet_ReturnsNotFound_IfNoGPSystem()
        {
            _odsCodeMassager.Setup(x => x.CheckOdsCode("000000"))
                .Returns("000000");
            _odsCodeLookup.Setup(l => l.LookupSupplier("000000"))
                .Returns(Task.FromResult(Option.None<Supplier>()));

            var result = await _systemUnderTest.Get("000000");

            var resultValue = result.Should().BeAssignableTo<NotFoundObjectResult>().Subject.Value;
            var gpSupplierResponseValue =
                resultValue.Should().BeAssignableTo<GpSystemSupplierErrorResponse>();

            gpSupplierResponseValue.Subject.ErrorReason.Should().Be("GP supplier for ODS Code: 000000 not found.");
            gpSupplierResponseValue.Subject.OdsCode.Should().Be("000000");
            resultValue.Should().NotBeAssignableTo<GpSystemSupplierResponse>();
        }

        [TestMethod]
        public async Task RegistrationControllerGet_ReturnsNotFound_IfGpSystemQualtrics()
        {
            _odsCodeMassager.Setup(x => x.CheckOdsCode("odsCode"))
                .Returns("odsCode");

            _odsCodeLookup.Setup(l => l.LookupSupplier("odsCode"))
                .Returns(Task.FromResult(Option.Some(Supplier.Qualtrics)));

            var result = await _systemUnderTest.Get("odsCode");

            var resultValue = result.Should().BeAssignableTo<NotFoundObjectResult>().Subject.Value;
            var gpSupplierResponseValue =
                resultValue.Should().BeAssignableTo<GpSystemSupplierErrorResponse>();

            gpSupplierResponseValue.Subject.ErrorReason.Should().Be("GP supplier for ODS Code: odsCode not found.");
            gpSupplierResponseValue.Subject.OdsCode.Should().Be("odsCode");
            resultValue.Should().NotBeAssignableTo<GpSystemSupplierResponse>();
        }

        [TestCleanup]
        public void Dispose() => _systemUnderTest?.Dispose();
    }
}