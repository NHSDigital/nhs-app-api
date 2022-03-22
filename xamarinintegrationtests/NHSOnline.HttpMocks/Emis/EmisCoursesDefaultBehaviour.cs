using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Emis.Models.Prescriptions;

namespace NHSOnline.HttpMocks.Emis
{
    public class EmisCoursesDefaultBehaviour: IEmisCoursesBehaviour
    {
        public IActionResult Behave()
        {
            return new JsonResult(new
            {
                Courses = new ArrayList
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
                }
            });
        }
    }
}