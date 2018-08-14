using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisConsulationMapper
    {
        private const string Observation = "Observation";
        private const string Unknown = "UnKnown";
        
        public Consultations Map(MedicationRootObject consultationsGetResponse)
        {
            var consultations = new Consultations();

            if (consultationsGetResponse.MedicalRecord == null) return consultations;
            
            var medicalRecord = consultationsGetResponse.MedicalRecord;

            consultations.Data = (medicalRecord.Consultations ?? Enumerable.Empty<Consultation>())
                .Select(GetConsultationItem);

            return consultations;
        }

        private ConsultationItem GetConsultationItem(Consultation response)
        {
            var consultationItem = new ConsultationItem
            {
                EffectiveDate = new MyRecordDate
                {
                    Value = response.EffectiveDate.Value,
                    DatePart = response.EffectiveDate.DatePart
                },
                ConsultantLocation = 
                    !string.IsNullOrEmpty(response.Location) ?
                        $"{response.Location} - {response.ConsultantName}" :
                        response.ConsultantName,
                ConsultationHeaders = response.Sections != null ? (
                    from section in response.Sections
                    where section.Observations != null &&
                          section.Observations.Any(
                              (obs => 
                                  FilterObservationTypes(obs) &&
                                  (!string.IsNullOrEmpty(obs.Term) ||
                                  (obs.AssociatedText != null && obs.AssociatedText.Any(at =>!string.IsNullOrEmpty(at.Text))))))                         
                    select new ConsultationHeaderItem
                    {
                        Header = section.Header,
                        ObservationsWithTerm = 
                            (from obs in section.Observations
                            where obs != null 
                                  && !string.IsNullOrEmpty(obs.Term)
                                  && FilterObservationTypes(obs)
                            select new ObservationItemWithTerm
                            {
                                Term = obs.Term,
                                AssociatedTexts = obs.AssociatedText != null ? (from obsText in obs.AssociatedText
                                                  where !string.IsNullOrEmpty(obsText.Text)
                                                  select GetAssociatedText(obsText)).ToList() 
                                : new List<string>()
                            }).ToList(),
                        AssociatedTexts =  
                            (from obs in section.Observations
                             where obs != null 
                                   && string.IsNullOrEmpty(obs.Term)
                                   && FilterObservationTypes(obs)
                                   && obs.AssociatedText != null
                             from associatedText in obs.AssociatedText
                             where associatedText != null && !string.IsNullOrEmpty(associatedText.Text)                             
                             select GetAssociatedText(associatedText)).ToList()
                    }).ToList() : new List<ConsultationHeaderItem>()
            };
            
            return consultationItem;
        }

        private static bool FilterObservationTypes(Observation obs)
        {
            return !string.IsNullOrEmpty(obs.ObservationType) &&
                   (obs.ObservationType.Equals(Observation, StringComparison.OrdinalIgnoreCase) ||
                    obs.ObservationType.Equals(Unknown, StringComparison.OrdinalIgnoreCase));
        }
        
        private static string GetAssociatedText(AssociatedText associatedText)
        {
            return associatedText?.Text
                .Replace("\t", string.Empty, StringComparison.Ordinal)
                .Trim(new[] { '\n' })
                .Replace("\n", "; ", StringComparison.OrdinalIgnoreCase);
        }
    }
}