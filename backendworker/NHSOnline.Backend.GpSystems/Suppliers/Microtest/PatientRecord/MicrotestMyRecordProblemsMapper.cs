using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    internal sealed class MicrotestMyRecordProblemsMapper
    {
        private const string ProblemDateDisplayFormat = "d MMMM yyyy";

        private readonly ILogger _logger;

        public MicrotestMyRecordProblemsMapper(
            ILogger<MicrotestMyRecordProblemsMapper> logger)
        {
            _logger = logger;
        }

        internal void Map(MyRecordResponse myRecordResponse, ProblemData problemData)
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
                        problemLineItem.Text = $"Finish Date: {problem.FinishDate}";
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
                _logger.LogInformation(
                    "No valid rubric found in MyRecord Problem from Microtest. This item will not be mapped");
            }
        }

        private static bool IsRubricValid(string rubric)
        {
            var isValid = false;

            if (rubric != null)
            {
                var trimmedRubric = rubric.Trim();
                if (trimmedRubric.Length > 0 && !trimmedRubric.Equals(ProblemRubric.NoRubric, StringComparison.OrdinalIgnoreCase))
                {
                    isValid = true;
                }
            }

            return isValid;
        }
    }
}