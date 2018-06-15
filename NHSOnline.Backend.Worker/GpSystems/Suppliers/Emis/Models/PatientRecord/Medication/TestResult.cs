using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord.Medication
{
    public class TestResult
    {
        public Value Value { get; set; }
        public List<ChildValue> ChildValues { get; set; }
    }
}