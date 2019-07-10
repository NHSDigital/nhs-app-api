using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class PatientRecordGetResponse
    {
        [JsonProperty("allergies")]
        public AllergyData AllergyData { get; set; }

        [JsonProperty("drugs")]
        public MedicationData MedicationData { get; set; }
        
        [JsonProperty("vaccinations")]
        public ImmunisationData ImmunisationData { get; set; }
    }
}
