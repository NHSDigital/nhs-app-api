using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper
{
    public class VisionImmunisationsMapper : IVisionMapper<Immunisations>
    {
        private readonly ILogger _logger;

        public VisionImmunisationsMapper(ILogger logger) {
            _logger = logger;
        }

        public Immunisations Map(VisionPatientDataResponse response)
        {
            var immunisations = new Immunisations();

            var immunisationItems = new List<ImmunisationItem>();
            var rawContent = response.Record;

            if (string.IsNullOrEmpty(rawContent)) 
                return immunisations;
            
            try
            {
                var parsedContent = rawContent.DeserializeXml<Root>();

                foreach (var clinical in parsedContent.Patient.Clinicals)
                {
                    if (clinical.SubGroupCode.Equals("Immunisation", StringComparison.OrdinalIgnoreCase)){
                        immunisationItems.Add(new ImmunisationItem
                        {
                            Term = clinical.ReadTerm,
                            EffectiveDate = new MyRecordDate
                            {
                                Value = DateTime.TryParse(clinical.EventDate, out var eventDate) ?
                                        eventDate : (DateTimeOffset?)null,
                                DatePart = "Unknown"
                            }
                        });
                    }
                }
                immunisations.Data = immunisationItems;
            }
            catch(InvalidOperationException e) {
                _logger.LogWarning("Error deserializing raw Immunisations response content string. " + e.Message);
                immunisations.HasErrored = true;
            }
            return immunisations;
        }
    }
}