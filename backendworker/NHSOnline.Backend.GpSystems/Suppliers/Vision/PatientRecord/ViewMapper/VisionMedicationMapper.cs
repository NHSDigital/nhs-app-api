using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper
{
    public class VisionMedicationMapper : IVisionMapper<Medications>
    {
        private readonly ILogger<VisionMedicationMapper> _logger;
        private const string Acute = "Acute";
        private const string CurrentRepeat = "CurrentRepeat";
        private const string DiscontinuedRepeat = "DiscontinuedRepeat";
        
        public VisionMedicationMapper(ILogger<VisionMedicationMapper> logger)
        {
            _logger = logger;
        }

        public Medications Map(VisionPatientDataResponse response)
        {
            var medications = new Medications();

            if(response == null)
            {
                return medications;
            }

            var rawContent = response.Record;

            try
            {
                if (string.IsNullOrEmpty(rawContent)) 
                    return medications;
                
                var parsedContent = rawContent.DeserializeXml<Root>();

                var acuteMedications = parsedContent.Patient.Clinicals
                    .Where(x => x.SubGroupCode.Equals(Acute, StringComparison.OrdinalIgnoreCase) &&
                                DateTime.TryParse(x.EventDate, out var eventDate) 
                                && eventDate.Date >= DateTime.Now.AddYears(-1).Date);

                medications.Data.AcuteMedications = 
                    acuteMedications.Select(MapMedicationResponse);

                var currentRepeatMedications =
                    parsedContent.Patient.Clinicals.Where(
                        x => x.SubGroupCode.Equals(CurrentRepeat, StringComparison.OrdinalIgnoreCase));

                medications.Data.CurrentRepeatMedications =
                    currentRepeatMedications.Select(MapMedicationResponse);
                
                var discontinuedRepeatMedications = parsedContent.Patient.Clinicals
                    .Where(x => x.SubGroupCode.Equals(DiscontinuedRepeat, StringComparison.OrdinalIgnoreCase) &&
                                DateTime.TryParse(x.LastPrescribedDate,  out var lastPrescribedDate) 
                                && lastPrescribedDate.Date >= DateTime.Now.AddMonths(-6).Date);

                medications.Data.DiscontinuedRepeatMedications =
                    discontinuedRepeatMedications.Select(MapMedicationResponse);

                return medications;
            }
            catch(InvalidOperationException e) {
                _logger.LogWarning("Error deserializing raw Medications response content string. " + e.Message);
                medications.HasErrored = true;

                return medications;
            }
        }
        
        private static MedicationItem MapMedicationResponse(Clinical clinical)
        {       
            return new MedicationItem
            {
                Date = clinical.SubGroupCode.Equals(Acute, StringComparison.OrdinalIgnoreCase) ? 
                    (DateTime.TryParse(clinical.EventDate, out var eventDate) ? 
                        eventDate.Date : (DateTime?)null) :
                    DateTime.TryParse(clinical.FirstPrescribedDate, out var firstPrescribedDate) 
                        ? firstPrescribedDate.Date : (DateTime?)null
                ,
                LineItems = new List<MedicationLineItem>
                {
                    new MedicationLineItem { Text = clinical.DrugTerm },
                    new MedicationLineItem { Text = clinical.Dosage },
                    new MedicationLineItem { Text = $"{clinical.Quantity} {clinical.PackSize}" }
                }
            };
        }
    }
}