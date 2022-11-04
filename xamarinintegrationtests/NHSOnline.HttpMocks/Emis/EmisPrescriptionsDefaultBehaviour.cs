using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Emis.Models.Prescriptions;

namespace NHSOnline.HttpMocks.Emis
{
    public class EmisPrescriptionsDefaultBehaviour: IEmisPrescriptionsBehaviour
    {
        public IActionResult Behave()
        {
            return new JsonResult(new
            {
                MedicationCourses = new ArrayList
                {
                    new MedicationCourse
                    {
                        MedicationCourseGuid = new Guid().ToString(),
                        Name = "Tablet",
                        Dosage = "10",
                        QuantityRepresentation = "mg",
                        CanBeRequested = true,
                        Constituents = new List<string>
                        {
                            "Test"
                        },
                        PrescriptionType = PrescriptionType.Repeat
                    }
                },
                PrescriptionRequests = new ArrayList
                {
                    new PrescriptionRequest
                    {
                        DateRequested = new DateTimeOffset(2022, 1, 1, 1, 1, 1, TimeSpan.FromMinutes(0)),
                        RequestedByDisplayName = "Made by main user",
                        RequestedByForenames = "Made by",
                        RequestedBySurname = "main user",
                        RequestedMedicationCourses = new List<RequestedMedicationCourse>
                        {
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = "b9803122-c7a8-4489-9ce7-46b298e6f650",
                                RequestedMedicationCourseStatus = RequestedMedicationCourseStatus.Requested
                            },
                        }
                    }
                }
            });
        }
    }
}