using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using NHSOnline.IntegrationTestAnalyser.Models;
using Schemas;
using Formatting = Newtonsoft.Json.Formatting;

namespace NHSOnline.IntegrationTestAnalyser
{
    internal class Analyser
    {
        private IEnumerable<string> FilePaths { get; }

        public Analyser(IEnumerable<string> filePaths)
        {
            FilePaths = filePaths;
        }

        [SuppressMessage("ReSharper", "CA5369",  Justification = "Safe for means of analyse")]
        public Task Analyse()
        {
            XmlSerializer xmlSerializer = new(typeof(TestRunType));

            var totalTestCount = 0;
            var manualTestCount = 0;

            Collection<CustomTestResult> customTestResults = new();
            foreach (var filePath in FilePaths)
            {
                TestRunType testRunType;
                using (StreamReader fileStreamReader = new(filePath))
                {
                    testRunType = (TestRunType)xmlSerializer.Deserialize(fileStreamReader)!;
                }

                foreach (object run in testRunType.Items)
                {
                    if (run is not ResultsType resultsType)
                    {
                        continue;
                    }

                    foreach (object result in resultsType.Items)
                    {
                        totalTestCount++;
                        if (!(result is UnitTestResultType unitTestResultType &&
                              (unitTestResultType.outcome.Equals("NotExecuted", StringComparison.Ordinal) ||
                               unitTestResultType.outcome.Equals("Failed", StringComparison.Ordinal))))
                        {
                            continue;
                        }

                        var customTestResult =
                            new CustomTestResult(unitTestResultType.testId, unitTestResultType.outcome, unitTestResultType.startTime);

                        foreach (object output in unitTestResultType.Items)
                        {
                            if (output is not OutputType outputType)
                            {
                                continue;
                            }

                            if (outputType.StdOut is XmlNode[] xmlNodes)
                            {
                                foreach (XmlNode xmlNode in xmlNodes)
                                {
                                    if (xmlNode.Value == null ||
                                        xmlNode.Value.IndexOf("TestReport:", StringComparison.Ordinal) <= 0)
                                    {
                                        continue;
                                    }
                                    var jsonString =
                                        xmlNode.Value[
                                            (xmlNode.Value.IndexOf("TestReport:", StringComparison.Ordinal) + 11)..];

                                    customTestResult.TestReport = JsonConvert.DeserializeObject<CustomTestReport>(
                                        jsonString);
                                }
                            }

                            if (outputType.StdErr is XmlNode[] errorXmlNodes)
                            {
                                foreach (XmlNode xmlNode in errorXmlNodes)
                                {
                                    customTestResult.ErrorList.Add(xmlNode.Value!);
                                }
                            }

                            if (outputType.ErrorInfo?.Message is XmlNode[] errorInfoMessageXmlNodes)
                            {
                                foreach (XmlNode xmlNode in errorInfoMessageXmlNodes)
                                {
                                    customTestResult.ErrorList.Add(xmlNode.Value!);
                                }
                            }

                            if (outputType.ErrorInfo?.StackTrace is XmlNode[] errorInfoStacktraceXmlNodes)
                            {
                                foreach (XmlNode xmlNode in errorInfoStacktraceXmlNodes)
                                {
                                    customTestResult.ErrorList.Add(xmlNode.Value!);
                                }
                            }
                        }

                        if (!customTestResult.IsManual)
                        {
                            customTestResults.Add(customTestResult);
                        }
                        else
                        {
                            manualTestCount++;
                        }
                    }
                }
            }

            var categorisation = GetCategorisation(customTestResults);

            var automatedTestCount = totalTestCount - manualTestCount;

            Display(automatedTestCount, customTestResults, categorisation);

            return Task.CompletedTask;
        }

        private void Display(int totalTestCount, Collection<CustomTestResult> customTestResults, Dictionary<string, List<string>> categorisation){
            var runTimes = customTestResults.Select(result => result.RunTime).ToList();
            Console.WriteLine($"Date range: {runTimes.Min()} to {runTimes.Max()}");

            Console.WriteLine($"{customTestResults.Count} failures from {totalTestCount} tests");

            Console.WriteLine($"==========================");
            Console.WriteLine($"   Category Counts");

            foreach (var category in categorisation.OrderBy(x => x.Value.Count))
            {
                Console.WriteLine($"\"{category.Key}\": {category.Value.Count}");
            }

            var jsonResultsWithUnknownCategory = JsonConvert.SerializeObject(customTestResults.Where(result => result.Category.Equals("UNKNOWN", StringComparison.Ordinal)), Formatting.Indented);

            Console.WriteLine($"==========================");
            Console.WriteLine($"   Unknown Error Breakdown");
            Console.WriteLine(jsonResultsWithUnknownCategory);

            var byCategoryJson = JsonConvert.SerializeObject(categorisation, Formatting.Indented);

            Console.WriteLine($"==========================");
            Console.WriteLine($"   Category Browserstack Session Ids");
            Console.WriteLine(byCategoryJson);
        }

        private static Dictionary<string, List<string>> GetCategorisation(IEnumerable<CustomTestResult> customTestResults)
        {
            Dictionary<string, List<string>> categorisation = new();

            foreach (var result in customTestResults.OrderBy(result => result.TestReport?.MethodName))
            {
                var browserStackSessionId = result.TestReport?.BrowserStackSessionId;

                var resultOutput = $"{browserStackSessionId}";

                if (categorisation.ContainsKey(result.Category))
                {
                    categorisation[result.Category].Add(resultOutput);
                }
                else
                {
                    categorisation[result.Category] = new List<string> {resultOutput};
                }
            }

            return categorisation;
        }
    }
}