using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class TppDetailedTestResultMapper : ITppDetailedTestResultMapper
    {
        private readonly ILogger<TppDetailedTestResultMapper> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;
        
        public TppDetailedTestResultMapper(ILogger<TppDetailedTestResultMapper> logger, IHtmlSanitizer htmlSanitizer)
        {
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
        }
        
        public TestResultResponse Map(TestResultsViewReply testResultsViewReply)
        {
            if (testResultsViewReply == null)
            {
                throw new ArgumentNullException(nameof(testResultsViewReply));
            }

            try
            {
                var testResultHtml = testResultsViewReply.Items?.FirstOrDefault()?.Value;
                
                var sanitizedHtml = _htmlSanitizer.SanitizeHtml(testResultHtml);

                return new TestResultResponse
                {
                    TestResult = sanitizedHtml
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Error cleaning up HTML for TPP test results. " + e.Message);
                return new TestResultResponse
                {
                    HasErrored = true
                };
            }

        }
    }
}