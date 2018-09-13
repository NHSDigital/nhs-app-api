using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Extensions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions
{
    public class EmisPrescriptionMapper : IEmisPrescriptionMapper
    {
        private readonly ILogger _logger;

        public EmisPrescriptionMapper(ILogger<EmisPrescriptionMapper> logger)
        {
            _logger = logger;
        }

        public PrescriptionListResponse Map(PrescriptionRequestsGetResponse prescriptionGetResponse)
        {
            if (prescriptionGetResponse == null)
            {
                _logger.LogCritical("Null prescription object provided to mapper");
                throw new ArgumentNullException(nameof(prescriptionGetResponse));
            }

            var allPrescriptionsGrouped = new List<PrescriptionItem>();

            _logger.LogInformation($"Mapping {prescriptionGetResponse.PrescriptionRequests?.Count()} prescriptions.");
            _logger.LogInformation($"Mapping {prescriptionGetResponse.MedicationCourses?.Count()} courses.");
            
            foreach (var prescription in prescriptionGetResponse.PrescriptionRequests ?? Enumerable.Empty<PrescriptionRequest>())
            {
                foreach (var course in prescription.RequestedMedicationCourses ?? Enumerable.Empty<RequestedMedicationCourse>())
                {
                    var foundPrescriptionGroup = allPrescriptionsGrouped.FirstOrDefault(x => x.OrderDate == prescription.DateRequested && x.Status == course.RequestedMedicationCourseStatus.ToStatus());

                    var newCourseEntry = new CourseEntry
                    {
                        CourseId = course.RequestedMedicationCourseGuid,
                    };

                    if (foundPrescriptionGroup != null)
                    {
                        foundPrescriptionGroup.Courses.Add(newCourseEntry);
                    }
                    else
                    {
                        allPrescriptionsGrouped.Add(new PrescriptionItem
                        {
                            OrderDate = prescription.DateRequested,
                            Status = course.RequestedMedicationCourseStatus.ToStatus(),
                            Courses = new List<CourseEntry>
                            {
                                newCourseEntry,
                            }
                        });
                    }
                }
            }

            var result = new PrescriptionListResponse
            {
                Prescriptions = allPrescriptionsGrouped,
                Courses =
                    (prescriptionGetResponse.MedicationCourses ?? Enumerable.Empty<MedicationCourse>()).Select(MapMedicationCourseToCourse),
            };

            _logger.LogInformation($"{result.Prescriptions.Count()} prescriptions mapped");
            _logger.LogInformation($"{result.Courses.Count()} courses mapped");
            
            return result;
        }

        public CourseListResponse Map(CoursesGetResponse courseGetResponse)
        {
            if (courseGetResponse == null)
            {
                throw new ArgumentNullException(nameof(courseGetResponse));
            }

            var result = new CourseListResponse
            {
                Courses = (courseGetResponse.Courses ?? Enumerable.Empty<MedicationCourse>()).Select(MapMedicationCourseToCourse),
            };

            return result;
        }

        private static Course MapMedicationCourseToCourse(MedicationCourse course)
        {
            string details = null;
            
            if (!string.IsNullOrEmpty(course.Dosage) && !string.IsNullOrEmpty(course.QuantityRepresentation))
            {
                details = $"{course.Dosage} ‐ {course.QuantityRepresentation}";
            }
            else if (!string.IsNullOrEmpty(course.Dosage))
            {
                details = course.Dosage;
            }
            else if (!string.IsNullOrEmpty(course.QuantityRepresentation))
            {
                details = course.QuantityRepresentation;
            }

            return new Course
            {
                Id = course.MedicationCourseGuid,
                Name = course.Name,
                Details = details,
            };
        }
    }
}