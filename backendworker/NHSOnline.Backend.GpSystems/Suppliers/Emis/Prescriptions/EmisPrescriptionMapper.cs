using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions
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
                    var foundPrescriptionGroup = allPrescriptionsGrouped.FirstOrDefault(x => x.OrderDate == prescription.DateRequested && x.Status == MapStatus(course.RequestedMedicationCourseStatus));

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
                        // RequestedByDisplayName only contains forename and last name.
                        // RequestedByForenames contains forename and middle name(s).
                        string requestedBy = null;

                        if (string.IsNullOrWhiteSpace(prescription.RequestedByForenames)
                            || string.IsNullOrWhiteSpace(prescription.RequestedBySurname))
                        {
                            requestedBy = prescription.RequestedByDisplayName;
                        }
                        else
                        {
                            var forenamesAndMiddleNames =
                                prescription.RequestedByForenames.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                            var forename = forenamesAndMiddleNames[0];
                            var surname = prescription.RequestedBySurname;

                            if (!prescription.RequestedByDisplayName.StartsWith(forename, StringComparison.Ordinal) ||
                                !prescription.RequestedByDisplayName.EndsWith(surname, StringComparison.Ordinal))
                            {
                                requestedBy = prescription.RequestedByDisplayName;
                            }
                        }

                        allPrescriptionsGrouped.Add(new PrescriptionItem
                        {
                            OrderedBy = requestedBy,
                            OrderDate = prescription.DateRequested,
                            Status = MapStatus(course.RequestedMedicationCourseStatus),
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
                Details = details
            };
        }

        private static Status MapStatus(RequestedMedicationCourseStatus value)
        {
            switch (value)
            {
                case RequestedMedicationCourseStatus.Issued:
                    return Status.Approved;
                case RequestedMedicationCourseStatus.Requested:
                case RequestedMedicationCourseStatus.ForwardedForSigning:
                    return Status.Requested;
                case RequestedMedicationCourseStatus.Rejected:
                    return Status.Rejected;
                default:
                    return Status.Unknown;
            }
        }
    }
}