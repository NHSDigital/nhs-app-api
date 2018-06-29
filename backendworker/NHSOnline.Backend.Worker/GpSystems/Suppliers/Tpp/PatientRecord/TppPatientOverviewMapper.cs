using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using Allergies = NHSOnline.Backend.Worker.Areas.MyRecord.Models.Allergies;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class TppPatientOverviewMapper
    {       
        public Tuple<Allergies, Medications> Map(ViewPatientOverviewReply viewPatientOverviewGetResponse)
        {
            var allergies = new Allergies();
            var allergiesData = new List<AllergyItem>();
            
            if (viewPatientOverviewGetResponse.Allergies != null)
            {
                var allergiesOverview = viewPatientOverviewGetResponse.Allergies;
                allergiesData.AddRange(allergiesOverview.Select(x => MapAllergyResponse(x)));
            }
            
            if (viewPatientOverviewGetResponse.DrugSensitivities != null)
            {
                var drugSensitivitiesOverview = viewPatientOverviewGetResponse.DrugSensitivities;
                allergiesData.AddRange(drugSensitivitiesOverview.Select(x => MapAllergyResponse(x)));
            }

            allergies.Data = allergiesData;
            
            var medications = new Medications
            {
                Data = new MedicationsData()
            };
            var acuteMedications = new List<MedicationItem>();
            var currentRepeatMedications = new List<MedicationItem>();
            var pastRepeatMedications = new List<MedicationItem>();
            
            if (viewPatientOverviewGetResponse.Drugs != null)
            {
                var medicationsOverview = viewPatientOverviewGetResponse.Drugs;
                acuteMedications.AddRange(medicationsOverview.Select(x => MapMedicationResponse(x)));
            }
            
            if (viewPatientOverviewGetResponse.CurrentRepeats != null)
            {
                var medicationsOverview = viewPatientOverviewGetResponse.CurrentRepeats;
                currentRepeatMedications.AddRange(medicationsOverview.Select(x => MapMedicationResponse(x)));
            }
            
            if (viewPatientOverviewGetResponse.PastRepeats != null)
            {
                var medicationsOverview = viewPatientOverviewGetResponse.PastRepeats;
                pastRepeatMedications.AddRange(medicationsOverview.Select(x => MapMedicationResponse(x)));
            }

            medications.Data.AcuteMedications = acuteMedications;
            medications.Data.CurrentRepeatMedications = currentRepeatMedications;
            medications.Data.DiscontinuedRepeatMedications = pastRepeatMedications;

            return new Tuple<Allergies, Medications>(allergies, medications);
        }

        private AllergyItem MapAllergyResponse(Item item)
        {
            return new AllergyItem
            {
                Name = item.Value,
                Date = new Date { Value = DateTimeOffset.Parse(item.Date) }
            };
        }

        private MedicationItem MapMedicationResponse(Item item)
        {       
            var result = new MedicationItem
            {
                Date = DateTimeOffset.Parse(item.Date) 
            };
            
            var medicationLineItems = new List<MedicationLineItem>();
            medicationLineItems.Add(new MedicationLineItem {Text = item.Value});
            result.LineItems = medicationLineItems;
            return result;
        }
    }
}