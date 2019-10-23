using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class GetDocumentsTaskCheckerTests
    {
        private GetDocumentsTaskChecker _systemUnderTest;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<GetDocumentsTaskChecker>();
        }

        [TestMethod]
        public void GetMyRecord_ReturnsHasErrors_WhenUnSuccessfulDocumentsResponseFromEmis()
        {

            // Act
            var result = _systemUnderTest.Check(MedicationRootObjectResponse());
            
            result.Should().BeAssignableTo<PatientDocuments>().Subject.HasErrored.Should().BeTrue();

            result.Should().BeAssignableTo<PatientDocuments>()
                .Should().NotBeNull();
        }
        
        private Task<EmisClient.EmisApiObjectResponse<MedicationRootObject>> MedicationRootObjectResponse()
        {
            return Task.FromResult(
                new EmisClient.EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK)
                {
                    Body = null,
                    StatusCode = HttpStatusCode.OK
                });
        }
}
}