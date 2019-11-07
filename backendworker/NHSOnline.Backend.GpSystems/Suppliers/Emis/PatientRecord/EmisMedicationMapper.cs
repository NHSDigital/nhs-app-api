using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisMedicationMapper : IEmisMedicationMapper
    {
        private const string PrescriptionTypeAcute = "Acute";
        private const string PrescriptionTypeRepeat = "Repeat";
        private const string DrugStatusActive = "Active";
        private const string DrugStatusCancelled = "Cancelled";
        private const string DateFormat = "d MMMM yyyy";

        public Medications Map(MedicationRootObject medicationRootObject)
        {
            if (medicationRootObject == null)
            {
                throw new ArgumentNullException(nameof(medicationRootObject));
            }

            var result = new Medications();
            
            if (medicationRootObject.MedicalRecord != null)
            {
                var medicalRecord = medicationRootObject.MedicalRecord;

                if (medicalRecord.Medication.Any())
                {
                    var acuteMedications = medicalRecord.Medication
                        .OrderByDescending(x => x.LastIssueDate)
                        .Where(x => PrescriptionTypeAcute.Equals(x.PrescriptionType, StringComparison.Ordinal));
                      
                    result.Data.AcuteMedications = acuteMedications.Select(MapMedicationResponse);

                    var currentRepeatMedications = medicalRecord.Medication
                        .OrderByDescending(x => x.LastIssueDate)
                        .Where(x => PrescriptionTypeRepeat.Equals(x.PrescriptionType, StringComparison.Ordinal) &&
                                    DrugStatusActive.Equals(x.DrugStatus, StringComparison.Ordinal));
                        
                    result.Data.CurrentRepeatMedications = currentRepeatMedications.Select(MapMedicationResponse);

                    var discontinuedRepeatMedications = medicalRecord.Medication
                        .OrderByDescending(x => x.LastIssueDate)
                        .Where(x => PrescriptionTypeRepeat.Equals(x.PrescriptionType, StringComparison.Ordinal) &&
                                    DrugStatusCancelled.Equals(x.DrugStatus, StringComparison.Ordinal));
                    
                    result.Data.DiscontinuedRepeatMedications = discontinuedRepeatMedications.Select(MapMedicationResponse);
                }
            }

            return result;
        }

        private static MedicationItem MapMedicationResponse(Medication responseItem)
        {            
            var result = new MedicationItem
            {
                Date = responseItem.FirstIssueDate
            };
            
            var medicationLineItems = new List<MedicationLineItem>();
            
            var drugName = new MedicationLineItem { Text = responseItem.Term };
            medicationLineItems.Add(drugName);
            
            if (responseItem.IsMixture)
            {
                var mixture = new MedicationLineItem { Text = responseItem.Mixture.MixtureName + ", consisting of:" };
                var mixtureLineItems = new List<string>();
                
                foreach (var constituent in responseItem.Mixture.Constituents)
                {
                    mixtureLineItems.Add(constituent.ConstituentName + " - " + constituent.Strength);
                }

                mixture.LineItems = mixtureLineItems;
                medicationLineItems.Add(mixture);
            }
            
            var dosage = new MedicationLineItem { Text = responseItem.Dosage };
            medicationLineItems.Add(dosage);
            var quantityRepresentation = new MedicationLineItem { Text = responseItem.QuantityRepresentation };
            medicationLineItems.Add(quantityRepresentation);
            if (PrescriptionTypeRepeat.Equals(responseItem.PrescriptionType, StringComparison.Ordinal) &&
                DrugStatusCancelled.Equals(responseItem.DrugStatus, StringComparison.Ordinal) &&
                responseItem.LastIssueDate != null)
            {
                var lastIssueDate = new MedicationLineItem
                {
                    Text = "Ended: " + responseItem.LastIssueDate.Value.ToString(DateFormat, CultureInfo.InvariantCulture)
                };
                medicationLineItems.Add(lastIssueDate);
            }
            result.LineItems = medicationLineItems;
            
            return result;
        }       
    }
}