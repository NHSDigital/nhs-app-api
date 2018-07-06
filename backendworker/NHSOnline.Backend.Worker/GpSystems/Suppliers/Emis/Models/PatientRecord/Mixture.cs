using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public class Mixture
    {
        public string MixtureName { get; set; }
        public List<Constituent> Constituents { get; set; }
    }
}