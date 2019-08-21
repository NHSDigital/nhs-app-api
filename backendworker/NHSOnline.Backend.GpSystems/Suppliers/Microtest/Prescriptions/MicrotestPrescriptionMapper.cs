using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions;
using Course = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions.Course;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Prescriptions
{
    public class MicrotestPrescriptionMapper : IMicrotestPrescriptionMapper
    {
        private readonly ILogger _logger;

        private static readonly IDictionary<string, Status> StatusMap =
            new Dictionary<string, Status>(StringComparer.OrdinalIgnoreCase)
            {
                { PrescriptionStatus.Requested, Status.Requested },
                { PrescriptionStatus.Rejected, Status.Rejected },
                { PrescriptionStatus.Confirmed, Status.Approved },
                { PrescriptionStatus.Cancelled, Status.Unknown },
            };

        public MicrotestPrescriptionMapper(ILogger<MicrotestPrescriptionMapper> logger)
        {
            _logger = logger;
        }

        public PrescriptionListResponse Map(PrescriptionHistoryGetResponse prescriptionHistoryGetResponse)
        {
            if (prescriptionHistoryGetResponse == null)
            {
                _logger.LogError("Null prescription object provided to mapper");
                throw new ArgumentNullException(nameof(prescriptionHistoryGetResponse));
            }

            var allPrescriptionItems = new List<PrescriptionItem>();
            var allCourseItems = new List<GpSystems.Prescriptions.Models.Course>();

            _logger.LogInformation($"Mapping {prescriptionHistoryGetResponse.Courses.Count()} courses.");
            
            if (prescriptionHistoryGetResponse.Courses.Any())
            {
                foreach (var item in prescriptionHistoryGetResponse.Courses)
                {
                    var prescription = MapPrescription(item);
                    var course = MapCourse(item);

                    allPrescriptionItems.Add(prescription);
                    allCourseItems.Add(course);
                }
            }

            PrescriptionListResponse result = new PrescriptionListResponse
            {
                Prescriptions = allPrescriptionItems,
                Courses = allCourseItems,
            };

            _logger.LogInformation($"{result.Prescriptions.Count()} prescriptions mapped");
            _logger.LogInformation($"{result.Courses.Count()} courses mapped");

            return result;
        }

        private GpSystems.Prescriptions.Models.Course MapCourse(PrescriptionCourse course)
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

            return new GpSystems.Prescriptions.Models.Course
            {
                Id = course.Id,
                Name = course.Name,
                Details = details,
            };
        }

        private PrescriptionItem MapPrescription(PrescriptionCourse course)
        {
            return new PrescriptionItem
            {
                OrderDate = course.OrderDate,
                Courses = new List<CourseEntry> { new CourseEntry { CourseId = course.Id } },
                Status = MapPrescriptionStatus(course.Status),
            };
        }

        private static Status MapPrescriptionStatus(string statusString)
        {
            if (!string.IsNullOrEmpty(statusString) && StatusMap.ContainsKey(statusString))
            {
                return StatusMap[statusString];
            }

            return Status.Unknown;
        }

        public CourseListResponse Map(CoursesGetResponse courseGetResponse)
        {
            if (courseGetResponse == null)
            {
                throw new ArgumentNullException(nameof(courseGetResponse));
            }

            var result = new CourseListResponse
            {
                Courses = (courseGetResponse.Courses ?? Enumerable.Empty<Course>()).Select(MapMicrotestCourseToGenericCourse),
            };

            return result;
        }

        private static GpSystems.Prescriptions.Models.Course MapMicrotestCourseToGenericCourse(Course course)
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

            return new GpSystems.Prescriptions.Models.Course
            {
                Id = course.Id,
                Name = course.Name,
                Details = details
            };
        }

        public PrescriptionRequestPostPartialSuccessResponse Map(PrescriptionOrderPartiallySuccessfulResponse prescriptionOrderPartiallySuccessfulResponse)
        {
            var successfulOrders = new List<Order>();
            var unsuccessfulOrders = new List<Order>();

            if (prescriptionOrderPartiallySuccessfulResponse?.PatientRequests != null)
            {
                foreach (var courseOrder in prescriptionOrderPartiallySuccessfulResponse.PatientRequests)
                {
                    if (string.Equals(courseOrder.Status, PrescriptionOrderItemRequestStatus.Success, StringComparison.OrdinalIgnoreCase))
                    {
                        successfulOrders.Add(new Order
                        {
                            CourseId = courseOrder.Id,
                            Name = courseOrder.Name,
                        });
                    }
                    else if (string.Equals(courseOrder.Status, PrescriptionOrderItemRequestStatus.Failed, StringComparison.OrdinalIgnoreCase))
                    {
                        unsuccessfulOrders.Add(new Order
                        {
                            CourseId = courseOrder.Id,
                            Name = courseOrder.Name,
                        });
                    }
                    else
                    {
                        _logger.LogError($"Error mapping order - unexpected status - courseId: {courseOrder.Id}, status: {courseOrder.Status}");
                    }
                }
            }

            var response = new PrescriptionRequestPostPartialSuccessResponse
            {
                SuccessfulOrders = successfulOrders,
                UnsuccessfulOrders = unsuccessfulOrders,
            };

            return response;
        }
    }
}
