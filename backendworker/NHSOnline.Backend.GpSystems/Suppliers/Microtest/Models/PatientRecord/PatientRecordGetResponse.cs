using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class PatientRecordGetResponse
    {
        [JsonProperty("allergies")]
        public AllergyData AllergyData { get; set; }
    }
}
