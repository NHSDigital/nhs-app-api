using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Im1Connection;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Microtest.Im1Connection
{
    [TestClass]
    public class MicrotestIm1ConnectionServiceTests
    {
        private const string DefaultConnectionToken = "936578f1-27bd-4669-970e-f24755e8725d";
        private const string DefaultOdsCode = "ods code";

        private IFixture _fixture;
        private MicrotestIm1ConnectionService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _systemUnderTest = _fixture.Create<MicrotestIm1ConnectionService>();
        }

        [TestMethod]
        public async Task Verify_ReturnsAConnection_WhenRequested()
        {
            // Act
            var result = await _systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            var successResult = result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SuccessfullyVerified>().Subject;

            successResult.Response.ConnectionToken.Should().Be(DefaultConnectionToken);
            successResult.Response.NhsNumbers.Should().BeEmpty();
        }
        
        [TestMethod]
        public async Task Register_SuccessfullyRegistered_WhenDataAreCorrect()
        {
            // Arrange
            var request = _fixture.Create<PatientIm1ConnectionRequest>();
            
            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            var successResult = result.Should().BeAssignableTo<Im1ConnectionRegisterResult.SuccessfullyRegistered>().Subject;
            successResult.Response.NhsNumbers.Should().BeEmpty();
            successResult.Response.ConnectionToken.Should().Be("{}");
        }
    }
}
