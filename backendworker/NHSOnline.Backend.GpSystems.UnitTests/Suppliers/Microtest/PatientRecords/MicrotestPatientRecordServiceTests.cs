using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.PatientRecords
{
    [TestClass]
    public class MicrotestPatientRecordServiceTests
    {
        private MicrotestPatientRecordService _systemUnderTest;
        private MicrotestUserSession _microtestUserSession;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization()); 
            _microtestUserSession = _fixture.Create<MicrotestUserSession>();
            _systemUnderTest = _fixture.Create<MicrotestPatientRecordService>();
        }

        [TestMethod]
        public async Task GetMyRecord_ReturnsSuccess_Microtest()
        {
            // Act
            var result = await _systemUnderTest.GetMyRecord(_microtestUserSession);
            
            result.Should().BeAssignableTo<GetMyRecordResult.Success>();
            ((GetMyRecordResult.Success) result).Response.Should().NotBeNull();
            ((GetMyRecordResult.Success) result).Response.HasDetailedRecordAccess.Should().BeTrue();
        }
    }
    
}