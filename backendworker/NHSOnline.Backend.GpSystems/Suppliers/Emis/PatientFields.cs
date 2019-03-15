using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public static class PatientFields
    {
        public static IEnumerable<string> PatientSensitiveFields { get; }

        static PatientFields()
        {
            PatientSensitiveFields = new List<string>()
            {
                nameof(MedicalRecord.PatientGuid),
                nameof(MedicalRecord.Title),
                nameof(MedicalRecord.Forenames),
                nameof(MedicalRecord.Surname),
                nameof(MedicalRecord.Sex),
                nameof(MedicalRecord.DateOfBirth),
                nameof(ContactDetails.MobileNumber),
                nameof(ContactDetails.TelephoneNumber),
                nameof(DemographicsGetResponse.FirstName)
            };
        }
    }
}
