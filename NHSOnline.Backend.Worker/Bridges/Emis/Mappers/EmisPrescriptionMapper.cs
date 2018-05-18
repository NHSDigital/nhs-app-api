using System;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Mappers
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
                Prescriptions = (prescriptionGetResponse.PrescriptionRequests ?? Enumerable.Empty<PrescriptionRequest>()).Select(x => new PrescriptionItem
                {
                    OrderDate = x.DateRequested,
                    Courses = (x.RequestedMedicationCourses ?? Enumerable.Empty<RequestedMedicationCourse>()).Select(c => new CourseEntry
                    {
                        CourseId = c.RequestedMedicationCourseGuid,
                        // Status is not mapped due to unknowns - future story to address this: NHSO-516
                    })
                }),
                Courses = (prescriptionGetResponse.MedicationCourses ?? Enumerable.Empty<MedicationCourse>()).Select(x => MapMedicationCourseToCourse(x)),
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
                Courses = (coursesGetResponse.Courses ?? Enumerable.Empty<MedicationCourse>()).Select(x => MapMedicationCourseToCourse(x)),
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
}
