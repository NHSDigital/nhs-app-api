using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisTestResultMapper
    {
        public TestResults Map(MedicationRootObject testResultRequestsGetResponse)
        {
            var testResults = new TestResults();

            if (testResultRequestsGetResponse.MedicalRecord == null) return testResults;
            
            var medicalRecord = testResultRequestsGetResponse.MedicalRecord;

            testResults.Data = (medicalRecord.TestResults ?? Enumerable.Empty<TestResult>())
                .Select(GetTestResultItem);

            return testResults;
        }

        private TestResultItem GetTestResultItem(TestResult response)
        {
            var testResultItem = new TestResultItem();

            if (response.Value == null)
                return testResultItem;

            testResultItem.Date =
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

                testResultItem.Description = term.ToString();
                testResultItem.AssociatedTexts = response.Value.AssociatedText?.Any() == true
                    ? (from at in response.Value.AssociatedText
                        select at.Text.Replace("\t", string.Empty)
                            .Trim(new[] { '\n' })
                            .Replace("\n", "; ")).ToList()
                    : new List<string>();
            }
            else
            {
                testResultItem.Description = response.Value.Term;
                foreach (var childValue in response.ChildValues)
                {
                    var lineItem = new StringBuilder();
                    
                    lineItem.Append(string.Format("{0}: {1} {2}", childValue.Term, childValue.TextValue, childValue.NumericUnits));
                    
                    if (childValue.Range != null)
                    {
                        lineItem.Append(
                            string.Format(" (normal range: {0} - {1})", childValue.Range.MinimumText,
                                childValue.Range.MaximumText));
                    }

                    testResultItem.TestResultChildLineItems.Add(new TestResultChildLineItem
                    {
                        Description = lineItem.ToString(),
                        AssociatedTexts = childValue.AssociatedText?.Any() == true ?
                            (from at in childValue.AssociatedText
                            select at.Text.Replace("\t", string.Empty)
                                    .Trim(new[] { '\n' })
                                    .Replace("\n", "; ")).ToList() : new List<string>()
                    });                      
                }
           }

            return testResultItem;
        }
    }
}