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

            SetHasSummaryRecordAccess(myRecordResponse);

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
                                Value = DateTime.TryParse(x.StartDate, out var eventDate)
                                    ? eventDate
                                    : (DateTimeOffset?) null,
                                DatePart = x.StartDate
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

            myRecordResponse.Medications.Data.AcuteMedications = acuteMeds;
            myRecordResponse.Medications.Data.CurrentRepeatMedications = currentMeds;
            myRecordResponse.Medications.Data.DiscontinuedRepeatMedications = historicMeds;
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

        private static bool IsAny<T>(IEnumerable<T> data)
        {
            return data != null && data.Any();
        }
    }

}