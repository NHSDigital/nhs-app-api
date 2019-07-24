using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper
{
    public class VisionProceduresMapper : IVisionMapper<Procedures>
    {
        private readonly ILogger<VisionProceduresMapper> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public VisionProceduresMapper(ILogger<VisionProceduresMapper> logger, IHtmlSanitizer htmlSanitizer)
        {
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
        }

        public Procedures Map(VisionPatientDataResponse response)
        {
            var procedures = new Procedures();
            
            try
            {
                if (response != null && !string.IsNullOrEmpty(response.Record))
                {
                    var sanitizedHtml = _htmlSanitizer.SanitizeHtml(response.Record, null);
                    
                    sanitizedHtml = Regex.Replace(sanitizedHtml, @"((font-size:).*(!important;))", string.Empty);

                    procedures.RawHtml = sanitizedHtml;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("Error cleaning up HTML for Vision diagnosis. " + e.Message);
                procedures.HasErrored = true;
            }

            return procedures;
        }
    }
}