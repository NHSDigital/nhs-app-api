using System;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class TppDetailedTestResultMapper : ITppDetailedTestResultMapper
    {
        public TestResultResponse Map(TestResultsViewReply testResultsViewReply)
        {
            if (testResultsViewReply == null)
            {
                throw new ArgumentNullException(nameof(testResultsViewReply));
            }

            return new TestResultResponse
            {
                TestResult = testResultsViewReply.Items
                             != null
                    ? testResultsViewReply.Items.Select(item => item.Value).FirstOrDefault()
                    : string.Empty
            };
        }
    }
}