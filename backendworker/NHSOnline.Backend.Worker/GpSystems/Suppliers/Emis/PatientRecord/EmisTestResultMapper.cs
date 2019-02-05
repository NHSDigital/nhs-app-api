using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisTestResultMapper
    {
        public TestResults Map(MedicationRootObject testResultRequestsGetResponse)
        {
            if (testResultRequestsGetResponse == null)
            {
                throw new ArgumentNullException(nameof(testResultRequestsGetResponse));
            }

            var testResults = new TestResults();

            if (testResultRequestsGetResponse.MedicalRecord == null)
            {
                return testResults;
            }

            var medicalRecord = testResultRequestsGetResponse.MedicalRecord;

            testResults.Data = (medicalRecord.TestResults ?? Enumerable.Empty<TestResult>())
                .Select(GetTestResultItem);

            return testResults;
        }

        private TestResultItem GetTestResultItem(TestResult response)
        {
            var testResultItem = new TestResultItem();

            if (response.Value == null)
            {
                return testResultItem;                
            }

            testResultItem.Date =
                new MyRecordDate
                {
                    Value = response.Value.EffectiveDate.Value,
                    DatePart = response.Value.EffectiveDate.DatePart
                };

            if (response.ChildValues == null || !response.ChildValues.Any())
            {
                var itemDescription = BuildItemDescriptionWithNoChildValues(response);
                testResultItem.Description = itemDescription;

                var associatedTexts = BuildAssociatedTextsWithNoChildValues(response);
                testResultItem.AssociatedTexts = associatedTexts;
            }
            else
            {
                testResultItem.Description = response.Value.Term;

                if (response?.ChildValues != null)
                {
                    foreach (var childValue in response.ChildValues)
                    {
                        var description = BuildDescriptionWithChildValues(childValue);
                        var associatedTexts = BuildAssociatedTextsWithChildValues(childValue);

                        testResultItem.TestResultChildLineItems.Add(new TestResultChildLineItem
                        {
                            Description = description,
                            AssociatedTexts = associatedTexts
                        });
                    }
                }
            }

            return testResultItem;
        }

        private static List<string> BuildAssociatedTextsWithChildValues(ChildValue childValue)
        {
            var associatedTexts = new List<string>();

            if (childValue?.AssociatedText != null)
            {
                foreach (var at in childValue.AssociatedText)
                {
                    associatedTexts.Add(at.Text.Replace("\t", string.Empty, StringComparison.Ordinal)
                        .Trim(new[] { '\n' })
                        .Replace("\n", "; ", StringComparison.Ordinal));
                }
            }

            return associatedTexts;
        }

        private static string BuildDescriptionWithChildValues(ChildValue childValue)
        {
            var lineItem = new StringBuilder();
            
            lineItem.Append(string.Format(CultureInfo.InvariantCulture, "{0}: {1} {2}", childValue.Term,
                childValue.TextValue, childValue.NumericUnits));

            if (childValue.Range != null)
            {
                lineItem.Append(
                    string.Format(CultureInfo.InvariantCulture, " (normal range: {0} - {1})",
                        childValue.Range.MinimumText,
                        childValue.Range.MaximumText));
            }

            return lineItem.ToString();
        }

        private static List<string> BuildAssociatedTextsWithNoChildValues(TestResult response)
        {
             var associatedTexts = new List<string>();

             if (response?.Value?.AssociatedText != null)
             {
                 foreach (var at in response.Value.AssociatedText)
                 {
                     associatedTexts.Add(at.Text.Replace("\t", string.Empty, StringComparison.Ordinal)
                         .Trim(new[] { '\n' })
                         .Replace("\n", "; ", StringComparison.Ordinal));
                 }
             }

            return associatedTexts;
        }

        private static string BuildItemDescriptionWithNoChildValues(TestResult response)
        {
            var term = new StringBuilder();
            
            term.Append(string.Format(CultureInfo.InvariantCulture, "{0}: {1} {2}",
                response.Value.Term, response.Value.TextValue, response.Value.NumericUnits));

            if (response?.Value?.Range != null)
            {
                term.Append(
                    string.Format(CultureInfo.InvariantCulture, " (normal range: {0} - {1})",
                        response.Value.Range.MinimumText,
                        response.Value.Range.MaximumText));
            }

            return term.ToString();
        }
    }
}
