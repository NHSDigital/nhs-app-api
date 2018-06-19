using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord.Model;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisMedicationMapper : IEmisMedicationMapper
    {
        private const string PRESCRIPTION_TYPE_ACUTE = "Acute";
        private const string PRESCRIPTION_TYPE_REPEAT = "Repeat";
        private const string DRUG_STATUS_ACTIVE = "Active";
        private const string DRUG_STATUS_CANCELLED = "Cancelled";
        private const string DATE_FORMAT = "d MMMM yyyy";

        public Medications Map(MedicationRootObject medicationGetResponse)
        {
            if (medicationGetResponse == null)
            {
                throw new ArgumentNullException(nameof(medicationGetResponse));
            }

            var result = new Medications();
            
            if (medicationGetResponse.MedicalRecord != null)
            {                
                var medicalRecord = medicationGetResponse.MedicalRecord;

                if (medicalRecord.Medication.Any())
                {
                    var acuteMedications = medicalRecord.Medication
                        .OrderByDescending(x => x.LastIssueDate)
                        .Where(x =>
                            x.PrescriptionType == PRESCRIPTION_TYPE_ACUTE &&
                            x.LastIssueDate > DateTimeOffset.Now.AddYears(-1));
                      
                    result.Data.AcuteMedications = acuteMedications.Select(MapMedicationResponse);
                    
                    var currentRepeatMedications = medicalRecord.Medication
                        .OrderByDescending(x => x.LastIssueDate)
                        .Where(x =>
                            x.PrescriptionType == PRESCRIPTION_TYPE_REPEAT &&
                            x.DrugStatus == DRUG_STATUS_ACTIVE);
                        
                    result.Data.CurrentRepeatMedications = currentRepeatMedications.Select(MapMedicationResponse);

                    var discontinuedRepeatMedications = medicalRecord.Medication
                        .OrderByDescending(x => x.LastIssueDate)
                        .Where(x =>
                            x.PrescriptionType == PRESCRIPTION_TYPE_REPEAT &&
                            x.DrugStatus == DRUG_STATUS_CANCELLED);
                    
                    result.Data.DiscontinuedRepeatMedications = discontinuedRepeatMedications.Select(MapMedicationResponse);
                }
            }

            return result;
        }

        private MedicationItem MapMedicationResponse(Medication responseItem)
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
            if (responseItem.LastIssueDate != null)
            {
                var lastIssueDate = new MedicationLineItem
                {
                    Text = "Ended: " + responseItem.LastIssueDate.Value.ToString(DATE_FORMAT)
                };
                medicationLineItems.Add(lastIssueDate);
            }
            result.LineItems = medicationLineItems;
            
            return result;
        }       
    }
}