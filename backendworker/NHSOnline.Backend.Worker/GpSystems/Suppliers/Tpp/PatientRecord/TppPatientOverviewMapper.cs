using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using Allergies = NHSOnline.Backend.Worker.Areas.MyRecord.Models.Allergies;
using NHSOnline.Backend.Worker.Support.Logging;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public class TppPatientOverviewMapper
    {       
        private readonly ILogger<GetPatientOverviewTaskChecker> _logger;
        
        public TppPatientOverviewMapper(ILogger<GetPatientOverviewTaskChecker> logger)
        {
            _logger = logger;
        }
        
        public Tuple<Allergies, Medications> Map(ViewPatientOverviewReply viewPatientOverviewGetResponse)
        {
            _logger.LogEnter();
            
            var allergies = new Allergies();
            var allergiesData = new List<AllergyItem>();
            
            if (viewPatientOverviewGetResponse.Allergies != null)
            {
                _logger.LogDebug("TPP: Mapping patient allergies from allergies response");
                var allergiesOverview = viewPatientOverviewGetResponse.Allergies;
                allergiesData.AddRange(allergiesOverview.Select(MapAllergyResponse));
            }
            
            if (viewPatientOverviewGetResponse.DrugSensitivities != null)
            {
                _logger.LogDebug("TPP: Mapping patient allergies from drug sensitivities response");
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
                _logger.LogDebug("TPP: Mapping patient acute medications from drugs response");
                var medicationsOverview = viewPatientOverviewGetResponse.Drugs;
                acuteMedications.AddRange(medicationsOverview.Select(MapMedicationResponse));
            }
            
            if (viewPatientOverviewGetResponse.CurrentRepeats != null)
            {
                _logger.LogDebug("TPP: Mapping patient current repeat medications from current repeats response");
                var medicationsOverview = viewPatientOverviewGetResponse.CurrentRepeats;
                currentRepeatMedications.AddRange(medicationsOverview.Select(MapMedicationResponse));
            }
            
            if (viewPatientOverviewGetResponse.PastRepeats != null)
            {
                _logger.LogDebug("TPP: Mapping patient discontinued medications from past repeats response");
                var medicationsOverview = viewPatientOverviewGetResponse.PastRepeats;
                pastRepeatMedications.AddRange(medicationsOverview.Select(MapMedicationResponse));
            }

            medications.Data.AcuteMedications = acuteMedications;
            medications.Data.CurrentRepeatMedications = currentRepeatMedications;
            medications.Data.DiscontinuedRepeatMedications = pastRepeatMedications;

            _logger.LogExit();
            return new Tuple<Allergies, Medications>(allergies, medications);
        }

        private static AllergyItem MapAllergyResponse(ViewPatientOverViewItem item)
        {
            return new AllergyItem
            {
                Name = item.Value,
                Date = new MyRecordDate { Value = item.Date.SafeParseToNullableDateTimeOffset() }
            };
        }

        private static MedicationItem MapMedicationResponse(ViewPatientOverViewItem item)
        {       
            var result = new MedicationItem
            {
                Date = item.Date.SafeParseToNullableDateTimeOffset() 
            };
            
            var medicationLineItems = new List<MedicationLineItem>();
            medicationLineItems.Add(new MedicationLineItem {Text = item.Value});
            result.LineItems = medicationLineItems;
            return result;
        }
    }
}