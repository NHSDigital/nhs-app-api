using System;
using System.Collections.Generic;
using System.Globalization;
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
                allergiesData.AddRange(allergiesOverview.Select(MapAllergyResponse));
            }
            
            if (viewPatientOverviewGetResponse.DrugSensitivities != null)
            {
                var drugSensitivitiesOverview = viewPatientOverviewGetResponse.DrugSensitivities;
                allergiesData.AddRange(drugSensitivitiesOverview.Select(MapAllergyResponse));
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
                acuteMedications.AddRange(medicationsOverview.Select(MapMedicationResponse));
            }
            
            if (viewPatientOverviewGetResponse.CurrentRepeats != null)
            {
                var medicationsOverview = viewPatientOverviewGetResponse.CurrentRepeats;
                currentRepeatMedications.AddRange(medicationsOverview.Select(MapMedicationResponse));
            }
            
            if (viewPatientOverviewGetResponse.PastRepeats != null)
            {
                var medicationsOverview = viewPatientOverviewGetResponse.PastRepeats;
                pastRepeatMedications.AddRange(medicationsOverview.Select(MapMedicationResponse));
            }

            medications.Data.AcuteMedications = acuteMedications;
            medications.Data.CurrentRepeatMedications = currentRepeatMedications;
            medications.Data.DiscontinuedRepeatMedications = pastRepeatMedications;

            return new Tuple<Allergies, Medications>(allergies, medications);
        }

        private static AllergyItem MapAllergyResponse(ViewPatientOverViewItem item)
        {
            return new AllergyItem
            {
                Name = item.Value,
                Date = new MyRecordDate { Value = DateTimeOffset.Parse(item.Date, CultureInfo.InvariantCulture) }
            };
        }

        private static MedicationItem MapMedicationResponse(ViewPatientOverViewItem item)
        {       
            var result = new MedicationItem
            {
                Date = DateTimeOffset.Parse(item.Date, CultureInfo.InvariantCulture) 
            };
            
            var medicationLineItems = new List<MedicationLineItem>();
            medicationLineItems.Add(new MedicationLineItem {Text = item.Value});
            result.LineItems = medicationLineItems;
            return result;
        }
    }
}