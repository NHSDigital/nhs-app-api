using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper
{
    public class VisionImmunisationsMapper : IVisionImmunisationsMapper
    {
        readonly ILogger _logger;

        public VisionImmunisationsMapper(ILogger logger) {
            _logger = logger;
        }

        public Immunisations Map(VisionPatientDataResponse response)
        {
            var result = new Immunisations();

            List<ImmunisationItem> immunisations = new List<ImmunisationItem>();
            String rawContent = response.Record;

            if(!String.IsNullOrEmpty(rawContent)) {

                try
                {
                    Root parsedContent = rawContent.DeserializeXml<Root>();

                    foreach (var clinical in parsedContent.Patient.Clinicals)
                    {
                        if (clinical.SubGroupCode.Equals("Immunisation", StringComparison.OrdinalIgnoreCase)){
                            immunisations.Add(new ImmunisationItem
                            {
                                Term = clinical.ReadTerm,
                                EffectiveDate = new MyRecordDate
                                {
                                    Value = clinical.EventDate,
                                    DatePart = "Unknown"
                                }
                            });
                        }
                    }
                    result.Data = immunisations;
                }
                catch(InvalidOperationException e) {
                    _logger.LogWarning("Error deserializing raw Immunisations response content string. " + e.Message);
                    result.HasErrored = true;
                }
            }
            return result;
        }
    }
}