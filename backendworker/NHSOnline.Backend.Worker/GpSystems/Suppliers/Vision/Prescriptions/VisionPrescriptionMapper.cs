using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Extensions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Courses;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Prescriptions
{
    public class VisionPrescriptionMapper : IVisionPrescriptionMapper
    {
        private readonly ILogger _logger;

        public VisionPrescriptionMapper(ILogger<VisionPrescriptionMapper> logger)
        {
            _logger = logger;
        }

        public PrescriptionListResponse Map(PrescriptionHistory prescriptionGetResponse)
        {
            if (prescriptionGetResponse == null)
            {
                _logger.LogCritical($"Null {nameof(prescriptionGetResponse)} provided to mapper");
                throw new ArgumentNullException(nameof(prescriptionGetResponse));
            }
            
            var allPrescriptionsGrouped = new List<PrescriptionItem>();
            var allCourses = new List<Course>();

            if (prescriptionGetResponse.Requests?.Count > 0)
            {
                foreach (var prescription in prescriptionGetResponse.Requests)
                {
                    var foundPrescriptionGroup = allPrescriptionsGrouped.FirstOrDefault(x => x.OrderDate == prescription.Date.Date && x.Status == prescription.Status.ToStatus());

                    if (foundPrescriptionGroup == null)
                    {
                        foundPrescriptionGroup = new PrescriptionItem
                        {
                            OrderDate = prescription.Date.Date,
                            Status = prescription.Status.ToStatus(),
                            Courses = new List<CourseEntry>(),
                        };

                        allPrescriptionsGrouped.Add(foundPrescriptionGroup);
                    }
                    
                    foreach (var course in prescription.Repeats)
                    {
                        // The prescriptions call for Vision does not return an id for each course,
                        // but our response to the front end has prescriptions and courses in different
                        // collections. Simply create an id to link the two together (only used for display).
                        var customIdForCourse = $"nhsapp-{ Guid.NewGuid() }";

                        allCourses.Add(MapPrescriptionRepeatToCourse(customIdForCourse, course));

                        var newCourseEntry = new CourseEntry
                        {
                            CourseId = customIdForCourse,
                        };

                        foundPrescriptionGroup.Courses.Add(newCourseEntry);
                    }
                }
            }
            
            var result = new PrescriptionListResponse
            {
                Prescriptions = allPrescriptionsGrouped,
                Courses = allCourses,
            };

            _logger.LogInformation($"{result.Prescriptions.Count()} prescriptions mapped");
            _logger.LogInformation($"{result.Courses.Count()} courses mapped");
            
            return result;
        }

        public CourseListResponse Map(EligibleRepeats eligibleRepeatsResponse)
        {
            if (eligibleRepeatsResponse == null)
            {
                throw new ArgumentNullException(nameof(eligibleRepeatsResponse));
            }

            var result = new CourseListResponse
            {
                Courses = eligibleRepeatsResponse.Repeats.Select(MapRepeatToCourse),
                SpecialRequestNecessity = eligibleRepeatsResponse.Settings?.AllowFreeText == true ? Areas.SharedModels.Necessity.Optional : Areas.SharedModels.Necessity.NotAllowed,
            };

            return result;
        }

        
        private static Course MapPrescriptionRepeatToCourse(string id, GetPrescriptionRepeat course)
        {
            var details = GetDetails(course);

            return new Course
            {
                Id = id,
                Name = course.Drug,
                Details = details,
            };
        }
        
        private static Course MapRepeatToCourse(Repeat course)
        {
            var details = GetDetails(course);

            return new Course
            {
                Id = course.Id,
                Name = course.Drug,
                Details = details,
            };
        }

        private static string GetDetails(IRepeat course)
        {
            string details = null;
            
            if (!string.IsNullOrEmpty(course.Dosage) && !string.IsNullOrEmpty(course.Quantity))
            {
                details = $"{course.Dosage} ‐ {course.Quantity}";
            }
            else if (!string.IsNullOrEmpty(course.Dosage))
            {
                details = course.Dosage;
            }
            else if (!string.IsNullOrEmpty(course.Quantity))
            {
                details = course.Quantity;
            }

            return details;
        }
    }
}