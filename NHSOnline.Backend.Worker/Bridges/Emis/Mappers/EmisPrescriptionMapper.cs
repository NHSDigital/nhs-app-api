using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions;
using Course = NHSOnline.Backend.Worker.Areas.Prescriptions.Models.Course;

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
                    Courses = (x.RequestedMedicationCourses ?? Enumerable.Empty<RequestedMedicationCourse>()).Select(c => new CourseEntry
                    {
                        CourseId = c.RequestedMedicationCourseGuid,
                        // Status is not mapped due to unknowns - future story to address this: NHSO-516
                    })
                }),
                Courses = (prescriptionGetResponse.MedicationCourses ?? Enumerable.Empty<MedicationCourse>()).Select(x => new Course
                {
                    Dosage = x.Dosage,
                    Id = x.MedicationCourseGuid,
                    Name = x.Name,
                    Quantity = x.QuantityRepresentation,
                })
            };

            return result;
        }
    }
}
