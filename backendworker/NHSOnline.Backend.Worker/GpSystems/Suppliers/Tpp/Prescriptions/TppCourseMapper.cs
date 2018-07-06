using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions
{
    public class TppCourseMapper : ITppCourseMapper
    {
        public CourseListResponse Map(List<Medication> medications)
        {
            if (medications == null)
            {
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

            return result;
        }
    }
}