using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Extensions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions
{
    public class EmisPrescriptionMapper : IEmisPrescriptionMapper
    {
        public PrescriptionListResponse Map(PrescriptionRequestsGetResponse prescriptionGetResponse)
        {
            if (prescriptionGetResponse == null)
            {
                throw new ArgumentNullException(nameof(prescriptionGetResponse));
            }

            var allPrescriptionsGrouped = new List<PrescriptionItem>();

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
                    (prescriptionGetResponse.MedicationCourses ?? Enumerable.Empty<MedicationCourse>()).Select(x =>
                        MapMedicationCourseToCourse(x)),
            };

            return result;
        }

        public CourseListResponse Map(CoursesGetResponse coursesGetResponse)
        {
            if (coursesGetResponse == null)
            {
                throw new ArgumentNullException(nameof(coursesGetResponse));
            }

            var result = new CourseListResponse
            {
                Courses = (coursesGetResponse.Courses ?? Enumerable.Empty<MedicationCourse>()).Select(x =>
                    MapMedicationCourseToCourse(x)),
            };

            return result;
        }

        private Course MapMedicationCourseToCourse(MedicationCourse course)
        {
            string details = null;
            
            if (!string.IsNullOrEmpty(course.Dosage) && !string.IsNullOrEmpty(course.QuantityRepresentation))
            {
                details = $"{course.Dosage} - {course.QuantityRepresentation}";
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