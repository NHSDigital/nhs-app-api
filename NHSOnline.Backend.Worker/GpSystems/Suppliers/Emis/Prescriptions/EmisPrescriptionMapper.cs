using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
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

            var result = new PrescriptionListResponse
            {
                Prescriptions =
                    (prescriptionGetResponse.PrescriptionRequests ?? Enumerable.Empty<PrescriptionRequest>()).Select(
                        x => new PrescriptionItem
                        {
                            OrderDate = x.DateRequested,
                            Courses = (x.RequestedMedicationCourses ?? Enumerable.Empty<RequestedMedicationCourse>())
                                .Select(c => new CourseEntry
                                {
                                    CourseId = c.RequestedMedicationCourseGuid,
                                    Status = c.RequestedMedicationCourseStatus.ToString()
                                })
                        }),
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
            return new Course
            {
                Dosage = course.Dosage,
                Id = course.MedicationCourseGuid,
                Name = course.Name,
                Quantity = course.QuantityRepresentation,
            };
        }
    }   
       
    // TODO To be used in NHSO-1460
    public static class Extentions
    {
        public static Status ToFrontEndStatus(this RequestedMedicationCourseStatus value)
        {
            switch (value)
            {
                case RequestedMedicationCourseStatus.Issued:
                    return Status.Approved;
                case RequestedMedicationCourseStatus.Requested:
                    return Status.Ordered;
                case RequestedMedicationCourseStatus.ForwardedForSigning:
                    return Status.Ordered;
                case RequestedMedicationCourseStatus.Rejected:
                    return Status.Rejected;   
                default:
                    throw new InvalidEnumArgumentException(nameof(value));
            }
        }
    }
}