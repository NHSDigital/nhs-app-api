using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class EmisTestResultMapperTests
    {
        [TestMethod]
        public void MapTestResultsRequestsGetResponseToTestResultsListResponse_WithNullResponse_ThrowsNullReferenceException()
        {
            Action act = () => new EmisTestResultMapper().Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("testResultRequestsGetResponse");
        }
    }
}
