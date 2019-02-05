using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface ITppTestResultsMapper
    {
        TestResults Map(TestResultsViewReply testResultsViewReply);
    }

    public class TppTestResultsMapper : ITppTestResultsMapper
    {
        public TestResults Map(TestResultsViewReply testResultsViewReply)
        {
            if (testResultsViewReply == null)
            {
                throw new ArgumentNullException(nameof(testResultsViewReply));
            }

            var testResultData = new List<TestResultItem>();
            var testResults = new TestResults();

            if (testResultsViewReply.Items != null && testResultsViewReply.Items.Any())
            {
                foreach (var testResult in testResultsViewReply.Items)
                {
                    var newTestResultItem = new TestResultItem
                    {
                        Id = testResult.Id,
                        Date = new MyRecordDate
                            { Value = DateTimeOffset.Parse(testResult.Date, CultureInfo.InvariantCulture) },
                        Description = string.Format(CultureInfo.InvariantCulture, "{0} - {1}", testResult.Description,
                            testResult.Value)
                    };

                    testResultData.Add(newTestResultItem);
                }
            }

            testResults.Data = testResultData;
            return testResults;
        }
    }
}