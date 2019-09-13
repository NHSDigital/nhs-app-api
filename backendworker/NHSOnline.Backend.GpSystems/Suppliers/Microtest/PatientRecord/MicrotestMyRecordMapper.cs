using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;
using Medication = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord.Medication;
using Problem = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord.Problem;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    public class MicrotestMyRecordMapper : IMicrotestMyRecordMapper
    {
        private readonly ILogger<MicrotestMyRecordMapper> _logger;
        
        private const string ProblemDateDisplayFormat = "d MMMM yyyy";

        private const string NoRubric = "No rubric recorded";

        public MicrotestMyRecordMapper(ILogger<MicrotestMyRecordMapper> logger)
        {
            _logger = logger;
        }

        public MyRecordResponse Map(PatientRecordGetResponse patientRecordGetResponse)
        {
            if (patientRecordGetResponse == null)
            {
                throw new ArgumentNullException(nameof(patientRecordGetResponse));
            }

            var myRecordResponse = new MyRecordResponse();

            MapAllergies(myRecordResponse, patientRecordGetResponse.AllergyData);
            MapMedications(myRecordResponse, patientRecordGetResponse.MedicationData);
            MapImmunisations(myRecordResponse, patientRecordGetResponse.ImmunisationData);
            MapProblems(myRecordResponse, patientRecordGetResponse.ProblemData);
            MapMedicalHistory(myRecordResponse, patientRecordGetResponse.MedicalHistoryData);

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

                myRecordResponse.Allergies.HasUndeterminedAccess = !allergyData.Allergies.Any();
            }
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
                            AddMedicationItemIfValid(currentMedications, medication);
                        }
                        else if (medication.Type.Equals(MedicationType.Historic, StringComparison.OrdinalIgnoreCase))
                        {
                            AddMedicationItemIfValid(historicalMedications, medication);
                        }
                        else
                        {
                            _logger.LogInformation("Found unexpected type in Microtest medication:" +
                                                   medication.Type + ". This medication will not be mapped");
                        }
                    }
                    else if (medication.Status.Equals(MedicationStatus.Acute, StringComparison.OrdinalIgnoreCase))
                    {
                        AddMedicationItemIfValid(acuteMedications, medication);
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

                myRecordResponse.Immunisations.HasUndeterminedAccess = !immunisationData.Immunisations.Any();
            }
        }

        private void MapProblems(MyRecordResponse myRecordResponse, ProblemData problemData)
        {
            if (problemData != null)
            {
                var problemItems = new List<ProblemItem>();

                foreach (var problem in problemData.Problems)
                {
                    AddProblemItemIfValid(problemItems, problem);  
                }
                
                myRecordResponse.Problems.Data 
                    = problemItems.OrderByDescending(p => p.EffectiveDate?.Value.GetValueOrDefault());
                
                myRecordResponse.Problems.HasUndeterminedAccess = !problemData.Problems.Any();
            }
        }
        
        private void MapMedicalHistory(MyRecordResponse myRecordResponse, MedicalHistoryData medicalHistoryData)
        {
            if (medicalHistoryData != null)
            {
                var medHistoryItems = new List<MedicalHistoryItem>();

                foreach (var medicalHistory in medicalHistoryData.MedicalHistories)
                {
                    AddMedicalHistoryItemIfValid(medHistoryItems, medicalHistory);  
                }
                
                myRecordResponse.MedicalHistories.Data 
                    = medHistoryItems.OrderByDescending(p => p.StartDate?.Value.GetValueOrDefault());
                
                myRecordResponse.MedicalHistories.HasUndeterminedAccess = !medicalHistoryData.MedicalHistories.Any();
            }
        }
        
        private void AddMedicalHistoryItemIfValid(List<MedicalHistoryItem> medicalHistoryItems, MedicalHistory medicalHistory)
        {            
            if(!string.IsNullOrWhiteSpace(medicalHistory.Rubric))
            {
                MyRecordDate medHistoryStartDate = null;
                var validStartDate = DateTime.TryParse(medicalHistory.StartDate, out var parsedStartDate);

                if (validStartDate)
                {
                    medHistoryStartDate = new MyRecordDate
                    {
                        Value = parsedStartDate,
                        DatePart = "Unknown"
                    };
                }

                medicalHistoryItems.Add(new MedicalHistoryItem
                {
                    StartDate = medHistoryStartDate,
                    Rubric = medicalHistory.Rubric,
                    Description = medicalHistory.Description
                });
            }
            else
            {
                _logger.LogInformation("This item will not be mapped as no valid rubric found in MyRecord Medical History from Microtest.");            
            }
        }
        
        /**
         * Adds a medication item if a valid date is present.
         */
        private void AddMedicationItemIfValid(List<MedicationItem> medicationItems, Medication medication)
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

        private void AddProblemItemIfValid(List<ProblemItem> problemItems, Problem problem)
        {
            var validRubric = IsRubricValid(problem.Rubric);
            
            if (validRubric)
            {
                MyRecordDate problemStartDate = null;
                var validStartDate = DateTime.TryParse(problem.StartDate, out var parsedStartDate);

                if (validStartDate)
                {
                    problemStartDate = new MyRecordDate
                    {
                        Value = parsedStartDate,
                        DatePart = "Unknown"
                    };
                }

                var lineItems = new List<ProblemLineItem>();

                if (problem.FinishDate != null)
                {
                    var problemLineItem = new ProblemLineItem();
                    
                    var validFinishDate = DateTime.TryParse(problem.FinishDate, out var parsedFinishDate);
                    if (validFinishDate)
                    {
                        problemLineItem.Text = "Finish Date: " +
                                parsedFinishDate.ToString(ProblemDateDisplayFormat, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        problemLineItem.Text = "Finish Date: " + problem.FinishDate;
                    }
                    lineItems.Add(problemLineItem);
                }

                //This will always be valid at this point, so no need for any checks
                lineItems.Add(new ProblemLineItem { Text = problem.Rubric });

                problemItems.Add(new ProblemItem
                {
                    EffectiveDate = problemStartDate,
                    LineItems = lineItems
                });
            }
            else
            {
                _logger.LogInformation("No valid rubric found in MyRecord Problem from Microtest. " +
                                       "This item will not be mapped");            
            }
        }       

        private static bool IsRubricValid(string rubric)
        {
            var isValid = false;

            if (rubric != null)
            {
                var trimmedRubric = rubric.Trim();
                if (trimmedRubric.Length > 0 && !trimmedRubric.Equals(NoRubric, StringComparison.OrdinalIgnoreCase))
                {
                    isValid = true;
                }
            }

            return isValid;
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
            myRecordResponse.HasDetailedRecordAccess = 
                IsAny(myRecordResponse.Immunisations.Data) ||
                IsAny(myRecordResponse.Problems.Data) ||
                IsAny(myRecordResponse.MedicalHistories.Data);
        }

        private static bool IsAny<T>(IEnumerable<T> data)
        {
            return data != null && data.Any();
        }
    }
}