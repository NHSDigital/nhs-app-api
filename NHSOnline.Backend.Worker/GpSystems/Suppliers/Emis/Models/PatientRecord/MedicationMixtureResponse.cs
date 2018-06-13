using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public class MedicationMixtureResponse
    {
        public string MixtureName { get; set; }
        public IEnumerable<MedicationMixtureConstituentResponse> Constituents { get; set; }
    }
}