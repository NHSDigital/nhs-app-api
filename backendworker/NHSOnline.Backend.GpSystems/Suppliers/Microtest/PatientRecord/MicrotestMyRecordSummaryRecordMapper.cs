using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    internal sealed class MicrotestMyRecordSummaryRecordMapper
    {
        private readonly ILogger _logger;

        public MicrotestMyRecordSummaryRecordMapper(ILogger<MicrotestMyRecordSummaryRecordMapper> logger)
        {
            _logger = logger;
        }

        internal void Map(MyRecordResponse myRecordResponse, PatientRecordGetResponse patientRecordGetResponse)
        {
            MapAllergies(myRecordResponse, patientRecordGetResponse.AllergyData);
            MapMedications(myRecordResponse, patientRecordGetResponse.MedicationData);
        }

        private void MapAllergies(MyRecordResponse myRecordResponse, AllergyData allergyData)
        {
            if (allergyData != null)
            {
                var allergies = allergyData.Allergies.Where(HasSeverity).ToList();

                var removedCount = allergyData.Count - allergies.Count;
                if (removedCount != 0)
                {
                    _logger.LogInformation(
                        $"{removedCount} items filtered out of Allergies due to value stored in Severity field.");
                }

                myRecordResponse.Allergies.Data = allergies
                    .Select(x => new AllergyItem
                    {
                        Name = x.Description,
                        Date = x.StartDate != null
                            ? new MyRecordDate
                            {
                                Value = DateTime.TryParse(x.StartDate, out var allergyDate)
                                    ? allergyDate
                                    : (DateTimeOffset?)null,
                                DatePart = "Unknown"
                            }
                            : null
                    })
                    .OrderByDescending(o => o.Date?.Value.GetValueOrDefault())
                    .ToList();

                myRecordResponse.Allergies.HasUndeterminedAccess = !allergyData.Allergies.Any();
            }

            bool HasSeverity(Allergy allergy) => !string.IsNullOrEmpty(allergy.Severity);
        }

        private void MapMedications(MyRecordResponse myRecordResponse, MedicationData microtestMedicationData)
        {
            var acuteMedications = new List<MedicationItem>();
            var currentMedications = new List<MedicationItem>();
            var historicalMedications = new List<MedicationItem>();

            if (microtestMedicationData != null)
            {
                var microtestMedications = microtestMedicationData.Medications;

                foreach (var medication in microtestMedications)
                {
                    if (medication.Status.Equals(MedicationStatus.Repeat, StringComparison.OrdinalIgnoreCase))
                    {
                        if (medication.Type.Equals(MedicationType.Current, StringComparison.OrdinalIgnoreCase))
                        {
                            AddMedicationItemIfValidDatePresent(currentMedications, medication);
                        }
                        else if (medication.Type.Equals(MedicationType.Historic, StringComparison.OrdinalIgnoreCase))
                        {
                            AddMedicationItemIfValidDatePresent(historicalMedications, medication);
                        }
                        else
                        {
                            _logger.LogInformation("Found unexpected type in Microtest medication:" +
                                                   medication.Type + ". This medication will not be mapped");
                        }
                    }
                    else if (medication.Status.Equals(MedicationStatus.Acute, StringComparison.OrdinalIgnoreCase))
                    {
                        AddMedicationItemIfValidDatePresent(acuteMedications, medication);
                    }
                    else
                    {
                        _logger.LogInformation("Found unexpected status in Microtest medication:" +
                                               medication.Status + ". This medication will not be mapped");
                    }
                }

                myRecordResponse.Medications.HasUndeterminedAccess = !microtestMedicationData.Medications.Any();
            }

            myRecordResponse.Medications.Data.AcuteMedications =
                acuteMedications.OrderByDescending(med => med.Date.GetValueOrDefault());

            myRecordResponse.Medications.Data.CurrentRepeatMedications =
                currentMedications.OrderByDescending(med => med.Date.GetValueOrDefault());

            myRecordResponse.Medications.Data.DiscontinuedRepeatMedications =
                historicalMedications.OrderByDescending(med => med.Date.GetValueOrDefault());
        }

        private void AddMedicationItemIfValidDatePresent(List<MedicationItem> medicationItems, Medication medication)
        {
            var validDate = DateTime.TryParse(medication.PrescribedDate, out var prescribedDate);

            if (validDate)
            {
                medicationItems.Add(new MedicationItem
                {
                    Date = prescribedDate,
                    LineItems = new List<MedicationLineItem>
                    {
                        new MedicationLineItem { Text = medication.Name },
                        new MedicationLineItem { Text = medication.Dosage },
                        new MedicationLineItem { Text = medication.Quantity },
                        new MedicationLineItem { Text = $"Reason: {medication.Reason}" }
                    }
                });
            }
            else
            {
                _logger.LogInformation("Could not parse a valid date from Medication item. " +
                                       "This medication will not be mapped");
            }
        }
    }
}