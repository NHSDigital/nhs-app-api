using System;
using System.Linq;
using System.Text;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord.Medication;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisTestResultMapper
    {
        public TestResults Map(MedicationRootObject testResultRequestsGetResponse)
        {
            var testResults = new TestResults();
            
            if (testResultRequestsGetResponse.MedicalRecord != null)
            {
                var medicalRecord = testResultRequestsGetResponse.MedicalRecord;

                testResults.Data = (medicalRecord.TestResults ?? Enumerable.Empty<TestResult>())
                    .Select(GetTestResultItem);
            }

            return testResults;
        }

        private TestResultItem GetTestResultItem(TestResult response)
        {
            var testResultItem = new TestResultItem();

            if (response.Value == null)
                return testResultItem;

            testResultItem.EffectiveDate =
                new Date { Value = response.Value.EffectiveDate.Value, 
                           DatePart = response.Value.EffectiveDate.DatePart };
            
            if (response.ChildValues == null || !response.ChildValues.Any())
            {
                var term = new StringBuilder();
                term.Append(string.Format("{0}: {1} {2}",
                    response.Value.Term, response.Value.TextValue, response.Value.NumericUnits));

                if (response.Value.Range != null)
                {
                    term.Append(
                        string.Format(" (normal range: {0} - {1})", response.Value.Range.MinimumText,
                            response.Value.Range.MaximumText));
                }

                testResultItem.Term = term.ToString();
            }
            else
            {
                testResultItem.Term = response.Value.Term;
                foreach (var lineitem in response.ChildValues)
                {
                    var lineItem = new StringBuilder();
                    
                    lineItem.Append(string.Format("{0}: {1} {2}", lineitem.Term, lineitem.TextValue, lineitem.NumericUnits));
                    
                    if (lineitem.Range != null)
                    {
                        lineItem.Append(
                            string.Format(" (normal range: {0} - {1})", lineitem.Range.MinimumText,
                                lineitem.Range.MaximumText));
                    }
                    
                    testResultItem.TestResultLineItems.Add(lineItem.ToString());
                }
           }

            return testResultItem;
        }
    }
}