using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper
{
    public class VisionTestResultsMapper : IVisionMapper<TestResults>
    {
        private readonly ILogger<VisionTestResultsMapper> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;
        
        public VisionTestResultsMapper(ILogger<VisionTestResultsMapper> logger, IHtmlSanitizer htmlSanitizer)
        {
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
        }

        public TestResults Map(VisionPatientDataResponse response)
        {
            try
            {
                var testResults = new TestResults();

                if (string.IsNullOrEmpty(response.Record))
                {
                    return testResults;
                }
                
                var sanitizedHtml = _htmlSanitizer.SanitizeHtml(response.Record);
                    
                var formattedHtml = Regex.Replace(sanitizedHtml, @"((background:).*(;))", string.Empty);
                formattedHtml = Regex.Replace(formattedHtml, @"((font-size:).*(!important;))", string.Empty);
                formattedHtml = formattedHtml.Replace("tbody tr {", "tbody tr { line-height: 1.5em !important; ",
                    StringComparison.InvariantCultureIgnoreCase);

                var resultsCount = formattedHtml.Split("inps_class_single_result_frame")
                    .Skip(1)
                    .Count(part =>
                        !part.Contains("inps_class_no_data_found", StringComparison.InvariantCultureIgnoreCase));

                if (resultsCount > 0)
                {
                    testResults.RawHtml = formattedHtml;
                }

                return testResults;
            }
            catch (Exception e)
            {
                _logger.LogError("Error cleaning up HTML for Vision test results. " + e.Message);
                return new TestResults
                {
                    HasErrored = true
                };
            }
        }
    }
}
