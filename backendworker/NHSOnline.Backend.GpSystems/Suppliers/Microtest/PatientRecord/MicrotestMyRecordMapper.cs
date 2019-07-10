using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    public class MicrotestMyRecordMapper : IMicrotestMyRecordMapper
    {
        private readonly ILogger<MicrotestMyRecordMapper> _logger;

        public MicrotestMyRecordMapper(ILogger<MicrotestMyRecordMapper> logger)
        {
            _logger = logger;
        }

        public MyRecordResponse Map(PatientRecordGetResponse patientRecordGetResponse)
        {
            if (patientRecordGetResponse == null)
            {
                throw new System.ArgumentNullException(nameof(patientRecordGetResponse));
            }

            var myRecordResponse = new MyRecordResponse();

            MapAllergies(myRecordResponse, patientRecordGetResponse.AllergyData);
            MapMedications(myRecordResponse, patientRecordGetResponse.MedicationData);
            MapImmunisations(myRecordResponse, patientRecordGetResponse.ImmunisationData);

            SetHasSummaryRecordAccess(myRecordResponse);
            SetHasDetailedRecordAccess(myRecordResponse);

            return myRecordResponse;
        }

        private static void MapAllergies(MyRecordResponse myRecordResponse, AllergyData allergyData)
        {
            if (allergyData != null)
            {
                var allergies = allergyData.Allergies.Where(o => !string.IsNullOrEmpty(o.Severity));

                myRecordResponse.Allergies.Data = allergies
                    .Select(x => new AllergyItem
                    {
                        Name = x.Description,
                        Date = x.StartDate != null
                            ? new MyRecordDate
                            {
                                Value = DateTime.TryParse(x.StartDate, out var allergyDate)
                                    ? allergyDate
                                    : (DateTimeOffset?) null,
                                DatePart = "Unknown"
                            }
                            : null
                    })
                    .OrderByDescending(o => o.Date?.Value.GetValueOrDefault())
                    .ToList();
            }
        }

        private void MapMedications(MyRecordResponse myRecordResponse, MedicationData microtestMedicationData)
        {
            var acuteMeds = new List<MedicationItem>();
            var currentMeds = new List<MedicationItem>();
            var historicMeds = new List<MedicationItem>();

            if (microtestMedicationData != null)
            {
                var microtestMedications = microtestMedicationData.Medications;

                foreach (var medication in microtestMedications)
                {
                    if (medication.Status.Equals(MedicationStatus.Repeat, StringComparison.OrdinalIgnoreCase))
                    {
                        if (medication.Type.Equals(MedicationType.Current, StringComparison.OrdinalIgnoreCase))
                        {
                            AddMedicationItem(currentMeds, medication);
                        }
                        else if (medication.Type.Equals(MedicationType.Historic, StringComparison.OrdinalIgnoreCase))
                        {
                            AddMedicationItem(historicMeds, medication);
                        }
                        else
                        {
                            _logger.LogInformation("Found unexpected type in Microtest medication:" +
                                                   medication.Type + ". This medication will not be mapped");
                        }
                    }
                    else if (medication.Status.Equals(MedicationStatus.Acute, StringComparison.OrdinalIgnoreCase))
                    {
                        AddMedicationItem(acuteMeds, medication);
                    }
                    else
                    {
                        _logger.LogInformation("Found unexpected status in Microtest medication:" +
                                               medication.Status + ". This medication will not be mapped");
                    }
                }
            }

            myRecordResponse.Medications.Data.AcuteMedications =
                acuteMeds.OrderByDescending(med => med.Date.GetValueOrDefault());
            
            myRecordResponse.Medications.Data.CurrentRepeatMedications = 
                currentMeds.OrderByDescending(med => med.Date.GetValueOrDefault());
            
            myRecordResponse.Medications.Data.DiscontinuedRepeatMedications = 
                historicMeds.OrderByDescending(med => med.Date.GetValueOrDefault());
        }

        
        private static void MapImmunisations(MyRecordResponse myRecordResponse, ImmunisationData immunisationData)
        {
            if (immunisationData != null)
            {
                myRecordResponse.Immunisations.Data = immunisationData.Immunisations
                    .Select(x =>
                    {
                        var item = new ImmunisationItem
                        {
                            Term = x.Description,
                            EffectiveDate = x.Date != null
                                ? new MyRecordDate
                                {
                                    Value = DateTime.TryParse(x.Date, out var effectiveDate)
                                        ? effectiveDate
                                        : (DateTimeOffset?) null,
                                    DatePart = "Unknown"
                                }
                                : null,
                            Status = x.Status
                        };

                        if (x.NextDate != null)
                        {
                            item.NextDate = new MyRecordDateRawString();
                            if (DateTime.TryParse(x.NextDate, out var nextDate))
                            {
                                item.NextDate.Value = nextDate;
                                item.NextDate.DatePart = "Unknown";
                            }
                            else
                            {
                                item.NextDate.RawValue = x.NextDate;
                            }
                        }
                        return item;
                    })
                    .OrderByDescending(o => o.EffectiveDate?.Value.GetValueOrDefault())
                    .ToList();
            }
        }        
        
        /**
         * Adds a medication item if a valid date is present.
         */
        private void AddMedicationItem(List<MedicationItem> items, Medication medication)
        {
            var validDate = DateTime.TryParse(medication.PrescribedDate, out var prescribedDate);

            if (validDate)
            {
                items.Add(new MedicationItem
                {
                    Date = prescribedDate,
                    LineItems = new List<MedicationLineItem>
                    {
                        new MedicationLineItem { Text = medication.Name },
                        new MedicationLineItem { Text = medication.Dosage },
                        new MedicationLineItem { Text = medication.Quantity },
                        new MedicationLineItem { Text = "Reason: " + medication.Reason }
                    }
                });
            }
            else
            {
                _logger.LogInformation("Could not parse a valid date from Medication item. " +
                                       "This medication will not be mapped");            
            }
        }

        private static void SetHasSummaryRecordAccess(MyRecordResponse myRecordResponse)
        {
            myRecordResponse.HasSummaryRecordAccess =
                IsAny(myRecordResponse.Allergies.Data) ||
                IsAny(myRecordResponse.Medications.Data.AcuteMedications) ||
                IsAny(myRecordResponse.Medications.Data.CurrentRepeatMedications) ||
                IsAny(myRecordResponse.Medications.Data.DiscontinuedRepeatMedications);
        }

        private static void SetHasDetailedRecordAccess(MyRecordResponse myRecordResponse)
        {
            myRecordResponse.HasDetailedRecordAccess = IsAny(myRecordResponse.Immunisations.Data);
        }

        private static bool IsAny<T>(IEnumerable<T> data)
        {
            return data != null && data.Any();
        }
    }

}