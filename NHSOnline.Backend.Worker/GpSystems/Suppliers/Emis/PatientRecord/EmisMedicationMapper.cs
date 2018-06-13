using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisMedicationMapper : IEmisMedicationMapper
    {       
        public const string PRESCRIPTION_TYPE_ACUTE = "Acute";
        public const string PRESCRIPTION_TYPE_REPEAT = "Repeat";
        public const string DRUG_STATUS_ACTIVE = "Active";
        public const string DRUG_STATUS_CANCELLED = "Cancelled";
        public const string DATE_FORMAT = "dd MMMM yyyy";

        public Medications Map(MedicationRequestsGetResponse medicationGetResponse)
        {
            if (medicationGetResponse == null)
            {
                throw new ArgumentNullException(nameof(medicationGetResponse));
            }

            var result = new Medications
            {
                Data = new MedicationsData
                {
                    AcuteMedications = new List<MedicationItem>(),
                    CurrentRepeatMedications = new List<MedicationItem>(),
                    DiscontinuedRepeatMedications = new List<MedicationItem>(),
                }
            };
            
            if (medicationGetResponse.MedicalRecord != null)
            {
                var medicalRecord = medicationGetResponse.MedicalRecord;

                if (medicalRecord.Medication.Any())
                {
                    var medications = medicalRecord.Medication.OrderByDescending(x => x.LastIssueDate);

                    result.Data.AcuteMedications = medications.Where(x =>
                        x.PrescriptionType == PRESCRIPTION_TYPE_ACUTE &&
                        x.LastIssueDate > DateTimeOffset.Now.AddYears(-1)).Select(x =>
                        MapMedicationResponse(x)
                    );
                    
                    result.Data.CurrentRepeatMedications = medications.Where(x =>
                        x.PrescriptionType == PRESCRIPTION_TYPE_REPEAT &&
                        x.DrugStatus == DRUG_STATUS_ACTIVE).Select(x =>
                        MapMedicationResponse(x)
                    );
                    
                    result.Data.DiscontinuedRepeatMedications = medications.Where(x =>
                        x.PrescriptionType == PRESCRIPTION_TYPE_REPEAT &&
                        x.DrugStatus == DRUG_STATUS_CANCELLED).Select(x =>
                        MapMedicationResponse(x)
                    );
                }
            }

            return result;
        }

        private MedicationItem MapMedicationResponse(MedicationResponse responseItem)
        {
            var result = new MedicationItem();
            result.Date = responseItem.FirstIssueDate;
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
            var lastIssueDate = new MedicationLineItem { Text = "Ended: " + responseItem.LastIssueDate.ToString(DATE_FORMAT) };
            medicationLineItems.Add(lastIssueDate);

            result.LineItems = medicationLineItems;
            return result;
        }       
    }
}