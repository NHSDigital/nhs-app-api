using System;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisMyRecordMapper : IEmisMyRecordMapper
    {
        public MyRecordResponse Map(Allergies allergies, Medications medications)
        {
            var result = new MyRecordResponse
            {
                Allergies = allergies,
                Medications = medications
            };

            return result;
        }
    }
}
