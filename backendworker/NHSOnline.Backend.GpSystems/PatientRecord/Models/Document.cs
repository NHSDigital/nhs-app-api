using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class PatientDocument
    {
        public string Content{ get; set; }
        public bool HasErrored { get; set; }
    }
}