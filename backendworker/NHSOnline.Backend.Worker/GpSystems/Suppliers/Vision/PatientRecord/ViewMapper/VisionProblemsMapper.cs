using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper
{
    public class VisionProblemsMapper : IVisionMapper<Problems>
    {
        private readonly ILogger<VisionProblemsMapper> _logger;
        private const string PastProblem = "PastProblem";
        private const string CurrentProblem = "CurrentProblem";
        
        public VisionProblemsMapper(ILogger<VisionProblemsMapper> logger) {
            _logger = logger;
        }

        public Problems Map(VisionPatientDataResponse response)
        {
            var problems = new Problems();

            var problemItems = new List<ProblemItem>();
            var rawContent = response.Record;

            if (string.IsNullOrEmpty(rawContent)) 
                return problems;
            
            try
            {
                var parsedContent = rawContent.DeserializeXml<Root>();

                foreach (var problem in parsedContent.Patient.Problems)
                {
                    string problemStatus;
                    switch (problem.SubGroupCode)
                    {
                        case CurrentProblem:
                            problemStatus = "Current";
                            break;
                        case PastProblem:
                            problemStatus = "Past";
                            break;
                        default:
                            problemStatus = problem.SubGroupCode;
                            break;

                    }
                    problemItems.Add(new ProblemItem
                    {
                        EffectiveDate = new MyRecordDate
                        {
                            Value = DateTime.TryParse(problem.EventDate, out var eventDate) ?
                                eventDate : (DateTimeOffset?)null,
                            DatePart = "Unknown"
                        },
                        LineItems = new List<ProblemLineItem>
                        {
                            new ProblemLineItem { Text = problem.ReadTerm },
                            new ProblemLineItem { Text = "Status: " + problemStatus }
                        }
                    });
                }
                problems.Data = problemItems;
            }
            catch(InvalidOperationException e) {
                _logger.LogWarning("Error deserializing raw Problems response content string. " + e.Message);
                problems.HasErrored = true;
            }
            return problems;
        }
    }
}