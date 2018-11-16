using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
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

            return new TestResults
            {    
                Data = testResultsViewReply.Items != null ?
                        (from testResult in testResultsViewReply.Items
                            select new TestResultItem
                            {
                                Id = testResult.Id,
                                Date = new MyRecordDate { Value = DateTimeOffset.Parse(testResult.Date, CultureInfo.InvariantCulture) },
                                Description = string.Format(CultureInfo.InvariantCulture,"{0} - {1}", testResult.Description, testResult.Value),
                            }
                        ).ToList() : new List<TestResultItem>()
            };
        }     
    }
}