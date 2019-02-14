using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public class Problem
    {
        public string Status { get; set; }
        public string Significance { get; set; }
        public DateTimeOffset? ProblemEndDate { get; set; }
        public Observation Observation { get; set; }
    }
}