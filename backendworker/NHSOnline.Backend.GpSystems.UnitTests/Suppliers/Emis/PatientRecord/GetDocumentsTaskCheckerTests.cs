using System.Collections.Generic;
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
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class GetDocumentsTaskCheckerTests
    {
        private GetDocumentsTaskChecker _systemUnderTest;
        private IFixture _fixture;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<GetDocumentsTaskChecker>();
            _sampleSuccessStatusCodes = new List<HttpStatusCode>()
            {
                HttpStatusCode.OK
            };
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
        
        private Task<EmisApiObjectResponse<MedicationRootObject>> MedicationRootObjectResponse()
        {
            return Task.FromResult(
                new EmisApiObjectResponse<MedicationRootObject>(HttpStatusCode.OK, RequestsForSuccessOutcome.MedicalDocumentGet, _sampleSuccessStatusCodes)
                {
                    Body = null,
                    StatusCode = HttpStatusCode.OK
                });
        }
}
}