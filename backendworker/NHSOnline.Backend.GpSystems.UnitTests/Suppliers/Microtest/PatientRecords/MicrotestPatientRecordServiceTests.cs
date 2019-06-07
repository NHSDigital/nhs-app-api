using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.PatientRecords
{
    [TestClass]
    public class MicrotestPatientRecordServiceTests
    {
        private MicrotestPatientRecordService _microtestPatientRecordService;
        private MicrotestUserSession _microtestUserSession;
        private IFixture _fixture;
        private Mock<IMicrotestClient> _microtestClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization()); 
            _microtestUserSession = _fixture.Create<MicrotestUserSession>();
            _microtestClient = _fixture.Freeze<Mock<IMicrotestClient>>();
            
            _microtestPatientRecordService = _fixture.Create<MicrotestPatientRecordService>();
        }

        [TestMethod]
        public async Task GetMyRecord_ReturnsSuccess_Microtest()
        {
            var patientRecordGetResponse = _fixture.Create<PatientRecordGetResponse>();

            _microtestClient.Setup(x => x.MedicalRecordGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber))
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<PatientRecordGetResponse>(HttpStatusCode.OK)
                    {
                        Body = patientRecordGetResponse,
                    }));
            
            // Act
            var result = await _microtestPatientRecordService.GetMyRecord(_microtestUserSession);
            
            result.Should().BeAssignableTo<GetMyRecordResult.Success>();
            ((GetMyRecordResult.Success) result).Response.Should().NotBeNull();
        }
    }
    
}