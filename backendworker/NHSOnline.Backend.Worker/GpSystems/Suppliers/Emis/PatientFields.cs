using System.Collections.Generic;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public static class PatientFields
    {
        public static IEnumerable<string> PatientSensitiveFields { get; }

        static PatientFields()
        {
            MedicalRecord patient;
            PatientSensitiveFields = new List<string>()
            {
                { nameof(patient.PatientGuid) },
                { nameof(patient.Title) },
                { nameof(patient.Forenames) },
                { nameof(patient.Surname) },
                { nameof(patient.Sex) },
                { nameof(patient.DateOfBirth) }
            };
        }
    }
}