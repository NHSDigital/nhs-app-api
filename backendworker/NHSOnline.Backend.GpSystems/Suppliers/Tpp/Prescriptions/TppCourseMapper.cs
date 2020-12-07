using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions
{
    public class TppCourseMapper : ITppCourseMapper
    {
        private readonly ILogger _logger;
        private readonly IGpSystem _gpSystem;

        public TppCourseMapper(ILogger<TppCourseMapper> logger, IGpSystemFactory gpSystemFactory)
        {
            _logger = logger;
            _gpSystem = gpSystemFactory.CreateGpSystem(Supplier.Tpp);
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
                    Courses = new List<Course>(),
                };
            }

            result.SpecialRequestCharacterLimit = _gpSystem.PrescriptionSpecialRequestCharacterLimit;

            _logger.LogDebug($"Mapped {medications.Count} TPP courses to {result.Courses.Count()} NHS Online courses.");

            return result;
        }
    }
}