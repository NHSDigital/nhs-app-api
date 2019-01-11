using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class EmisTestResultMapperTests
    {
        private EmisTestResultMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new EmisTestResultMapper();
        }

        [TestMethod]
        public void MapTestResultsRequestsGetResponseToTestResultsListResponse_WithNullResponse_ThrowsNullReferenceException()
        {
            Action act = () => _systemUnderTest.Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("testResultRequestsGetResponse");
        }
    }
}
