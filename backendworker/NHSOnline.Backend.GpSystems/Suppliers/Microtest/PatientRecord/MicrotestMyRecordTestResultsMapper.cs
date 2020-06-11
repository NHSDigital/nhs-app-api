using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    internal sealed class MicrotestMyRecordTestResultsMapper
    {
        private readonly ILogger _logger;

        public MicrotestMyRecordTestResultsMapper(
            ILogger<MicrotestMyRecordTestResultsMapper> logger)
        {
            _logger = logger;
        }

        internal void Map(MyRecordResponse myRecordResponse, TestResultData testResultData)
        {
            if (testResultData?.TestResult != null)
            {
                var testResultItems = new List<TestResultItem>();

                testResultItems.AddRange(MapInrResults(testResultData.TestResult.InrResultsData));
                testResultItems.AddRange(MapPathResults(testResultData.TestResult.PathResultsData));

                myRecordResponse.TestResults.Data = testResultItems;

                myRecordResponse.TestResults.HasUndeterminedAccess =
                    !testResultData.TestResult.InrResultsData.InrResults.Any() &&
                    !testResultData.TestResult.PathResultsData.PathResults.Any();
            }
        }

        private static List<TestResultItem> MapInrResults(InrResultData inrResultsData)
        {
            var inrItems = new List<TestResultItem>();

            if (inrResultsData != null)
            {
                foreach (var inrResult in inrResultsData.InrResults)
                {
                    var associatedTexts = new List<string>();
                    associatedTexts.Add($"INR Results: {inrResult.Value} (target - {inrResult.Target})");
                    associatedTexts.Add($"Condition: {inrResult.CodeDescription}");
                    associatedTexts.Add($"Therapy: {inrResult.Therapy}");
                    associatedTexts.Add($"Dose: {inrResult.Dose}");
                    associatedTexts.Add($"Next test date: {inrResult.NextTestDate}");

                    inrItems.Add(new TestResultItem
                    {
                        Date = new MyRecordDate
                        {
                            Value = DateTime.TryParse(inrResult.RecordDateTime, out var outDate)
                                ? outDate
                                : (DateTimeOffset?)null,
                            DatePart = "Unknown"
                        },
                        AssociatedTexts = associatedTexts
                    });
                }
            }

            inrItems = inrItems.OrderByDescending(i => i.Date?.Value.GetValueOrDefault()).ToList();

            return inrItems;
        }

        private List<TestResultItem> MapPathResults(PathResultData pathResultsData)
        {
            var pathItems = new List<TestResultItem>();

            if (pathResultsData != null)
            {
                var pathResults = pathResultsData.PathResults.Where(NotAwaitingResults).ToList();

                var removedCount = pathResultsData.PathResults.Count - pathResults.Count;
                if (removedCount != 0)
                {
                    _logger.LogInformation(
                        $"{removedCount} items filtered out of PathResults due to value stored in Status field.");
                }

                foreach (var pathResult in pathResults)
                {
                    var associatedTexts = new List<string>
                    {
                        $"{pathResult.Name}: {pathResult.ElementName}",
                        $"Value: {pathResult.Value}",
                        $"Units: {pathResult.Units}"
                    };

                    pathItems.Add(new TestResultItem
                    {
                        Date = new MyRecordDate
                        {
                            Value = DateTime.TryParse(pathResult.RecordDate, out var outDate)
                                ? outDate
                                : (DateTimeOffset?)null,
                            DatePart = "Unknown"
                        },
                        AssociatedTexts = associatedTexts
                    });
                }
            }

            return pathItems.OrderByDescending(i => i.Date?.Value.GetValueOrDefault()).ToList();

            bool NotAwaitingResults(PathResult pr) => !string.Equals(pr.Status, TestResultStatus.AwaitingResults, StringComparison.OrdinalIgnoreCase);
        }
    }
}