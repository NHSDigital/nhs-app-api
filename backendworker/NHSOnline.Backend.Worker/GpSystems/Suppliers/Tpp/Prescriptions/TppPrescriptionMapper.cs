using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions
{
    public class TppPrescriptionMapper : ITppPrescriptionMapper
    {
        private readonly ILogger _logger;

        public TppPrescriptionMapper(ILogger<TppPrescriptionMapper> logger)
        {
            _logger = logger;
        }

        public PrescriptionListResponse Map(List<Medication> medications)
        {
            if (medications == null)
            {
                _logger.LogError("No Prescriptions provided to mapper");
                throw new ArgumentNullException(nameof(medications));
            }

            _logger.LogInformation($"Mapping {medications?.Count()} prescriptions.");

            PrescriptionListResponse result;
            
            if (medications.Any())
            {
                result = new PrescriptionListResponse
                {
                    Prescriptions = new List<PrescriptionItem>
                    {
                        new PrescriptionItem
                        {
                            OrderDate = null,
                            Status = null,
                            Courses = medications
                                .Select(c => new CourseEntry { CourseId = c.DrugId }).ToList()
                        }
                    },
                    Courses = medications.Select(x =>
                        new Course { Id = x.DrugId, Name = x.Drug, Details = x.Details }),
                };
            }
            else
            {
                result = new PrescriptionListResponse
                {
                    Prescriptions = new List<PrescriptionItem>(),
                    Courses = new List<Course>()
                };
            }
            
            _logger.LogDebug($"Mapped {medications.Count} TPP prescriptions to {result.Courses.Count()} NHS Online prescriptions.");

            return result;
        }
    }
}
