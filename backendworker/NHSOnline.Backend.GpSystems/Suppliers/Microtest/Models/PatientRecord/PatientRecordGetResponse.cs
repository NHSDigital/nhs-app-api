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
        
        [JsonProperty("medicalProblems")]
        public ProblemData ProblemData { get; set; }
        
        [JsonProperty("testResults")]
        public TestResultData TestResultData { get; set; }
        
        [JsonProperty("medicalHistory")]
        public MedicalHistoryData MedicalHistoryData { get; set; }

        [JsonProperty("recalls")]
        public RecallData RecallData { get; set; }
        
        [JsonProperty("encounters")]
        public EncounterData EncounterData { get; set; }
        
        [JsonProperty("referral")]
        public ReferralData ReferralData { get; set; }
    }
}
