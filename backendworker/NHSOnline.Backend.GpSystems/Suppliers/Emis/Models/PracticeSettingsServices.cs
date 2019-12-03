namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class PracticeSettingsServices
    {
        public bool AppointmentsSupported { get; set; }
        public bool PrescribingSupported { get; set; }
        public bool EpsSupported { get; set; }
        public bool DemographicsUpdateSupported { get; set; }
        public bool PracticePatientCommunicationSupported { get; set; }
        public bool OnlineRegistrationSupported { get; set; }
        public bool PreRegistrationSupported { get; set; }
        public bool OnlineTriageSupported { get; set; }
        public bool MedicalRecordSupported { get; set; }
        public bool RecordSharingSupported { get; set; }

        public override string ToString()
        {
            return $"Appointments enabled: {AppointmentsSupported}, " +
                   $"Prescriptions enabled: {PrescribingSupported}, " +
                   $"Demographics enabled: {DemographicsUpdateSupported}, " +
                   $"Online Registration enabled: {OnlineRegistrationSupported}, " +
                   $"EPS enabled: {EpsSupported}, " +
                   $"Pre Registration enabled: {PreRegistrationSupported}, " +
                   $"PFS messaging enabled: {PracticePatientCommunicationSupported}, " +
                   $"Online Triage enabled: {OnlineTriageSupported}, " +
                   $"Record Sharing enabled: {RecordSharingSupported}, " +
                   $"Medical Record enabled: {MedicalRecordSupported}";
        }
    }
}