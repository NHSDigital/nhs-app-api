namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    internal class Journeys
    {
        public Appointments Appointments { get; set; }
        
        public Prescriptions Prescriptions { get; set; }
        
        public MedicalRecord MedicalRecord  { get; set; }
    }
}