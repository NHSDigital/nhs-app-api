using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Prescriptions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions
{
    public class TppCourseMapper : ITppCourseMapper
    {
        private readonly ILogger _logger;

        public TppCourseMapper(ILogger<TppCourseMapper> logger)
        {
            _logger = logger;
        }

        public CourseListResponse Map(List<Medication> medications)
        {
            if (medications == null)
            {
                _logger.LogError("No Courses provided to mapper");
                throw new ArgumentNullException(nameof(medications));
            }

            CourseListResponse result;
            
            if (medications.Any())
            {
                result = new CourseListResponse
                {
                    Courses = medications.Select(x =>
                        new Course { Id = x.DrugId, Name = x.Drug, Details = x.Details }),
                };
            }
            else
            {
                result = new CourseListResponse
                {
                    Courses = new List<Course>()
                };
            }
            
            _logger.LogDebug($"Mapped {medications.Count} TPP courses to {result.Courses.Count()} NHS Online courses.");

            return result;
        }
    }
}