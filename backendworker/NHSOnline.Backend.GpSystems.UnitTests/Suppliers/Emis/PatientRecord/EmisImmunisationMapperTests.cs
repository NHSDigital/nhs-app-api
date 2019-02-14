using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
