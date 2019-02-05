using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisConsultationMapper
    {
        private const string Observation = "Observation";
        private const string Unknown = "UnKnown";

        public Consultations Map(MedicationRootObject consultationsGetResponse)
        {
            if(consultationsGetResponse == null)
            {
                throw new ArgumentNullException(nameof(consultationsGetResponse));
            }

            var consultations = new Consultations();

            if (consultationsGetResponse.MedicalRecord == null)
            {
                return consultations;
            }

            var medicalRecord = consultationsGetResponse.MedicalRecord;

            consultations.Data =
                (medicalRecord.Consultations ?? Enumerable.Empty<Consultation>()).Select(FilterConsultationItem);

            return consultations;
        }

        private ConsultationItem FilterConsultationItem(Consultation response)
        {
            var consultationHeaders = FilterConsultationHeaders(response);
            
            var effectiveDate = new MyRecordDate
            {
                Value = response.EffectiveDate.Value, DatePart = response.EffectiveDate.DatePart
            };

            var consultantLocation = !string.IsNullOrEmpty(response.Location)
                ? $"{response.Location} - {response.ConsultantName}"
                : response.ConsultantName;

            var consultationItem = new ConsultationItem
            {
                EffectiveDate = effectiveDate,
                ConsultantLocation = consultantLocation,
                ConsultationHeaders = consultationHeaders
            };

            return consultationItem;
        }

        private static List<ConsultationHeaderItem> FilterConsultationHeaders(Consultation response)
        {
            var consultationHeaders = new List<ConsultationHeaderItem>();

            if (response?.Sections != null)
            {
                foreach (var section in response.Sections)
                {
                    if (section.Observations == null || !section.Observations.Any(obs =>
                            FilterObservationTypes(obs) && (!string.IsNullOrEmpty(obs.Term) || obs.AssociatedText != null &&
                                                            obs.AssociatedText.Any(at => !string.IsNullOrEmpty(at.Text)))))
                    {
                        continue;
                    }

                    var observationItemWithTerms = FilterObservationsWithTerms(section);
                    var associatedTexts = FilterAssociatedTexts(section);

                    consultationHeaders.Add(new ConsultationHeaderItem
                    {
                        Header = section.Header,
                        ObservationsWithTerm = observationItemWithTerms,
                        AssociatedTexts = associatedTexts
                    });
                }
            }

            return consultationHeaders;
        }

        private static List<string> FilterAssociatedTexts(Section section)
        {
            var associatedTexts = new List<string>();

            if (section?.Observations != null)
            {
                foreach (var obs in section.Observations)
                {
                    if (obs == null || !string.IsNullOrEmpty(obs.Term) || !FilterObservationTypes(obs) ||
                        obs.AssociatedText == null)
                    {
                        continue;
                    }

                    if (obs.AssociatedText != null)
                    {
                        foreach (var associatedText in obs.AssociatedText)
                        {
                            if (associatedText != null && !string.IsNullOrEmpty(associatedText.Text))
                            {
                                associatedTexts.Add(GetAssociatedText(associatedText));
                            }
                        }
                    }
                }                
            }

            return associatedTexts;
        }

        private static List<ObservationItemWithTerm> FilterObservationsWithTerms(Section section)
        {
            var observationItemWithTerms = new List<ObservationItemWithTerm>();

            if (section?.Observations != null)
            {
                foreach (var obs in section.Observations)
                {
                    if (obs == null || string.IsNullOrEmpty(obs.Term) || !FilterObservationTypes(obs))
                    {
                        continue;
                    }

                    var associatedTextsWithTerms = FilterAssociatedTextsWithTerms(obs);

                    observationItemWithTerms.Add(new ObservationItemWithTerm
                    {
                        Term = obs.Term,
                        AssociatedTexts = associatedTextsWithTerms
                    });
                }
            }

            return observationItemWithTerms;
        }

        private static List<string> FilterAssociatedTextsWithTerms(Observation obs)
        {
            var associatedTextsWithTerms = new List<string>();

            if (obs?.AssociatedText != null)
            {
                foreach (var obsText in obs.AssociatedText)
                {
                    if (!string.IsNullOrEmpty(obsText.Text))
                    {
                        associatedTextsWithTerms.Add(GetAssociatedText(obsText));
                    }
                }
            }

            return associatedTextsWithTerms;
        }

        private static bool FilterObservationTypes(Observation obs)
        {
            return !string.IsNullOrEmpty(obs.ObservationType) &&
                   (obs.ObservationType.Equals(Observation, StringComparison.OrdinalIgnoreCase) ||
                    obs.ObservationType.Equals(Unknown, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetAssociatedText(AssociatedText associatedText)
        {
            return associatedText?.Text.Replace("\t", string.Empty, StringComparison.Ordinal)
                .Trim(new[] { '\n' })
                .Replace("\n", "; ", StringComparison.OrdinalIgnoreCase);
        }
    }
}