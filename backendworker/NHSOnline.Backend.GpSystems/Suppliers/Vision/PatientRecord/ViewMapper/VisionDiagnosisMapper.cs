using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper
{
    public class VisionDiagnosisMapper : IVisionMapper<Diagnosis>
    {
        private readonly ILogger<VisionDiagnosisMapper> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public VisionDiagnosisMapper(ILogger<VisionDiagnosisMapper> logger, IHtmlSanitizer htmlSanitizer)
        {
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
        }

        public Diagnosis Map(VisionPatientDataResponse response)
        {
            var diagnosis = new Diagnosis();

            try
            {
                if (response != null && !string.IsNullOrEmpty(response.Record))
                {
                    var sanitizedHtml = _htmlSanitizer.SanitizeHtml(response.Record);

                    var splitByNoDataFound = sanitizedHtml.Split("class=\"inps_class_no_data_found\"");

                    if (splitByNoDataFound.Length >= 3)
                    {
                        return diagnosis;
                    }

                    sanitizedHtml = Regex.Replace(sanitizedHtml, @"((background:).*(;))", string.Empty);
                    sanitizedHtml = Regex.Replace(sanitizedHtml, @"((font-size:).*(!important;))", string.Empty);
                    sanitizedHtml = sanitizedHtml.Replace("tbody tr {", "tbody tr { line-height: 1.5em !important; ",
                        StringComparison.InvariantCultureIgnoreCase);

                    diagnosis.RawHtml = sanitizedHtml;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error cleaning up HTML for Vision diagnosis. " + e.Message);
                diagnosis.HasErrored = true;
            }

            return diagnosis;
        }
    }
}