using System;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Emis.Models.Records;

namespace NHSOnline.HttpMocks.Emis
{
    public class EmisRecordsDefaultBehaviour : IEmisRecordsBehaviour
    {
        public IActionResult Behave()
        {
            return new JsonResult(new
            {
                MedicationRootObject = new MedicationRootObject()
                {
                    MedicalRecord = new MedicalRecord()
                    {
                        PatientGuid = new Guid().ToString(),
                        Title = "Mr",
                        Forenames = "Test",
                        Surname = "1"
                    }
                }
            });
        }
    }
}