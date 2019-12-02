using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisConsultationMapper
    {
        private readonly ILogger<EmisConsultationMapper> _logger;

        private const string Observation = "Observation";
        private const string Unknown = "UnKnown";

        public EmisConsultationMapper(ILogger<EmisConsultationMapper> logger)
        {
            _logger = logger;
        }

        public Consultations Map(MedicationRootObject consultationsGetResponse)
        {
            if (consultationsGetResponse == null)
            {
                throw new ArgumentNullException(nameof(consultationsGetResponse));
            }

            var consultations = new Consultations();

            if (consultationsGetResponse.MedicalRecord == null)
            {
                return consultations;
            }

            var medicalRecord = consultationsGetResponse.MedicalRecord;

            consultations.Data = (medicalRecord.Consultations ?? Enumerable.Empty<Consultation>())
                .Select(FilterConsultationItem);

            return consultations;
        }

        private ConsultationItem FilterConsultationItem(Consultation response)
        {
            if (response == null)
            {
                _logger.LogWarning("Removing a consultation due to null response");
                return null;
            }

            var consultationHeaders = FilterConsultationHeaders(response);

            MyRecordDate effectiveDate = response.EffectiveDate?.Value != null
                ? new MyRecordDate
                {
                    Value = response.EffectiveDate.Value,
                    DatePart = response.EffectiveDate.DatePart
                }
                : new MyRecordDate();

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

                    if (string.Equals(section.Header, "Document", StringComparison.Ordinal))
                    {
                        var documentComments = retrieveAllComments(response);

                        consultationHeaders.Add(new ConsultationHeaderItem
                        {
                            Header = section.Header,
                            ObservationsWithTerm = observationItemWithTerms,
                            AssociatedTexts = associatedTexts,
                            Comments = documentComments
                        });
                    }
                    else
                    {
                        consultationHeaders.Add(new ConsultationHeaderItem
                        {
                            Header = section.Header,
                            ObservationsWithTerm = observationItemWithTerms,
                            AssociatedTexts = associatedTexts
                        });
                    }
                }
            }

            return consultationHeaders;
        }

        private static List<string> retrieveAllComments(Consultation response)
        {
            var associatedTexts = new List<string>();
            if (response?.Sections != null)
            {
                foreach (var section in response.Sections)
                {
                    if (string.Equals(section.Header, "Comment", StringComparison.Ordinal))
                    {
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
                    }
                }
            }
            return associatedTexts;
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
                        AssociatedTexts = associatedTextsWithTerms,
                        EventGuid = obs.EventGuid,
                        CodeId = obs.CodeId,
                        ObservationType = obs.ObservationType
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