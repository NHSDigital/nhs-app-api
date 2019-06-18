using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class EmisImmunisationMapperTests
    {
        private EmisImmunisationMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new EmisImmunisationMapper();
        }

        [TestMethod]
        public void MapImmunisationRequestsGetResponseToImmunisationListResponse_WithNullResponse_ThrowsNullReferenceException()
        {
            Action act = () => _systemUnderTest.Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("immunisationsGetResponse");
        }
        
        [TestMethod]
        public void MapImmunisationRequestsGetResponseToImmunisationListResponse_WithNullTerm_GivesEmptyResponse()
        {
            var immunisations = new List<Immunisation>();
            immunisations.Add(new Immunisation
            {
                Term = null,
                EffectiveDate = new EffectiveDate
                {
                    DatePart = "testDatePart",
                    Value = new DateTime(2019, 6, 15, 3, 30, 0)
                }   
            });
            var mappedImmunisationList = _systemUnderTest.Map(new MedicationRootObject
            {
                MedicalRecord = new MedicalRecord
                {
                    Immunisations = immunisations
                }
            });
            mappedImmunisationList.Data.Should().BeEmpty();
        }
        
        [TestMethod]
        public void MapImmunisationRequestsGetResponseToImmunisationListResponse_WithNullEffectiveDate_GivesResponseWithOnlyTerm()
        {
            var immunisations = new List<Immunisation>();
            immunisations.Add(new Immunisation
            {
                Term = "testImmunisation",
                EffectiveDate = null
            });
            var mappedImmunisationList = _systemUnderTest.Map(new MedicationRootObject
            {
                MedicalRecord = new MedicalRecord
                {
                    Immunisations = immunisations
                }
            });
            mappedImmunisationList.Data.FirstOrDefault().Term.Should().Be("testImmunisation");
            mappedImmunisationList.Data.FirstOrDefault().EffectiveDate.DatePart.Should().BeNull();
            mappedImmunisationList.Data.FirstOrDefault().EffectiveDate.Value.Should().BeNull();
        }
    }
}
