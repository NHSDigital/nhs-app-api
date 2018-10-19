using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper
{
    public class VisionAllergyMapper : IVisionMapper<Allergies>
    {
        private readonly ILogger _logger;

        public VisionAllergyMapper(ILogger logger) {
            _logger = logger;
        }

        public Allergies Map(VisionPatientDataResponse response)
        {
            var result = new Allergies();

            var allergies = new List<AllergyItem>();
            var rawContent = response.Record;

            if (string.IsNullOrEmpty(rawContent)) return result;
            
            try
            {
                var parsedContent = rawContent.DeserializeXml<Root>();

                foreach (var clinical in parsedContent.Patient.Clinicals)
                {
                    allergies.Add(new AllergyItem
                    {
                        Name = clinical.ReadTerm,
                        Date = new MyRecordDate
                        {
                            Value =DateTime.TryParse(clinical.EventDate, out var eventDate) ?
                                eventDate : (DateTimeOffset?)null,
                            DatePart = "Unknown"
                        },
                        Reaction = clinical.ReadTerm2,
                        Drug = clinical.DrugTerm
                    });
                }

                result.Data = allergies;
            }
            catch(InvalidOperationException e) {
                _logger.LogWarning("Error deserializing raw Allergies response content string. " + e.Message);
                result.HasErrored = true;
            }

            return result;
        }
    }
}
