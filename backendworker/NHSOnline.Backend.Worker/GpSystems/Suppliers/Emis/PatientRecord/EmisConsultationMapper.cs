using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisConsulationMapper
    {
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
                EffectiveDate = new Date
                {
                    Value = response.EffectiveDate.Value,
                    DatePart = response.EffectiveDate.DatePart
                },
                ConsultantLocation = $"{response.Location}, {response.ConsultantName}",
                ConsultationHeaders = response.Sections != null ? (from section in response.Sections
                    select new ConsultationHeaderItem
                    {
                        Header = section.Header,
                        Observations = section.Observations != null ? (from obs in section.Observations
                            select new ObservationItem
                            {
                                Term = obs.Term,
                                AssociatedTexts = obs.AssociatedText != null ? (from associatedText in obs.AssociatedText
                                    select new Regex("[\n\t]").Replace(associatedText.Text, string.Empty)).ToList().ToList() : new List<string>()
                            }).ToList() : new List<ObservationItem>()
                    }).ToList() : new List<ConsultationHeaderItem>()
            };
            
            return consultationItem;
        }
    }
}