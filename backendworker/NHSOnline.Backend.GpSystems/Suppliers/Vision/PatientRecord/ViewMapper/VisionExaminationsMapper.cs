using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper
{
    public class VisionExaminationsMapper : IVisionMapper<Examinations>
    {
        private readonly ILogger<VisionExaminationsMapper> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public VisionExaminationsMapper(ILogger<VisionExaminationsMapper> logger, IHtmlSanitizer htmlSanitizer)
        {
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
        }

        public Examinations Map(VisionPatientDataResponse response)
        {
            var examinations = new Examinations();
            
            try
            {
                if (response != null && !string.IsNullOrEmpty(response.Record))
                {
                    var sanitizedHtml = _htmlSanitizer.SanitizeHtml(response.Record, null);
                    
                    sanitizedHtml = Regex.Replace(sanitizedHtml, @"((font-size:).*(!important;))", string.Empty);

                    examinations.RawHtml = sanitizedHtml;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error cleaning up HTML for Vision diagnosis. " + e.Message);
                examinations.HasErrored = true;
            }

            return examinations;
        }
    }
}