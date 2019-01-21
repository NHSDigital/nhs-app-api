using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper
{
    public class VisionTestResultsMapper : IVisionMapper<TestResults>
    {
        private readonly ILogger _logger;

        public VisionTestResultsMapper(ILogger logger)
        {
            _logger = logger;
        }

        public TestResults Map(VisionPatientDataResponse response)
        {
            var testResults = new TestResults();
            
            try
            {
                var html = response.Record;
                
                html = html.Replace("background: #d9f0ff;", "background: #f0f4f5; color: #212b32;",
                    StringComparison.InvariantCultureIgnoreCase);
                html = html.Replace("<h3", "<h4", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("</h3>", "</h4>", StringComparison.InvariantCultureIgnoreCase);
                html = html.Replace("tbody tr {", "tbody tr { line-height: 1.5em !important; ",
                    StringComparison.InvariantCultureIgnoreCase);

                var resultsCount = html.Split("inps_class_single_result_frame")
                    .Skip(1)
                    .Count(part => !part.Contains("inps_class_no_data_found", StringComparison.InvariantCultureIgnoreCase));

                if (resultsCount > 0)
                {
                    testResults.RawHtml = html;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error cleaning up HTML for Vision test results. " + e.Message);
                testResults.HasErrored = true;
            }

            return testResults;
        }
    }
}
