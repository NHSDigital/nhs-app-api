using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    internal sealed class EmisProblemMapper
    {
        private const string DateFormat = "d MMMM yyyy";

        public Problems Map(MedicationRootObject problemsGetResponse)
        {
            if (problemsGetResponse == null)
            {
                throw new ArgumentNullException(nameof(problemsGetResponse));
            }

            var problems = new Problems();

            var medicalRecord = problemsGetResponse.MedicalRecord;

            if (medicalRecord?.Problems != null)
            {
                problems.Data = medicalRecord.Problems.Select(Map).ToList();
            }

            return problems;
        }

        private static ProblemItem Map(Problem problem)
        {
            var problemItem = new ProblemItem
            {
                EffectiveDate = problem.Observation.EffectiveDate != null
                    ? new MyRecordDate
                    {
                        Value = problem.Observation.EffectiveDate.Value,
                        DatePart = problem.Observation.EffectiveDate.DatePart
                    }
                    : new MyRecordDate(),
                LineItems = new List<ProblemLineItem>
                {
                    new ProblemLineItem { Text = problem.Observation.Term },
                    new ProblemLineItem { Text = "Significance: " + problem.Significance },
                    new ProblemLineItem { Text = "Status: " + problem.Status }
                }
            };

            if (problem.Observation.AssociatedText != null)
            {
                problemItem.LineItems.Add(new ProblemLineItem
                {
                    Text = "Notes:",
                    LineItems = problem.Observation.AssociatedText.Select(x => x.Text).ToList(),
                });
            }

            if (problem.ProblemEndDate != null)
            {
                problemItem.LineItems.Add(new ProblemLineItem
                {
                    Text = "Ended: " + problem.ProblemEndDate.Value.ToString(DateFormat, CultureInfo.InvariantCulture)
                });
            }

            return problemItem;
        }
    }
}